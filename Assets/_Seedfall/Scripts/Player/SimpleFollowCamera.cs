using UnityEngine;

namespace Seedfall.Player
{
    // Plain follow camera: copies the target's position with a fixed world-space offset.
    // Deliberately never touches rotation -- the camera keeps whatever angle you set on it
    // in the Inspector. This avoids a feedback loop where a camera parented to a rotating
    // Player would itself rotate, which would then change the movement-direction reference
    // PlayerController reads next frame.
    public class SimpleFollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 3f, -6f);

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            transform.position = target.position + offset;
        }
    }
}
