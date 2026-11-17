# Firebase Authentication setup

## What is included
- Guest sign-in uses Firebase Anonymous Authentication.
- Google sign-in is linked to an existing guest account when possible, preserving its Firebase UID.
- The game server verifies Firebase ID tokens with Firebase Admin SDK before allowing matchmaking or gameplay.
- The server uses the verified Firebase UID; it never trusts a UID supplied by the client.

## Firebase project setup
1. Use the Firebase project you created: **`abbas-f4f5`**. Open it in the Firebase Console and confirm the project ID matches exactly.
2. Add your Android app using the exact Unity package/bundle ID.
3. Enable **Authentication → Sign-in method → Anonymous** and **Google**.
4. Download the Android `google-services.json` for this project and place it in the Unity project's `Assets/` folder. Do not commit this file unless you explicitly intend to publish its project configuration.
5. Import the official Firebase Unity SDK packages **Firebase App (Core)** and **Firebase Authentication**. The SDK is distributed separately; do not add a guessed version to Packages/manifest.json.
6. Add the Google Sign-In for Unity plugin, configure its Web/server client ID from the same Firebase project, and obtain a Google ID token. Pass that token to `LinkGoogleAccount(idToken, accessToken)`.
7. Add `FirebaseLoginController` to a scene object and assign the `SniperOnlineClient` component. Call `SignInAsGuest()` for guest entry. After Google Sign-In returns a token, call `LinkGoogleAccount(...)`.

## Server credentials
1. In Firebase Console → Project settings → Service accounts, generate a private service-account key for server use.
2. Store it outside the repository and outside the Unity project. Never upload it to GitHub.
3. Set environment variables on the server:
   - `FIREBASE_PROJECT_ID` to the Firebase project ID.
   - `GOOGLE_APPLICATION_CREDENTIALS` to the absolute path of the service-account JSON file.
4. The server initializes Firebase Admin with Application Default Credentials and verifies token revocation status.

## Local server
```bash
cd Server
npm install
FIREBASE_PROJECT_ID=your-project-id GOOGLE_APPLICATION_CREDENTIALS=/secure/path/service-account.json npm start
```
The Unity client must use the deployed `wss://` URL in production. Do not use `ws://` over the public internet.

## Important
- This commit adds integration code and setup instructions only. It does not create your Firebase project, Google OAuth client, service-account key, or deploy the game server.
- Anonymous users must link Google while still signed in to the same guest Firebase user to preserve identity. If linking reports that the Google credential already belongs to another account, a deliberate account-merge flow is required; do not silently overwrite either account.
- Restrict Firebase API keys in Google Cloud/Firebase as appropriate. The Android config file contains app configuration, not server credentials.
- Server auth is enforced before queue, pose, aim, fire, and match operations. Configure the same Firebase project on client and server.
