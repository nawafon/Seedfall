using UnityEngine;

namespace Seedfall.Player
{
    // Third-person movement driven by CharacterController.
    // Movement direction is relative to the main camera's flattened facing.
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float gravity = -20f;

        // Caps the per-frame time step used for gravity/movement. Without this, a single
        // frame hitch (e.g. the Editor stalling after losing OS focus) can produce a huge
        // Time.deltaTime, which accumulates into a huge single-frame gravity step and
        // punches CharacterController.Move() straight through thin geometry (the ground
        // Plane's MeshCollider has zero thickness) -- confirmed as the cause of a fall-
        // through-the-floor bug after respawn.
        [SerializeField] private float maxDeltaTime = 0.1f;

        private CharacterController _controller;
        private Camera _mainCamera;
        private float _verticalVelocity;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _mainCamera = Camera.main;
        }

        // Called by PlayerHealth right after a respawn teleport so no residual fall speed
        // from before death carries into the new position.
        public void ResetVerticalVelocity()
        {
            _verticalVelocity = 0f;
        }

        private void Update()
        {
            // Every menu (GraftMenuUI, BreakSeedMenuUI) unlocks the cursor while open --
            // same signal MouseOrbitCamera uses to freeze. Without this the player could
            // walk around blindly while a menu is open and the camera is frozen.
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                return;
            }

            float dt = Mathf.Min(Time.deltaTime, maxDeltaTime);

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Build a move direction relative to the camera so WASD matches what the player sees on screen.
            Vector3 moveDir = Vector3.zero;
            if ((Mathf.Abs(horizontal) > 0.0001f || Mathf.Abs(vertical) > 0.0001f) && _mainCamera != null)
            {
                Vector3 camForward = _mainCamera.transform.forward;
                Vector3 camRight = _mainCamera.transform.right;
                camForward.y = 0f;
                camRight.y = 0f;
                camForward.Normalize();
                camRight.Normalize();

                moveDir = camForward * vertical + camRight * horizontal;
            }

            // Smoothly rotate the player to face the direction it's moving.
            if (moveDir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * dt);
            }

            // Gravity handling: keep a small downward velocity while grounded so
            // CharacterController.isGrounded stays true and the player sticks to slopes/ground.
            // No jump input, so vertical velocity only ever comes from gravity.
            if (_controller.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }
            else
            {
                _verticalVelocity += gravity * dt;
            }

            Vector3 motion = moveDir * moveSpeed;
            motion.y = _verticalVelocity;

            _controller.Move(motion * dt);
        }
    }
}
