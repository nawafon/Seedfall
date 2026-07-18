using UnityEngine;
using Seedfall.Player;

namespace Seedfall.Plants
{
    // A SeedData lying in the world that the player can walk into to collect.
    // Mirrors SeedCorePickup's exact interaction pattern -- new file, SeedCorePickup.cs
    // is left untouched and unused for now (cleanup happens in a later step).
    [RequireComponent(typeof(Collider))]
    public class SeedPickup : MonoBehaviour
    {
        [SerializeField] private SeedData seedData;

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
            PlayerSeedInventory inventory = other.GetComponent<PlayerSeedInventory>();
            if (inventory == null)
            {
                return;
            }

            if (seedData == null)
            {
                Debug.LogWarning($"SeedPickup on '{name}' has no SeedData assigned.");
                return;
            }

            inventory.AddSeed(seedData);
            Debug.Log($"Picked up Seed: {seedData.DisplayName}");
            Destroy(gameObject);
        }
    }
}
