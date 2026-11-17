using UnityEngine;
using UnityEngine.UI;

namespace SniperOnline.Gameplay
{
    /// <summary>
    /// Connect these public methods to Unity UI Button OnClick events.
    /// </summary>
    public sealed class SniperHUDController : MonoBehaviour
    {
        [SerializeField] private SniperWeaponController weapon;
        [SerializeField] private SniperScopeController scope;
        [SerializeField] private Text zoomLabel;

        private void Start()
        {
            RefreshZoomLabel();
        }

        public void OnFirePressed()
        {
            if (weapon != null) weapon.Fire();
        }

        public void OnZoomPressed()
        {
            if (scope == null) return;
            scope.NextZoom();
            RefreshZoomLabel();
        }

        public void OnPreviousZoomPressed()
        {
            if (scope == null) return;
            scope.PreviousZoom();
            RefreshZoomLabel();
        }

        private void RefreshZoomLabel()
        {
            if (zoomLabel != null && scope != null)
                zoomLabel.text = scope.GetZoomLabel();
        }
    }
}
