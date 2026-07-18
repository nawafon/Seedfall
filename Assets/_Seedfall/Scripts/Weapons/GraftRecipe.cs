using UnityEngine;
using Seedfall.Plants;

namespace Seedfall.Weapons
{
    [CreateAssetMenu(fileName = "GraftRecipe", menuName = "Seedfall/GraftRecipe")]
    public class GraftRecipe : ScriptableObject
    {
        public SeedCoreType coreA;
        public SeedCoreType coreB;
        public WeaponData result;

        public bool Matches(SeedCoreType a, SeedCoreType b)
        {
            return (coreA == a && coreB == b) || (coreA == b && coreB == a);
        }
    }
}
