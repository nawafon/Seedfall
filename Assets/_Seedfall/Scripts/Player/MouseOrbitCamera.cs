using UnityEngine;

namespace Seedfall.Player
{
    // Mouse-controlled orbit camera. Always orbits around and looks at "target".
    // Yaw/pitch come purely from mouse input, not from the target's own rotation,
    // so this has no feedback loop with PlayerController's camera-relative movement.
    public class MouseOrbitCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float distance = 6f;
        [SerializeField] private float mouseSensitivity = 3f;
        [SerializeField] private float minPitch = 10f;
        [SerializeField] private float maxPitch = 80f;
        [SerializeField] private float targetHeight = 1.5f; // look at roughly head height, not the player's feet/pivot

        private float _yaw;
        private float _pitch = 25f;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            // Escape toggles the cursor lock so you can still click into the Editor UI during testing.
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                bool isLocked = Cursor.lockState == CursorLockMode.Locked;
                Cursor.lockState = isLocked ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = isLocked;
            }
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            _yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            _pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 focusPoint = target.position + Vector3.up * targetHeight;
            Vector3 desiredPosition = focusPoint - rotation * Vector3.forward * distance;

            transform.position = desiredPosition;
            transform.rotation = rotation;
        }
    }
}
