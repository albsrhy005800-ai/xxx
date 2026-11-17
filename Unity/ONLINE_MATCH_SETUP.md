# 1v1 player, remote avatar, and combat setup

## Multiplayer presentation
- The server assigns two spawn positions at X=-500m and X=+500m (1,000m separation) on the 4km desert map.
- Unity sends player position and aim orientation at up to 20 updates per second.
- The server stores accepted poses and relays them only to the opponent in that match.
- Unity creates a temporary capsule avatar for the opponent and interpolates its position/rotation.

## Combat and health
- Call `SniperOnlineClient.SendFire()` from the weapon's actual fire event.
- The server applies the shot cooldown and calculates the ray direction from the shooter's latest server-stored yaw/pitch.
- The server checks the ray against a simplified opponent sphere (1.25m radius) up to 2,000m. A hit deals 100 HP; zero HP ends the match.
- Subscribe to `HealthChanged(current, maximum)`, `HitConfirmed(damage, distance)`, and `MatchEnded(winnerId, loserId, reason)` to update the HUD and result panel.
- The client does not send a target ID, hit point, or damage amount.

## Unity scene wiring
1. Keep one `SniperOnlineClient` in the scene and connect it to the server.
2. Add `OnlineMatchPresentation`; assign the client and local player root.
3. Add `OnlinePlayerSync` to the local player; assign the same client, player root, and camera/aim pivot.
4. The Find button calls `FindMatch()`. Connect the weapon's fire event to `SendFire()` (not only the local prototype raycast).
5. Subscribe a HUD component to `HealthChanged` and a result panel to `MatchEnded`.

## Prototype limitations
- The capsule is a placeholder, not a final animated soldier.
- The hitbox is a simple sphere; there are no headshots, armor, terrain occlusion, or full ballistic projectile simulation on the server.
- Client poses are bounded and rate-limited, but movement speed and terrain legality are not yet authoritatively simulated.
- Guest/Google token verification, persistent profiles, reconnect recovery, TLS deployment, and load testing remain outstanding.
- Source changes have not been compiled in Unity or tested between two devices. The server has not been deployed.
