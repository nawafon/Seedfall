using UnityEngine;
using Seedfall.Plants;

namespace Seedfall.Player
{
    // Lets the player press a key near a plot to harvest a matured plot (priority) or
    // plant the first available seed from their inventory into an empty one.
    [RequireComponent(typeof(PlayerSeedInventory))]
    public class PlantingInteract : MonoBehaviour
    {
        [SerializeField] private float interactRange = 2.5f;
        [SerializeField] private KeyCode interactKey = KeyCode.E;

        private PlayerSeedInventory _inventory;

        private void Awake()
        {
            _inventory = GetComponent<PlayerSeedInventory>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(interactKey))
            {
                TryPlantNearby();
            }
        }

        private void TryPlantNearby()
        {
            Collider[] nearby = Physics.OverlapSphere(transform.position, interactRange);

            // Harvest takes priority over planting in the same key-press.
            foreach (Collider col in nearby)
            {
                PlantPlot plot = col.GetComponent<PlantPlot>();
                if (plot != null && plot.IsOccupied && plot.HasMatured)
                {
                    plot.TryHarvest(_inventory);
                    return;
                }
            }

            if (_inventory.Seeds.Count == 0)
            {
                return;
            }

            SeedData seedToPlant = _inventory.Seeds[0];

            foreach (Collider col in nearby)
            {
                PlantPlot plot = col.GetComponent<PlantPlot>();
                if (plot != null && !plot.IsOccupied)
                {
                    plot.TryPlant(seedToPlant, _inventory);
                    return;
                }
            }
        }
    }
}
