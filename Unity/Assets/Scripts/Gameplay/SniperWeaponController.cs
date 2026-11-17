using UnityEngine;

namespace SniperOnline.Gameplay
{
    /// <summary>
    /// Local prototype weapon controller. In online matches, hit validation and damage
    /// must be moved to the authoritative game server.
    /// </summary>
    public sealed class SniperWeaponController : MonoBehaviour
    {
        [Header("Weapon")]
        [SerializeField] private Camera aimCamera;
        [SerializeField] private float rangeMeters = 2000f;
        [SerializeField] private float shotsPerMinute = 35f;
        [SerializeField] private LayerMask hitMask = ~0;

        [Header("Ballistics")]
        [SerializeField] private float muzzleVelocity = 850f;
        [SerializeField] private float gravityScale = 1f;
        [SerializeField] private float windMetersPerSecond = 0f;

        private float nextShotTime;

        public float MuzzleVelocity => muzzleVelocity;
        public float GravityScale => gravityScale;
        public float WindMetersPerSecond => windMetersPerSecond;

        private void Awake()
        {
            if (aimCamera == null)
                aimCamera = Camera.main;
        }

        public bool Fire()
        {
            if (aimCamera == null || Time.time < nextShotTime)
                return false;

            nextShotTime = Time.time + 60f / Mathf.Max(1f, shotsPerMinute);
            Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, rangeMeters, hitMask, QueryTriggerInteraction.Ignore))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.red, 1.5f);
                Debug.Log($"Prototype shot hit: {hit.collider.name} at {hit.distance:F1} m");
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * rangeMeters, Color.yellow, 1.5f);
            }

            return true;
        }

        public void SetWind(float metersPerSecond)
        {
            windMetersPerSecond = metersPerSecond;
        }
    }
}
