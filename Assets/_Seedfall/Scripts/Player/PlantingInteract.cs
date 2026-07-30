using UnityEngine;
using Seedfall.Plants;

namespace Seedfall.Player
{
    // Plot interaction logic (harvest-priority-over-plant). The E-keypress itself now
    // lives in PlayerInteract, which arbitrates between this and weapon pickups by
    // distance and calls TryInteractWithNearbyPlot() when a plot wins.
    [RequireComponent(typeof(PlayerSeedInventory))]
    public class PlantingInteract : MonoBehaviour
    {
        [SerializeField] private float interactRange = 2.5f;

        private PlayerSeedInventory _inventory;

        private void Awake()
        {
            _inventory = GetComponent<PlayerSeedInventory>();
        }

        public bool TryInteractWithNearbyPlot()
        {
            Collider[] nearby = Physics.OverlapSphere(transform.position, interactRange);

            // Harvest takes priority over planting in the same key-press.
            foreach (Collider col in nearby)
            {
                PlantPlot plot = col.GetComponent<PlantPlot>();
                if (plot != null && plot.IsOccupied && plot.HasMatured)
                {
                    plot.TryHarvestForSeeds(_inventory);
                    return true;
                }
            }

            if (_inventory.Seeds.Count == 0)
            {
                return false;
            }

            SeedData seedToPlant = _inventory.Seeds[0];

            foreach (Collider col in nearby)
            {
                PlantPlot plot = col.GetComponent<PlantPlot>();
                if (plot != null && !plot.IsOccupied)
                {
                    plot.TryPlant(seedToPlant, _inventory);
                    return true;
                }
            }

            return false;
        }

        public bool TryHarvestNearbyPlotForSap()
        {
            Collider[] nearby = Physics.OverlapSphere(transform.position, interactRange);
            foreach (Collider col in nearby)
            {
                PlantPlot plot = col.GetComponent<PlantPlot>();
                if (plot != null && plot.IsOccupied && plot.HasMatured)
                {
                    return plot.TryHarvestForSap(_inventory);
                }
            }
            return false;
        }
    }
}
