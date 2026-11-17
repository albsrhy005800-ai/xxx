using UnityEngine;

namespace SniperOnline.Gameplay
{
    /// <summary>Attach to the local player. Sends the player's world pose at 20 Hz.</summary>
    public sealed class OnlinePlayerSync : MonoBehaviour
    {
        [SerializeField] private SniperOnline.Core.SniperOnlineClient client;
        [SerializeField] private Transform playerRoot;
        [SerializeField] private Transform aimTransform;
        [SerializeField, Range(5f, 30f)] private float updatesPerSecond = 20f;
        private float nextSend;

        private void Awake()
        {
            if (playerRoot == null) playerRoot = transform;
            if (aimTransform == null && Camera.main != null) aimTransform = Camera.main.transform;
        }

        private void Update()
        {
            if (client == null || !client.IsConnected || string.IsNullOrEmpty(client.MatchId)) return;
            if (Time.unscaledTime < nextSend) return;
            nextSend = Time.unscaledTime + 1f / updatesPerSecond;
            Transform aim = aimTransform != null ? aimTransform : playerRoot;
            float yaw = aim.eulerAngles.y;
            float pitch = aim.localEulerAngles.x;
            if (pitch > 180f) pitch -= 360f;
            client.SendPose(playerRoot.position, yaw, pitch);
        }
    }
}
