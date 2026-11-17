using UnityEngine;

namespace SniperOnline.Gameplay
{
    /// <summary>Simple touch-look and keyboard test controller. Add a CharacterController to the same object.</summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class SniperMobileController : MonoBehaviour
    {
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float lookSensitivity = 0.12f;
        [SerializeField] private float gravity = -18f;
        private CharacterController motor;
        private float pitch;
        private float verticalVelocity;
        private int lookFinger = -1;

        private void Awake()
        {
            motor = GetComponent<CharacterController>();
            if (cameraPivot == null && Camera.main != null) cameraPivot = Camera.main.transform;
        }

        private void Update()
        {
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
            Vector3 move = (transform.right * x + transform.forward * z).normalized * moveSpeed;
            if (motor.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
            verticalVelocity += gravity * Time.deltaTime;
            motor.Move((move + Vector3.up * verticalVelocity) * Time.deltaTime);

            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.position.x < Screen.width * 0.42f) continue;
                    if (touch.phase == TouchPhase.Began && lookFinger == -1) lookFinger = touch.fingerId;
                    if (touch.fingerId != lookFinger) continue;
                    if (touch.phase == TouchPhase.Moved) ApplyLook(touch.deltaPosition);
                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) lookFinger = -1;
                }
            }
#if UNITY_EDITOR || UNITY_STANDALONE
            ApplyLook(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 8f);
#endif
        }

        private void ApplyLook(Vector2 delta)
        {
            transform.Rotate(Vector3.up * delta.x * lookSensitivity, Space.World);
            pitch = Mathf.Clamp(pitch - delta.y * lookSensitivity, -80f, 80f);
            if (cameraPivot != null) cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }
}
