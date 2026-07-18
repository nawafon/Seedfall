// TEMP: remove after Step 4a confirmed
using UnityEngine;
using Seedfall.Plants;

namespace Seedfall.Weapons
{
    public class GraftTestDebug : MonoBehaviour
    {
        public GraftingSystem graftingSystem;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                WeaponData result = graftingSystem.TryGraft(SeedCoreType.Growth, SeedCoreType.Heat);
                if (result != null)
                {
                    Debug.Log($"Test 1: got {result.weaponName}");
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                WeaponData result = graftingSystem.TryGraft(SeedCoreType.Growth, SeedCoreType.Wind);
                if (result != null)
                {
                    Debug.Log($"Test 2: got {result.weaponName}");
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                WeaponData result = graftingSystem.TryGraft(SeedCoreType.Heat, SeedCoreType.Wind);
                if (result != null)
                {
                    Debug.Log($"Test 3: got {result.weaponName}");
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                WeaponData result = graftingSystem.TryGraft(SeedCoreType.Growth, SeedCoreType.Growth);
                if (result != null)
                {
                    Debug.Log($"Test 4: got {result.weaponName}");
                }
            }
        }
    }
}
