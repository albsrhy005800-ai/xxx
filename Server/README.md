# Authentication integration

The WebSocket server now requires Firebase Authentication before gameplay.

Required environment:
- `FIREBASE_PROJECT_ID`
- `GOOGLE_APPLICATION_CREDENTIALS` (path to service-account JSON outside this repository)

Install dependencies with `npm install`, then run `npm start`.
See [Firebase setup](../Unity/FIREBASE_AUTH_SETUP.md) for Unity and Firebase Console configuration.

Never commit service-account keys or OAuth secrets.
