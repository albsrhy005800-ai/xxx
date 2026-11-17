using UnityEngine;

namespace SniperOnline.Gameplay
{
    /// <summary>
    /// Minimal 1v1 presentation: positions the local player 1000m from the opponent
    /// and creates/interpolates a visible capsule for the remote player.
    /// Replace the generated capsule with the final sniper character prefab later.
    /// </summary>
    public sealed class OnlineMatchPresentation : MonoBehaviour
    {
        [SerializeField] private SniperOnline.Core.SniperOnlineClient client;
        [SerializeField] private Transform localPlayer;
        [SerializeField] private float spawnDistance = 500f;
        [SerializeField] private float remoteMoveSpeed = 12f;
        [SerializeField] private float remoteTurnSpeed = 360f;

        private Transform remoteRoot;
        private Vector3 targetPosition;
        private Quaternion targetRotation = Quaternion.identity;
        private bool hasRemotePose;

        private void OnEnable()
        {
            if (client == null) return;
            client.MatchFound += OnMatchFound;
            client.OpponentPose += OnOpponentPose;
            client.OpponentLeft += OnOpponentLeft;
        }
        private void OnDisable()
        {
            if (client == null) return;
            client.MatchFound -= OnMatchFound;
            client.OpponentPose -= OnOpponentPose;
            client.OpponentLeft -= OnOpponentLeft;
        }

        private void OnMatchFound(string matchId, int spawnSlot, string opponentId)
        {
            if (localPlayer == null)
            {
                Debug.LogError("[SniperOnline] Assign Local Player in OnlineMatchPresentation.");
                return;
            }
            float localX = spawnSlot == 0 ? -spawnDistance : spawnDistance;
            localPlayer.position = new Vector3(localX, Mathf.Max(1f, localPlayer.position.y), 0f);
            localPlayer.rotation = Quaternion.Euler(0f, spawnSlot == 0 ? 90f : -90f, 0f);

            if (remoteRoot != null) Destroy(remoteRoot.gameObject);
            GameObject remote = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            remote.name = "RemoteSniper_" + opponentId;
            remoteRoot = remote.transform;
            remoteRoot.position = new Vector3(-localX, 1f, 0f);
            remoteRoot.rotation = Quaternion.Euler(0f, spawnSlot == 0 ? -90f : 90f, 0f);
            targetPosition = remoteRoot.position;
            targetRotation = remoteRoot.rotation;
            hasRemotePose = true;

            // Distinctive head marker so the remote avatar is easy to see at close range.
            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "RemoteHead";
            head.transform.SetParent(remoteRoot, false);
            head.transform.localPosition = new Vector3(0f, 0.72f, 0f);
            head.transform.localScale = new Vector3(0.48f, 0.48f, 0.48f);
            Collider headCollider = head.GetComponent<Collider>();
            if (headCollider != null) Destroy(headCollider);
        }

        private void OnOpponentPose(string playerId, Vector3 position, float yaw, float pitch)
        {
            if (remoteRoot == null) return;
            targetPosition = position;
            targetRotation = Quaternion.Euler(0f, yaw, 0f);
            hasRemotePose = true;
        }

        private void OnOpponentLeft()
        {
            hasRemotePose = false;
            if (remoteRoot != null) Destroy(remoteRoot.gameObject);
            remoteRoot = null;
        }

        private void Update()
        {
            if (remoteRoot == null || !hasRemotePose) return;
            remoteRoot.position = Vector3.Lerp(remoteRoot.position, targetPosition, Time.deltaTime * remoteMoveSpeed);
            remoteRoot.rotation = Quaternion.RotateTowards(remoteRoot.rotation, targetRotation, remoteTurnSpeed * Time.deltaTime);
        }
    }
}
