# 1v1 player and remote-avatar setup

## Added multiplayer presentation
- The server assigns two spawn positions at X=-500m and X=+500m (1,000m separation) on the 4km desert map.
- The Unity client sends position and aim orientation at up to 20 updates per second while in a match.
- The server validates numeric values and world bounds, rate-limits pose messages, and relays them only to the opponent in that match.
- Unity creates a visible temporary capsule avatar for the opponent and interpolates its position/rotation.

## Unity scene wiring
1. Keep one `SniperOnlineClient` in the scene and connect it to the server.
2. Add `OnlineMatchPresentation` to a scene GameObject. Drag the client component into **Client**, and the locally controlled player root into **Local Player**.
3. Add `OnlinePlayerSync` to the local player. Assign the same client, the player root, and the camera (or aim pivot) to **Aim Transform**.
4. Your match UI's Find button calls `SniperOnlineClient.FindMatch()`. Your weapon fire event calls `SendFire()`.
5. On a match, the local player is placed at -500m or +500m and the opponent capsule appears at the opposite spawn.

## Important prototype boundaries
- The capsule is a placeholder, not a final animated soldier or a networked character prefab.
- Movement replication sends the local root transform; the project still needs a real first-person movement controller and collision/anti-cheat authority.
- The server currently relays shots but does not compute ballistic trajectories, verify hits, apply health/damage, or declare a winner.
- This change is source code only. It has not been run in Unity or tested with two connected devices. The server must be deployed and configured with a public TLS `wss://` address before remote players can connect over the internet.
