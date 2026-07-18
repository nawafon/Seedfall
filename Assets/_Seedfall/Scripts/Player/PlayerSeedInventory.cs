using System.Collections.Generic;
using UnityEngine;
using Seedfall.Plants;

namespace Seedfall.Player
{
    // Minimal holder for SeedCores the player has collected. A later step (planting) will
    // consume from this list. No scarcity design here -- seeds are meant to be easy to find.
    public class PlayerSeedInventory : MonoBehaviour
    {
        [SerializeField] private List<SeedCoreData> seedCores = new List<SeedCoreData>();

        public IReadOnlyList<SeedCoreData> SeedCores => seedCores;

        public void AddSeedCore(SeedCoreData seedCore)
        {
            seedCores.Add(seedCore);
        }
    }
}
