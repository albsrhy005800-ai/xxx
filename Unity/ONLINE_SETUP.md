# Unity multiplayer connection

Unity version: **2022.3.62f1**.

## Required packages
- NativeWebSocket is declared in `Packages/manifest.json`.
- Import the official Firebase Unity SDK packages **Firebase App (Core)** and **Firebase Authentication** separately, then add the Android `google-services.json` for your own Firebase project. Follow [FIREBASE_AUTH_SETUP.md](FIREBASE_AUTH_SETUP.md).

## Authentication and connection
1. Create a scene object named `OnlineSession`.
2. Add `SniperOnlineClient` and `FirebaseLoginController` to scene objects; assign the client reference in the login controller.
3. Configure `serverUrl` on `SniperOnlineClient` with the actual server URL. Use `wss://` with valid TLS for public Android builds.
4. Call `FirebaseLoginController.SignInAsGuest()` to create/sign in to a guest account.
5. The controller obtains a Firebase ID token and calls `SniperOnlineClient.ConnectWithToken(token)`.
6. Wait for the server's authenticated/welcome status before calling `FindMatch()`.
7. For Google account linking, complete Google Sign-In with the configured plugin, then pass its ID token and optional access token to `LinkGoogleAccount(idToken, accessToken)`.
8. Add `OnlinePlayerSync` and `OnlineMatchPresentation` as described in [ONLINE_MATCH_SETUP.md](ONLINE_MATCH_SETUP.md). Connect weapon fire to `SendFire()`.

## Important
- The server rejects matchmaking and gameplay messages until Firebase ID token verification succeeds.
- The Firebase client SDK and project configuration are not included in this repository.
- The server has not yet been deployed; set its Firebase service-account credentials securely on the host.
- The current repository does not include a completed Unity scene or Android APK.
