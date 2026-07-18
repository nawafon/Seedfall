using UnityEngine;
using Seedfall.Player;

namespace Seedfall.Tools
{
    // Trigger/detection pattern mirrors SeedPickup.cs exactly (component check on the
    // entering collider, not CompareTag). The wired playerInventory field is what
    // actually receives the rock uses, per the task spec's explicit field list.
    [RequireComponent(typeof(Collider))]
    public class RockPickup : MonoBehaviour
    {
        public PlayerSeedInventory playerInventory;

        private const int USES_PER_ROCK = 3;

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
            if (other.GetComponent<PlayerSeedInventory>() == null)
            {
                return;
            }

            playerInventory.AddRockUses(USES_PER_ROCK);
            Debug.Log("Picked up a rock (+3 uses)");
            Destroy(gameObject);
        }
    }
}
