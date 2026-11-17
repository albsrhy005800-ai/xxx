# Health HUD setup (Unity uGUI)

1. In the gameplay Canvas, create a Slider for local health and optionally a Text label.
2. Optionally create a second Slider and Text for opponent health.
3. Create a Text element for hit feedback.
4. Create a result panel GameObject with a Text child for VICTORY / DEFEAT; keep it inactive initially.
5. Add `OnlineHealthHUD` to a scene GameObject. Assign the same `SniperOnlineClient` used by multiplayer scripts and drag the UI references into their fields.
6. The local bar updates when you are hit; the opponent bar updates when your shot is confirmed. The result panel opens on match end or opponent departure.

All UI references are optional. The script uses Unity built-in `UnityEngine.UI.Text` and `Slider`. This source has not yet been compiled in Unity or visually tested.