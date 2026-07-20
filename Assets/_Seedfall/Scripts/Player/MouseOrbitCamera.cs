using System.Collections;
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

        // Small positional jitter added on top of the computed orbit position, folded in
        // right where that position is assigned below -- avoids any script-execution-order
        // race, since nothing outside LateUpdate ever writes to transform directly here.
        private Vector3 _shakeOffset = Vector3.zero;
        private Coroutine _shakeCoroutine;

        public void Shake(float duration, float magnitude)
        {
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
            }
            _shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
        }

        private IEnumerator ShakeRoutine(float duration, float magnitude)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                _shakeOffset = Random.insideUnitSphere * magnitude;
                elapsed += Time.deltaTime;
                yield return null;
            }
            _shakeOffset = Vector3.zero;
            _shakeCoroutine = null;
        }

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

            // Every menu (GraftMenuUI, BreakSeedMenuUI) unlocks the cursor while open --
            // use that as the "a menu is open" signal so the camera doesn't spin around
            // while the player is trying to click UI with the mouse.
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                return;
            }

            _yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            _pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 focusPoint = target.position + Vector3.up * targetHeight;
            Vector3 desiredPosition = focusPoint - rotation * Vector3.forward * distance;

            transform.position = desiredPosition + _shakeOffset;
            transform.rotation = rotation;
        }
    }
}
