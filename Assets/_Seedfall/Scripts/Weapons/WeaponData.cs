using UnityEngine;

namespace Seedfall.Weapons
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Seedfall/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        public string weaponName;
        public float damage;
        public float range;
        public float attackCooldown;
        public WeaponGimmick gimmick;
        public GameObject placeholderPrefab;
    }
}
