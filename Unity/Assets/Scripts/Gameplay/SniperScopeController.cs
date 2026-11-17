using UnityEngine;

namespace SniperOnline.Gameplay
{
    /// <summary>Cycles through the agreed 4x, 8x and 15x optic magnifications.</summary>
    public sealed class SniperScopeController : MonoBehaviour
    {
        [SerializeField] private Camera scopeCamera;
        [SerializeField] private float baseFieldOfView = 60f;
        [SerializeField] private float[] magnifications = { 4f, 8f, 15f };

        private int currentIndex;

        public int CurrentMagnification
        {
            get
            {
                if (magnifications == null || magnifications.Length == 0) return 1;
                return Mathf.RoundToInt(magnifications[currentIndex]);
            }
        }

        private void Awake()
        {
            if (scopeCamera == null) scopeCamera = Camera.main;
            if (scopeCamera != null) baseFieldOfView = scopeCamera.fieldOfView;
            ApplyZoom();
        }

        public void NextZoom()
        {
            if (magnifications == null || magnifications.Length == 0) return;
            currentIndex = (currentIndex + 1) % magnifications.Length;
            ApplyZoom();
        }

        public void PreviousZoom()
        {
            if (magnifications == null || magnifications.Length == 0) return;
            currentIndex = (currentIndex - 1 + magnifications.Length) % magnifications.Length;
            ApplyZoom();
        }

        public void SetZoomIndex(int index)
        {
            if (magnifications == null || magnifications.Length == 0) return;
            currentIndex = Mathf.Clamp(index, 0, magnifications.Length - 1);
            ApplyZoom();
        }

        private void ApplyZoom()
        {
            if (scopeCamera == null || magnifications == null || magnifications.Length == 0) return;
            float magnification = Mathf.Max(1f, magnifications[currentIndex]);
            scopeCamera.fieldOfView = Mathf.Clamp(baseFieldOfView / magnification, 1f, 60f);
        }

        public string GetZoomLabel()
        {
            return $"{CurrentMagnification}x";
        }
    }
}
