using UnityEngine;
using Seedfall.Plants;
using Seedfall.Weapons;

namespace Seedfall.Player
{
    // Single E-key listener for all walk-near-and-press-E interactables (plots, dropped
    // weapon pickups). Whichever candidate is physically closest to the player fires;
    // the other is ignored for that keypress. Plot execution (harvest-before-plant,
    // which specific plot) stays entirely inside PlantingInteract -- this script only
    // decides whether plots or weapon pickups get first say.
    [RequireComponent(typeof(PlantingInteract))]
    [RequireComponent(typeof(PlayerSeedInventory))]
    [RequireComponent(typeof(WeaponInventory))]
    public class PlayerInteract : MonoBehaviour
    {
        [SerializeField] private float interactRange = 2.5f;
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private KeyCode sapHarvestKey = KeyCode.F;

        private PlantingInteract _plantingInteract;
        private PlayerSeedInventory _inventory;
        private WeaponInventory _weaponInventory;

        private void Awake()
        {
            _plantingInteract = GetComponent<PlantingInteract>();
            _inventory = GetComponent<PlayerSeedInventory>();
            _weaponInventory = GetComponent<WeaponInventory>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(interactKey))
            {
                TryInteractWithClosest();
            }

            if (Input.GetKeyDown(sapHarvestKey))
            {
                _plantingInteract.TryHarvestNearbyPlotForSap();
            }
        }

        // Single OverlapSphere scanned for both candidate types.
        private void TryInteractWithClosest()
        {
            Collider[] nearby = Physics.OverlapSphere(transform.position, interactRange);

            bool hasPlotCandidate = false;
            float plotDistance = float.MaxValue;

            WeaponPickup nearestPickup = null;
            float pickupDistance = float.MaxValue;

            foreach (Collider col in nearby)
            {
                PlantPlot plot = col.GetComponent<PlantPlot>();
                if (plot != null)
                {
                    bool harvestable = plot.IsOccupied && plot.HasMatured;
                    bool plantable = !plot.IsOccupied && _inventory.Seeds.Count > 0;
                    if (harvestable || plantable)
                    {
                        float distance = Vector3.Distance(transform.position, col.transform.position);
                        if (distance < plotDistance)
                        {
                            plotDistance = distance;
                            hasPlotCandidate = true;
                        }
                    }
                    continue;
                }

                WeaponPickup pickup = col.GetComponent<WeaponPickup>();
                if (pickup != null)
                {
                    float distance = Vector3.Distance(transform.position, col.transform.position);
                    if (distance < pickupDistance)
                    {
                        pickupDistance = distance;
                        nearestPickup = pickup;
                    }
                }
            }

            bool hasPickupCandidate = nearestPickup != null;
            if (!hasPlotCandidate && !hasPickupCandidate)
            {
                return;
            }

            // Ties favor the plot -- arbitrary but deterministic.
            bool plotWins = hasPlotCandidate && (!hasPickupCandidate || plotDistance <= pickupDistance);

            if (plotWins)
            {
                _plantingInteract.TryInteractWithNearbyPlot();
            }
            else
            {
                nearestPickup.TryPickUp(_weaponInventory);
            }
        }
    }
}
