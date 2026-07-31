using UnityEngine;

namespace Seedfall.World
{
    // A trigger the player walks through to be repositioned elsewhere in the same
    // scene -- the Step 6 "expedition" entrance/exit. Two instances are used: one in
    // the plot area pointing at the arena spawn point, one in the arena pointing back
    // at the plot area spawn point. Same-scene teleport, not a scene load, per the
    // locked Step 6 design (avoids save/persistence complexity before the fun gate).
    [RequireComponent(typeof(Collider))]
    public class ExpeditionPortal : MonoBehaviour
    {
        [SerializeField] private Transform destination;
        [SerializeField] private bool entersExpedition;

        private void Reset()
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (destination == null)
            {
                Debug.LogWarning($"ExpeditionPortal on '{name}' has no destination assigned.");
                return;
            }

            // CharacterController maintains its own internal position state and fights
            // a direct transform.position write unless disabled for the move.
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller == null)
            {
                return; // not the player
            }

            controller.enabled = false;
            other.transform.position = destination.position;
            other.transform.rotation = destination.rotation;
            controller.enabled = true;

            Debug.Log($"Portal: teleported to {destination.name}");

            if (ExpeditionManager.Instance != null)
            {
                ExpeditionManager.Instance.SetOnExpedition(entersExpedition);
            }
            else
            {
                Debug.LogWarning("ExpeditionPortal: no ExpeditionManager in scene -- expedition state not updated.");
            }
        }
    }
}
