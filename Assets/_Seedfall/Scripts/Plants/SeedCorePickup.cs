using UnityEngine;
using Seedfall.Player;

namespace Seedfall.Plants
{
    // A SeedCore lying in the world that the player can walk into to collect.
    [RequireComponent(typeof(Collider))]
    public class SeedCorePickup : MonoBehaviour
    {
        [SerializeField] private SeedCoreData seedCore;

        private void Reset()
        {
            // Runs once when this component is first added in the Editor -- make sure the
            // collider is a trigger so walking into it doesn't physically block the player.
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerSeedInventory inventory = other.GetComponent<PlayerSeedInventory>();
            if (inventory == null)
            {
                return;
            }

            if (seedCore == null)
            {
                Debug.LogWarning($"SeedCorePickup on '{name}' has no SeedCoreData assigned.");
                return;
            }

            inventory.AddSeedCore(seedCore);
            Debug.Log($"Picked up SeedCore: {seedCore.CoreName} ({seedCore.CoreType})");
            Destroy(gameObject);
        }
    }
}
