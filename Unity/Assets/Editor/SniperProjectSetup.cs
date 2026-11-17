#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using SniperOnline.World;
using SniperOnline.Gameplay;

namespace SniperOnline.Editor
{
    public static class SniperProjectSetup
    {
        [MenuItem("Sniper Online/Create Desert Test Scene")]
        public static void CreateScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var world = new GameObject("World");
            world.AddComponent<DesertMapGenerator>();

            var light = new GameObject("Sun");
            var sun = light.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.2f;
            light.transform.rotation = Quaternion.Euler(45f, -35f, 0f);

            var player = new GameObject("SniperPlayer");
            player.transform.position = new Vector3(0f, 180f, 0f);
            var controller = player.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.35f;
            controller.center = new Vector3(0f, 0.9f, 0f);
            player.AddComponent<SniperMobileController>();

            var cameraPivot = new GameObject("CameraPivot");
            cameraPivot.transform.SetParent(player.transform, false);
            cameraPivot.transform.localPosition = new Vector3(0f, 1.55f, 0f);
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(cameraPivot.transform, false);
            cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
            var controllerScript = player.GetComponent<SniperMobileController>();
            var field = typeof(SniperMobileController).GetField("cameraPivot", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            field?.SetValue(controllerScript, cameraPivot.transform);

            EditorSceneManager.SaveScene(scene, "Assets/Scenes/DesertTest.unity");
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Scenes/DesertTest.unity", true) };
            Debug.Log("Desert test scene created. Press Play to generate the 4 km x 4 km map.");
        }
    }
}
#endif
