using UnityEngine;

namespace Seedfall.Plants
{
    // What the player finds/picks up in the world -- plantable. Graft-ready SeedCoreData
    // is now a separate item, only obtained by breaking a SeedData open (Cracking Stone, Part 2).
    [CreateAssetMenu(fileName = "SeedData", menuName = "Seedfall/SeedData")]
    public class SeedData : ScriptableObject
    {
        [SerializeField] private SeedCoreType seedType;
        [SerializeField] private string displayName;

        public SeedCoreType SeedType => seedType;
        public string DisplayName => displayName;
    }
}
