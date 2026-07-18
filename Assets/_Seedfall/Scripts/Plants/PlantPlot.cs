using UnityEngine;
using System.Collections;
using Seedfall.Player;

namespace Seedfall.Plants
{
    // A single plantable plot/slot in the world. Handles planting and the visual
    // growth-over-time progress only -- no weapon/harvest logic yet (that's Step 4).
    // Growth is plain progress data (0..1) driving a swap between two pre-placed
    // placeholder children (Stage_Small / Stage_Grown) via SetActive -- not a scaled
    // object -- so real small/grown models can later be swapped in on those same
    // child objects without touching this script.
    [RequireComponent(typeof(Collider))]
    public class PlantPlot : MonoBehaviour
    {
        [SerializeField] private float growTimeSeconds = 15f;
        [SerializeField] private GameObject stageSmall;
        [SerializeField] private GameObject stageGrown;
        [Range(0f, 1f)]
        [SerializeField] private float stageSwitchThreshold = 0.5f;

        private bool _isOccupied;
        private SeedCoreData _plantedCore;
        private float _growProgress; // 0..1, plain data -- not tied to any transform
        private bool _hasMatured;
        private bool _hasSwitchedStage;

        public bool IsOccupied => _isOccupied;
        public SeedCoreData PlantedCore => _plantedCore;
        public float GrowProgress => _growProgress;

        private void Reset()
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        private void Awake()
        {
            // Empty plot at start: neither growth stage should be visible until planted.
            SetActiveStages(smallActive: false, grownActive: false);
        }

        public bool TryPlant(SeedCoreData core, PlayerSeedInventory inventory)
        {
            if (_isOccupied || core == null || inventory == null)
            {
                return false;
            }

            if (!inventory.RemoveSeedCore(core))
            {
                return false;
            }

            _isOccupied = true;
            _plantedCore = core;
            _growProgress = 0f;
            _hasMatured = false;
            _hasSwitchedStage = false;

            TintStages(core);
            SetActiveStages(smallActive: true, grownActive: false);
            StartCoroutine(GrowRoutine());
            return true;
        }

        private IEnumerator GrowRoutine()
        {
            while (_growProgress < 1f)
            {
                _growProgress = Mathf.Clamp01(_growProgress + Time.deltaTime / growTimeSeconds);

                if (!_hasSwitchedStage && _growProgress >= stageSwitchThreshold)
                {
                    SetActiveStages(smallActive: false, grownActive: true);
                    _hasSwitchedStage = true;
                }

                yield return null;
            }

            _hasMatured = true;
            Debug.Log($"Plot matured: {_plantedCore.CoreName}");
        }

        private void SetActiveStages(bool smallActive, bool grownActive)
        {
            if (stageSmall != null)
            {
                stageSmall.SetActive(smallActive);
            }
            if (stageGrown != null)
            {
                stageGrown.SetActive(grownActive);
            }
        }

        private void TintStages(SeedCoreData core)
        {
            Color color = GetColorForCoreType(core.CoreType);
            TintRenderer(stageSmall, color);
            TintRenderer(stageGrown, color);
        }

        private static void TintRenderer(GameObject go, Color color)
        {
            if (go == null)
            {
                return;
            }
            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer == null)
            {
                return;
            }
            Material mat = renderer.material;
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
    }
}
