using UnityEngine;

namespace Seedfall.Plants
{
    // Data describing one kind of Seed-Core (Growth/Heat/Wind). Grafting (a later step) will
    // combine two of these into a weapon -- this asset only holds identity data for now.
    [CreateAssetMenu(fileName = "New SeedCore", menuName = "Seedfall/Seed Core")]
    public class SeedCoreData : ScriptableObject
    {
        [SerializeField] private SeedCoreType coreType;
        [SerializeField] private string coreName = "Seed Core";
        [SerializeField] [TextArea] private string description;

        public SeedCoreType CoreType => coreType;
        public string CoreName => coreName;
        public string Description => description;
    }
}
