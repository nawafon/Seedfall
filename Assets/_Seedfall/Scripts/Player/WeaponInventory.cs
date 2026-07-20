using UnityEngine;
using Seedfall.Weapons;

namespace Seedfall.Player
{
    // Holds up to 3 grafted weapons. Equipping a slot drives the same BareHandMelee
    // component bare-hand attacks already use (via SetStats/ResetToBareHandStats) --
    // no parallel attack system. Empty slot or no weapons at all = bare fists.
    [RequireComponent(typeof(BareHandMelee))]
    public class WeaponInventory : MonoBehaviour
    {
        // WeaponData has no radius field (confirmed during inspection) -- fixed for all
        // grafted weapons at MVP.
        private const float WeaponAttackRadius = 0.6f;

        private WeaponData[] slots = new WeaponData[3];
        private int _equippedIndex = -1; // -1 = no slot selected, bare fists

        private BareHandMelee _meleeAttacker;

        public WeaponData GetEquipped() => _equippedIndex >= 0 ? slots[_equippedIndex] : null;
        public int EquippedIndex => _equippedIndex;

        private void Awake()
        {
            _meleeAttacker = GetComponent<BareHandMelee>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) { EquipSlot(0); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { EquipSlot(1); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { EquipSlot(2); }
        }

        public bool TryAddWeapon(WeaponData weapon)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null)
                {
                    slots[i] = weapon;
                    return true;
                }
            }

            return false;
        }

        public void EquipSlot(int index)
        {
            if (index < 0 || index >= slots.Length)
            {
                return;
            }

            _equippedIndex = index;
            WeaponData weapon = slots[index];

            if (weapon != null)
            {
                _meleeAttacker.SetStats(weapon.range, WeaponAttackRadius, weapon.attackCooldown);
            }
            else
            {
                _meleeAttacker.ResetToBareHandStats();
            }
        }

        public string GetDebugSummary()
        {
            string equipped = GetEquipped() != null ? GetEquipped().weaponName : "Bare Hands";
            return $"Weapon Slots: [1]{SlotName(0)} [2]{SlotName(1)} [3]{SlotName(2)} -- Equipped: {equipped}";
        }

        private string SlotName(int index)
        {
            return slots[index] != null ? slots[index].weaponName : "empty";
        }
    }
}
