# Unity project status

Unity editor version: **2022.3.62f1**.

Open the `Unity` folder from Unity Hub. The repository currently contains gameplay/networking C# scripts and package configuration, but it is **not yet a complete playable Unity project**: no scene, prefabs, 3D models, materials, textures, audio, or Android build settings have been committed.

## Current prototype code
- WebSocket client and 1v1 matchmaking integration.
- Online player pose synchronization and temporary opponent capsule presentation.
- Basic sniper scope, weapon, ballistics, HUD, and health/result UI scripts.
- Firebase authentication integration source (requires the official Firebase Unity SDK and project configuration).

## Before an Android APK can be built
1. Create the Unity scene and wire the player, camera, weapon, UI, and online components.
2. Import Firebase App/Auth SDK and configure the Android app in Firebase.
3. Configure a real server URL using secure `wss://`.
4. Configure Android Player Settings, package ID, minimum API, and signing.
5. Build in Unity and test on at least two Android devices.

See [Firebase setup](FIREBASE_AUTH_SETUP.md), [online setup](ONLINE_SETUP.md), and [match setup](ONLINE_MATCH_SETUP.md).

This repository state is source code only. No APK has been built or tested yet.
