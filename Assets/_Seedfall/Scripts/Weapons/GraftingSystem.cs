using System.Collections.Generic;
using UnityEngine;
using Seedfall.Plants;

namespace Seedfall.Weapons
{
    public class GraftingSystem : MonoBehaviour
    {
        public List<GraftRecipe> recipes;

        public WeaponData TryGraft(SeedCoreType a, SeedCoreType b)
        {
            foreach (GraftRecipe recipe in recipes)
            {
                if (recipe.Matches(a, b))
                {
                    return recipe.result;
                }
            }

            Debug.Log($"No recipe found for {a} + {b}");
            return null;
        }
    }
}
