using UnityEngine;
using Seedfall.Plants;

namespace Seedfall.Player
{
    // Lets the player press a key near an unoccupied PlantPlot to plant the first
    // available SeedCore from their inventory into it.
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
            if (_inventory.SeedCores.Count == 0)
            {
                return;
            }

            SeedCoreData coreToPlant = _inventory.SeedCores[0];

            Collider[] nearby = Physics.OverlapSphere(transform.position, interactRange);
            foreach (Collider col in nearby)
            {
                PlantPlot plot = col.GetComponent<PlantPlot>();
                if (plot != null && !plot.IsOccupied)
                {
                    plot.TryPlant(coreToPlant, _inventory);
                    return;
                }
            }
        }
    }
}
