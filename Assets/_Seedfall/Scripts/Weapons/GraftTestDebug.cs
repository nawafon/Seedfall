// No longer TEMP test scaffolding -- the Alpha1-4 test-graft shortcuts were removed
// once GraftMenuUI covered the same combinations for real. This is now the only
// I-key debug view of PlayerSeedInventory's contents (seeds/cores/sap), so it stays.
using UnityEngine;
using Seedfall.Player;

namespace Seedfall.Weapons
{
    public class GraftTestDebug : MonoBehaviour
    {
        public PlayerSeedInventory playerInventory;
        public WeaponInventory weaponInventory;
        public PlayerHealth playerHealth;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log(playerInventory.GetDebugSummary());
                if (weaponInventory != null)
                {
                    Debug.Log(weaponInventory.GetDebugSummary());
                }
                if (playerHealth != null)
                {
                    Debug.Log($"HP: {playerHealth.CurrentHealth}/{playerHealth.MaxHealth}");
                }
            }
        }
    }
}
