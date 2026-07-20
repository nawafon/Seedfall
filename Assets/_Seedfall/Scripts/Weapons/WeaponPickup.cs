using UnityEngine;
using Seedfall.Player;

namespace Seedfall.Weapons
{
    // A grafted weapon that fell to the ground because all 3 inventory slots were full.
    // Picked up via PlayerInteract's E-press arbitration (TryPickUp) -- unlike
    // SeedPickup/RockPickup, this is deliberate walk-near-and-press-E, not an automatic
    // OnTriggerEnter, per the locked Step 5 design. Trigger collider (not solid) so the
    // player can step onto/into it without being physically stopped.
    [RequireComponent(typeof(Collider))]
    public class WeaponPickup : MonoBehaviour
    {
        public WeaponData weapon;

        private void Reset()
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        public bool TryPickUp(WeaponInventory inventory)
        {
            if (weapon == null || inventory == null)
            {
                return false;
            }

            if (!inventory.TryAddWeapon(weapon))
            {
                Debug.Log("Inventory full -- can't pick up weapon.");
                return false;
            }

            Debug.Log($"Picked up weapon: {weapon.weaponName}");
            Destroy(gameObject);
            return true;
        }
    }
}
