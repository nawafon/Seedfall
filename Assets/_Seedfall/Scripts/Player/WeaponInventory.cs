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

        // EquippedWeapon (not WeaponData) so each graft/pickup gets its own durability
        // counter -- WeaponData is a shared asset, all copies of "Thornblaze" would
        // otherwise share one counter.
        private EquippedWeapon[] slots = new EquippedWeapon[3];
        private int _equippedIndex = -1; // -1 = no slot selected, bare fists

        private BareHandMelee _meleeAttacker;

        public WeaponData GetEquipped() => _equippedIndex >= 0 ? slots[_equippedIndex]?.Data : null;
        public int EquippedIndex => _equippedIndex;

        private void Awake()
        {
            _meleeAttacker = GetComponent<BareHandMelee>();
            _meleeAttacker.OnHitLanded += HandleHitLanded;
        }

        private void OnDestroy()
        {
            _meleeAttacker.OnHitLanded -= HandleHitLanded;
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
                    slots[i] = new EquippedWeapon(weapon);
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
            EquippedWeapon weapon = slots[index];

            if (weapon != null)
            {
                _meleeAttacker.SetStats(weapon.Data.range, WeaponAttackRadius, weapon.Data.attackCooldown);
            }
            else
            {
                _meleeAttacker.ResetToBareHandStats();
            }
        }

        // A landed hit only decrements durability when a real weapon is equipped --
        // bare fists have no durability. At 0 remaining hits the weapon wilts: removed
        // from its slot entirely (no seed drop -- that comes from enemies in Step 7),
        // reverting to bare fists if it was the equipped slot.
        private void HandleHitLanded()
        {
            if (_equippedIndex < 0 || slots[_equippedIndex] == null)
            {
                return;
            }

            EquippedWeapon weapon = slots[_equippedIndex];
            bool wilted = weapon.DecrementAndCheckWilt();
            Debug.Log($"{weapon.Data.weaponName}: {weapon.RemainingHits} hits remaining");

            if (wilted)
            {
                Debug.Log($"{weapon.Data.weaponName} wilted.");
                slots[_equippedIndex] = null;
                _meleeAttacker.ResetToBareHandStats();
            }
        }

        public string GetDebugSummary()
        {
            string equipped = _equippedIndex >= 0 && slots[_equippedIndex] != null
                ? $"{slots[_equippedIndex].Data.weaponName} ({slots[_equippedIndex].RemainingHits} left)"
                : "Bare Hands";
            return $"Weapon Slots: [1]{SlotName(0)} [2]{SlotName(1)} [3]{SlotName(2)} -- Equipped: {equipped}";
        }

        private string SlotName(int index)
        {
            return slots[index] != null ? slots[index].Data.weaponName : "empty";
        }
    }
}
