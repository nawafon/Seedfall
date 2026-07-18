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

        public bool RemoveSeedCore(SeedCoreData seedCore)
        {
            return seedCores.Remove(seedCore);
        }

        public bool HasCoreOfType(SeedCoreType type)
        {
            foreach (SeedCoreData core in seedCores)
            {
                if (core != null && core.CoreType == type)
                {
                    return true;
                }
            }
            return false;
        }

        public bool RemoveCoreOfType(SeedCoreType type)
        {
            for (int i = 0; i < seedCores.Count; i++)
            {
                if (seedCores[i] != null && seedCores[i].CoreType == type)
                {
                    seedCores.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        [SerializeField] private List<SeedData> seeds = new List<SeedData>();

        public IReadOnlyList<SeedData> Seeds => seeds;

        public void AddSeed(SeedData seed)
        {
            seeds.Add(seed);
        }

        public bool RemoveSeed(SeedData seed)
        {
            return seeds.Remove(seed);
        }

        public bool HasSeedOfType(SeedCoreType type)
        {
            foreach (SeedData seed in seeds)
            {
                if (seed != null && seed.SeedType == type)
                {
                    return true;
                }
            }
            return false;
        }

        public bool RemoveSeedOfType(SeedCoreType type)
        {
            for (int i = 0; i < seeds.Count; i++)
            {
                if (seeds[i] != null && seeds[i].SeedType == type)
                {
                    seeds.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        [SerializeField] private Dictionary<SeedCoreType, int> sapAmounts = new Dictionary<SeedCoreType, int>();

        public void AddSap(SeedCoreType type, int amount)
        {
            if (sapAmounts.TryGetValue(type, out int current))
            {
                sapAmounts[type] = current + amount;
            }
            else
            {
                sapAmounts[type] = amount;
            }
        }

        public int GetSapCount(SeedCoreType type)
        {
            if (sapAmounts.TryGetValue(type, out int current))
            {
                return current;
            }
            return 0;
        }

        public string GetDebugSummary()
        {
            SeedCoreType[] types = { SeedCoreType.Growth, SeedCoreType.Heat, SeedCoreType.Wind };
            string summary = "";
            foreach (SeedCoreType type in types)
            {
                int seedCount = 0;
                foreach (SeedData seed in seeds)
                {
                    if (seed != null && seed.SeedType == type)
                    {
                        seedCount++;
                    }
                }

                int coreCount = 0;
                foreach (SeedCoreData core in seedCores)
                {
                    if (core != null && core.CoreType == type)
                    {
                        coreCount++;
                    }
                }

                int sapCount = GetSapCount(type);

                summary += $"{type}: seeds={seedCount}, cores={coreCount}, sap={sapCount}\n";
            }

            summary += $"rockUses={rockUses}\n";

            return summary;
        }

        [SerializeField] private int rockUses = 0;

        public void AddRockUses(int amount)
        {
            rockUses += amount;
        }

        public int GetRockUses()
        {
            return rockUses;
        }

        public bool UseRock()
        {
            if (rockUses <= 0)
            {
                return false;
            }
            rockUses--;
            return true;
        }
    }
}
