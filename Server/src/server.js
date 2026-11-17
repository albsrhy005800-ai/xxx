const http = require("node:http");
const { randomUUID } = require("node:crypto");
const { WebSocketServer, WebSocket } = require("ws");

const PORT = Number(process.env.PORT || 8080);
const MAX_PLAYERS = Number(process.env.MAX_PLAYERS || 500);
const SHOT_COOLDOWN_MS = 1500;
const POSE_INTERVAL_MS = 50;
const MAX_MESSAGE_BYTES = 4096;
const clients = new Map();
const waiting = [];
const rooms = new Map();

const server = http.createServer((req, res) => {
  if (req.url === "/health") {
    res.writeHead(200, { "content-type": "application/json" });
    return res.end(JSON.stringify({
      ok: true, server: "sniper-online-single-server",
      connectedPlayers: clients.size, activeMatches: rooms.size,
      waitingPlayers: waiting.length
    }));
  }
  res.writeHead(200, { "content-type": "text/plain" });
  res.end("Sniper Online game server");
});
const wss = new WebSocketServer({ server, maxPayload: MAX_MESSAGE_BYTES, perMessageDeflate: false });

function send(ws, type, data = {}) {
  if (ws.readyState === WebSocket.OPEN) ws.send(JSON.stringify({ type, ...data }));
}
function broadcast(room, type, data = {}, exceptId = null) {
  for (const id of room.players) {
    if (id === exceptId) continue;
    const player = clients.get(id);
    if (player) send(player.ws, type, data);
  }
}
function removeFromQueue(id) {
  const index = waiting.indexOf(id);
  if (index !== -1) waiting.splice(index, 1);
}
function createMatch(firstId, secondId) {
  const matchId = randomUUID();
  const room = { id: matchId, players: [firstId, secondId], createdAt: Date.now() };
  rooms.set(matchId, room);
  for (const [index, id] of room.players.entries()) {
    const player = clients.get(id);
    if (!player) continue;
    player.roomId = matchId;
    player.lastShotAt = 0;
    player.lastPoseAt = 0;
    player.pose = { x: index === 0 ? -500 : 500, y: 1, z: 0, yaw: index === 0 ? 90 : -90, pitch: 0 };
    send(player.ws, "match_found", {
      matchId, opponentId: room.players[1 - index], spawnSlot: index,
      spawnPosition: { x: player.pose.x, y: player.pose.y, z: player.pose.z },
      rules: { mode: "1v1", map: "desert_4km", maxRangeMeters: 2000 }
    });
  }
}
function joinQueue(id) {
  const player = clients.get(id);
  if (!player || player.roomId) return;
  removeFromQueue(id);
  while (waiting.length) {
    const opponentId = waiting.shift();
    const opponent = clients.get(opponentId);
    if (opponent && !opponent.roomId && opponent.ws.readyState === WebSocket.OPEN) {
      createMatch(opponentId, id);
      return;
    }
  }
  waiting.push(id);
  send(player.ws, "queue_joined", { position: waiting.length });
}
function leaveMatch(id) {
  const player = clients.get(id);
  if (!player || !player.roomId) return;
  const room = rooms.get(player.roomId);
  player.roomId = null;
  if (!room) return;
  rooms.delete(room.id);
  broadcast(room, "opponent_left", { matchId: room.id }, id);
  for (const otherId of room.players) {
    const other = clients.get(otherId);
    if (other) { other.roomId = null; other.pose = null; }
  }
}

wss.on("connection", (ws) => {
  if (clients.size >= MAX_PLAYERS) {
    send(ws, "server_full");
    ws.close(1013, "Server full");
    return;
  }
  const id = randomUUID();
  clients.set(id, { ws, roomId: null, lastShotAt: 0, lastPoseAt: 0, lastPongAt: Date.now(), pose: null });
  send(ws, "welcome", { playerId: id, protocol: 1 });

  ws.on("pong", () => {
    const player = clients.get(id);
    if (player) player.lastPongAt = Date.now();
  });
  ws.on("message", (raw) => {
    let message;
    try { message = JSON.parse(raw.toString()); }
    catch { return send(ws, "error", { code: "BAD_JSON" }); }
    if (!message || typeof message.type !== "string")
      return send(ws, "error", { code: "BAD_MESSAGE" });

    switch (message.type) {
      case "find_match": joinQueue(id); break;
      case "cancel_queue":
        removeFromQueue(id);
        send(ws, "queue_cancelled");
        break;
      case "leave_match": leaveMatch(id); break;
      case "aim": {
        const player = clients.get(id);
        const room = player && rooms.get(player.roomId);
        const yaw = Number(message.yaw), pitch = Number(message.pitch);
        if (!room || !Number.isFinite(yaw) || !Number.isFinite(pitch) ||
            Math.abs(yaw) > 360 || Math.abs(pitch) > 89) return;
        broadcast(room, "opponent_aim", { playerId: id, yaw, pitch }, id);
        break;
      }
      case "pose": {
        const player = clients.get(id);
        const room = player && rooms.get(player.roomId);
        if (!room) return;
        const now = Date.now();
        if (now - player.lastPoseAt < POSE_INTERVAL_MS) return;
        const x = Number(message.x), y = Number(message.y), z = Number(message.z);
        const yaw = Number(message.yaw), pitch = Number(message.pitch);
        if (![x, y, z, yaw, pitch].every(Number.isFinite) ||
            Math.abs(x) > 2000 || y < -5 || y > 300 || Math.abs(z) > 2000 ||
            Math.abs(yaw) > 360 || Math.abs(pitch) > 89) return;
        player.lastPoseAt = now;
        player.pose = { x, y, z, yaw, pitch };
        broadcast(room, "opponent_pose", { playerId: id, ...player.pose, serverTime: now }, id);
        break;
      }
      case "fire": {
        const player = clients.get(id);
        const room = player && rooms.get(player.roomId);
        if (!room) return send(ws, "error", { code: "NOT_IN_MATCH" });
        const now = Date.now();
        if (now - player.lastShotAt < SHOT_COOLDOWN_MS)
          return send(ws, "shot_rejected", { reason: "COOLDOWN" });
        player.lastShotAt = now;
        // Prototype relay only: server-side trajectory, hit validation and damage are not implemented.
        broadcast(room, "opponent_fired", { playerId: id, serverTime: now }, id);
        send(ws, "shot_accepted", { serverTime: now });
        break;
      }
      default: send(ws, "error", { code: "UNKNOWN_MESSAGE" });
    }
  });
  ws.on("close", () => { removeFromQueue(id); leaveMatch(id); clients.delete(id); });
  ws.on("error", () => {});
});

const heartbeat = setInterval(() => {
  for (const [id, player] of clients) {
    if (Date.now() - player.lastPongAt > 45000) { player.ws.terminate(); continue; }
    if (player.ws.readyState === WebSocket.OPEN) player.ws.ping();
  }
}, 15000);
server.listen(PORT, "0.0.0.0", () => console.log(`Sniper Online single server listening on ${PORT}`));
function shutdown() {
  clearInterval(heartbeat);
  for (const player of clients.values()) player.ws.close(1001, "Server shutting down");
  server.close(() => process.exit(0));
  setTimeout(() => process.exit(1), 10000).unref();
}
process.on("SIGINT", shutdown);
process.on("SIGTERM", shutdown);
