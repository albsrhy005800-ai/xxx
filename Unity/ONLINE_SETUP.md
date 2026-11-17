# Unity multiplayer connection

Unity version: **2022.3.62f1**.

## Package
The Unity manifest includes NativeWebSocket from its official UPM Git repository. Open the `Unity` folder in Unity Hub and allow Package Manager to resolve packages.

## Connect the game
1. Create an empty GameObject in the gameplay scene, name it `OnlineSession`.
2. Add the `SniperOnlineClient` component.
3. Set **Server Url** to the server address. Use `ws://127.0.0.1:8080` only when the server is running on the same development computer. For a deployed server use its real `wss://...` address.
4. Call `Connect()`, then wait for `StatusChanged` / the `welcome` message.
5. Call `FindMatch()`. When two clients are waiting, both receive `MatchFound(matchId, spawnSlot, opponentId)`.
6. Forward local camera yaw/pitch with `SendAim(yaw, pitch)`; call `SendFire()` when the weapon fires. Subscribe to `OpponentAim`, `OpponentFired`, and `OpponentLeft` to update the remote player.

Example:
```csharp
public SniperOnlineClient online;

private void OnEnable()
{
    online.MatchFound += OnMatchFound;
    online.OpponentAim += OnOpponentAim;
}
private void OnMatchFound(string matchId, int spawnSlot, string opponentId)
{
    // Spawn local and remote players at the two desert spawn positions.
}
private void OnOpponentAim(string id, float yaw, float pitch)
{
    // Apply the remote player's replicated aim to their avatar.
}
```

## Current limitations
- This is a transport adapter, not a finished multiplayer gameplay integration. The scene still needs player spawning, remote avatar interpolation, UI buttons, and wiring from the weapon/HUD.
- The server currently relays shot events only. It does **not** calculate bullet trajectories, validate hits, apply damage, or decide winners. Never trust a client-reported hit.
- The current server assigns temporary anonymous connection IDs. Guest/Google identity verification, persistent profiles, reconnect recovery, TLS termination, and production deployment are not configured.
- Do not ship with localhost. Production Android builds must use a public `wss://` endpoint with valid TLS.
