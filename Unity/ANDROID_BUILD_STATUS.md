# Android build readiness

## Current verified repository state
- Unity editor version is pinned to 2022.3.62f1.
- C# gameplay and networking source files are present.
- Firebase login source is present, but the Firebase Unity SDK is not included in the package manifest or repository.
- The Unity project currently has no committed `.unity` scene, prefabs, 3D assets, or Android build configuration.
- No Unity editor build, Android APK build, device test, Firebase project, or public game-server deployment has been verified.

## Required external setup
1. Create a Firebase project and Android app; enable Anonymous and Google providers.
2. Import Firebase App/Auth Unity SDK and configure `google-services.json`.
3. Create a Firebase service account for the server. Keep its JSON key outside GitHub and configure `FIREBASE_PROJECT_ID` and `GOOGLE_APPLICATION_CREDENTIALS` on the host.
4. Deploy the Node.js server on a host with TLS and set the Unity client URL to its `wss://` endpoint.
5. Complete and wire the Unity game scene, then configure Android Player Settings and signing.
6. Build and test the APK on two Android devices.

Do not treat a GitHub source-code link as an APK download. A download link should be shared only after a successful Android build has produced an actual APK artifact.
