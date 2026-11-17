# Server-authoritative combat prototype

The server stores each player's latest accepted pose and derives the shot direction from that pose. A `fire` message contains no target, hit point, or damage value. The server tests a ray against a simplified 1.25 m-radius sphere centered on the opponent's stored body position, within 2,000 m. A confirmed hit deals 100 HP and ends the match at zero HP.

Run `npm test` to execute the pure ballistics tests, then `npm start` to launch the WebSocket server.

This is an early prototype, not production anti-cheat: client-reported poses are range- and rate-limited but are not yet validated against movement speed, terrain, or authoritative physics. The hitbox is a sphere, not a full character collider; there are no headshots, armor, persistence, authentication, TLS termination, or reconnect recovery.