# Unity multiplayer connection

Unity version: **2022.3.62f1**.

## Package
The Unity manifest includes NativeWebSocket from its official UPM Git repository. Open the `Unity` folder from Unity Hub and allow Package Manager to resolve packages.

## Connect the game
1. Create an empty GameObject named `OnlineSession` and add `SniperOnlineClient`.
2. Set **Server Url** to the actual server address. Use `ws://127.0.0.1:8080` only for a server on the same development computer. Deployed Android clients need a public `wss://...` address with valid TLS.
3. Call `Connect()`, then wait for `StatusChanged` / the `welcome` message.
4. Call `FindMatch()`. Two queued clients receive `MatchFound(matchId, spawnSlot, opponentId)`.
5. Add `OnlinePlayerSync` and `OnlineMatchPresentation` as described in `ONLINE_MATCH_SETUP.md`.
6. Connect the weapon fire event to `SniperOnlineClient.SendFire()`.

## Combat events
- `HealthChanged(current, maximum)`: update the player's health bar.
- `HitConfirmed(damage, distance)`: show a hit marker / hit confirmation.
- `MatchEnded(winnerId, loserId, reason)`: show the match result.
- `OpponentPose`, `OpponentFired`, and `OpponentLeft`: update the remote avatar and match UI.

## Important
The server now decides hits from its stored player poses; the client cannot submit damage or a claimed target. This is still a prototype: poses are not yet checked against authoritative movement physics, and the hitbox is a simplified sphere. Account token verification, persistence, reconnect, deployment, and two-device testing are not complete.
