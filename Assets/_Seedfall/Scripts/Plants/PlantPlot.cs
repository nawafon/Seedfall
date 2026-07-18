using UnityEngine;
using Seedfall.Player;

namespace Seedfall.Plants
{
    // A single plantable plot/slot in the world. Handles planting and the visual
    // growth-over-time placeholder only -- no weapon/harvest logic yet (that's Step 4).
    [RequireComponent(typeof(Collider))]
    public class PlantPlot : MonoBehaviour
    {
        [SerializeField] private float growTimeSeconds = 15f;
        [SerializeField] private float startScale = 0.15f;
        [SerializeField] private float matureScale = 1f;

        private bool _isOccupied;
        private SeedCoreData _plantedCore;
        private Transform _growingObject;
        private float _growTimer;
        private bool _hasMatured;

        public bool IsOccupied => _isOccupied;
        public SeedCoreData PlantedCore => _plantedCore;

        private void Reset()
        {
            // Runs once when this component is first added -- the plot's own collider is
            // only used so PlantingInteract can find it via OverlapSphere, not for physical
            // blocking, so make sure it's a trigger.
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        public bool TryPlant(SeedCoreData core, PlayerSeedInventory inventory)
        {
            if (_isOccupied || core == null || inventory == null)
            {
                return false;
            }

            if (!inventory.RemoveSeedCore(core))
            {
                return false; // core wasn't actually in the inventory
            }

            _isOccupied = true;
            _plantedCore = core;
            _growTimer = 0f;
            _hasMatured = false;
            SpawnPlaceholder(core);
            return true;
        }

        private void SpawnPlaceholder(SeedCoreData core)
        {
            GameObject placeholder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            placeholder.name = "Growth_" + core.CoreName;
            placeholder.transform.SetParent(transform, false);
            placeholder.transform.localPosition = Vector3.zero;
            placeholder.transform.localScale = Vector3.one * startScale;

            // The primitive's own collider would physically block the player -- remove it,
            // the plot's own collider (set up in Reset) is what handles interaction range.
            Collider placeholderCollider = placeholder.GetComponent<Collider>();
            if (placeholderCollider != null)
            {
                Destroy(placeholderCollider);
            }

            Renderer renderer = placeholder.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = renderer.material;
                Color color = GetColorForCoreType(core.CoreType);
                // URP's Lit shader exposes "_BaseColor", not the legacy "_Color" that
                // Material.color assumes -- check both so this works regardless of which
                // shader the default material ends up using.
                if (mat.HasProperty("_BaseColor"))
                {
                    mat.SetColor("_BaseColor", color);
                }
                else if (mat.HasProperty("_Color"))
                {
                    mat.SetColor("_Color", color);
                }
            }

            _growingObject = placeholder.transform;
        }

        private static Color GetColorForCoreType(SeedCoreType type)
        {
            switch (type)
            {
                case SeedCoreType.Growth:
                    return Color.green;
                case SeedCoreType.Heat:
                    return new Color(1f, 0.4f, 0f);
                case SeedCoreType.Wind:
                    return Color.cyan;
                default:
                    return Color.white;
            }
        }

        private void Update()
        {
            if (!_isOccupied || _hasMatured || _growingObject == null)
            {
                return;
            }

            _growTimer += Time.deltaTime;
            float t = Mathf.Clamp01(_growTimer / growTimeSeconds);
            _growingObject.localScale = Vector3.one * Mathf.Lerp(startScale, matureScale, t);

            if (t >= 1f)
            {
                _hasMatured = true;
                Debug.Log($"Plot matured: {_plantedCore.CoreName}");
            }
        }
    }
}
