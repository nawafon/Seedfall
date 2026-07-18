using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Seedfall.Plants;
using Seedfall.Player;

namespace Seedfall.Weapons
{
    public class GraftMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject menuRoot;
        [SerializeField] private TMP_Dropdown coreADropdown;
        [SerializeField] private TMP_Dropdown coreBDropdown;
        [SerializeField] private Button graftButton;
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private GraftingSystem graftingSystem;
        [SerializeField] private PlayerSeedInventory playerInventory;
        [SerializeField] private Transform spawnPoint;

        // Not in the original field list -- needed so the "placeholderPrefab wasn't set"
        // fallback has something concrete to instantiate, without relying on a Resources/
        // folder (which would break the established project folder convention).
        [SerializeField] private GameObject fallbackPlaceholderPrefab;

        private static readonly SeedCoreType[] DropdownOrder =
        {
            SeedCoreType.Growth,
            SeedCoreType.Heat,
            SeedCoreType.Wind
        };

        private void Start()
        {
            List<string> options = new List<string> { "Growth", "Heat", "Wind" };
            coreADropdown.ClearOptions();
            coreADropdown.AddOptions(options);
            coreBDropdown.ClearOptions();
            coreBDropdown.AddOptions(options);

            menuRoot.SetActive(false);

            graftButton.onClick.AddListener(OnGraftClicked);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                bool wasActive = menuRoot.activeSelf;

                // If some OTHER menu is open (cursor already unlocked, but not by us),
                // don't also open this one on top of it -- only one menu at a time.
                if (!wasActive && Cursor.lockState != CursorLockMode.Locked)
                {
                    return;
                }

                bool isOpening = !wasActive;
                menuRoot.SetActive(isOpening);

                // MouseOrbitCamera locks/hides the cursor for mouse-look by default -- the
                // menu is unusable without releasing it while open.
                Cursor.lockState = isOpening ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = isOpening;
            }
        }

        private void OnGraftClicked()
        {
            SeedCoreType coreA = DropdownOrder[coreADropdown.value];
            SeedCoreType coreB = DropdownOrder[coreBDropdown.value];

            if (!playerInventory.HasCoreOfType(coreA) || !playerInventory.HasCoreOfType(coreB))
            {
                resultText.text = "You don't have that combination.";
                return;
            }

            WeaponData result = graftingSystem.TryGraft(coreA, coreB);
            if (result == null)
            {
                resultText.text = $"No recipe for {coreA} + {coreB}.";
                return;
            }

            playerInventory.RemoveCoreOfType(coreA);
            playerInventory.RemoveCoreOfType(coreB);

            GameObject prefabToSpawn = result.placeholderPrefab;
            if (prefabToSpawn == null)
            {
                Debug.LogWarning($"WeaponData '{result.weaponName}' has no placeholderPrefab assigned -- falling back to the shared placeholder.");
                prefabToSpawn = fallbackPlaceholderPrefab;
            }

            if (prefabToSpawn != null)
            {
                Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
            }
            else
            {
                Debug.LogWarning("No placeholder prefab available to spawn (neither WeaponData.placeholderPrefab nor fallbackPlaceholderPrefab is set).");
            }

            resultText.text = $"Crafted: {result.weaponName}!";
        }
    }
}
