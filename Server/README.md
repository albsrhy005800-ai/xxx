# Sniper Online — Single Server Prototype

This folder contains one Node.js WebSocket server process. It owns the waiting queue and creates isolated 1v1 match rooms on the same server instance.

## Run locally
1. Install Node.js 20 or newer.
2. From this folder run `npm install`.
3. Run `npm start`.
4. Check `http://localhost:8080/health`.

## WebSocket protocol
- On connect: `welcome` with a server-issued player ID.
- Client sends `{"type":"find_match"}`; server pairs two connected players.
- Server sends `match_found` to each player with the same match ID and different spawn slots.
- `cancel_queue`, `leave_match`, `aim`, and `fire` are supported.
- Health endpoint: `GET /health`.

## Deploy one instance
Build and run the Dockerfile on a host that supports persistent WebSocket connections. Configure the host to run exactly one application instance and expose port 8080. Set `PORT` if required by the host.

## Important prototype limits
- This is a single-process prototype, not a production-hardened public service.
- Queue and rooms are held in memory and are lost on restart.
- No Unity client adapter is connected yet.
- `fire` currently relays a shot event only. Server-authoritative world simulation, hit validation, damage, persistence, authentication, TLS (wss), abuse protection, reconnect/resume, monitoring, and load testing are still required before public release.
- A single instance is a single point of failure and has finite capacity. Do not advertise 500 concurrent players as tested capacity; MAX_PLAYERS is only a configurable connection guard.
