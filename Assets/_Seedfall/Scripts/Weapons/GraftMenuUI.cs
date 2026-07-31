using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Seedfall.Plants;
using Seedfall.Player;
using Seedfall.World;
using Seedfall.Tools;

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
        [SerializeField] private WeaponInventory weaponInventory;
        [SerializeField] private Transform spawnPoint;

        // Cross-reference used ONLY to check the other menu's real open state -- NOT
        // Cursor.lockState, which turned out to be an unreliable proxy: the OS auto-
        // unlocks the cursor whenever the Editor window loses focus (e.g. alt-tabbing),
        // which was permanently blocking both menus from ever reopening even though
        // neither was actually open.
        [SerializeField] private BreakSeedMenuUI breakSeedMenu;

        public bool IsOpen => menuRoot.activeSelf;

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
                bool isOpening = !wasActive;

                // If the OTHER menu is genuinely open, don't also open this one on top of
                // it -- checked directly rather than via Cursor.lockState (see field comment).
                if (isOpening && breakSeedMenu != null && breakSeedMenu.IsOpen)
                {
                    return;
                }

                // Grafting is home-only -- can't prep/craft mid-expedition.
                if (isOpening && ExpeditionManager.Instance != null && ExpeditionManager.Instance.IsOnExpedition)
                {
                    Debug.Log("Can't graft while on an expedition -- head back home first.");
                    return;
                }

                menuRoot.SetActive(isOpening);

                // MouseOrbitCamera locks/hides the cursor for mouse-look by default -- the
                // menu is unusable without releasing it while open.
                Cursor.lockState = isOpening ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = isOpening;
            }
        }

        // Called by Unity whenever the game window's OS focus changes. The cursor lock
        // gets auto-released by the OS whenever the window loses focus (e.g. alt-tabbing
        // out to read something) -- without this, regaining focus could leave the cursor
        // unlocked indefinitely with no menu actually open, freezing player movement/
        // camera and blocking both menus from reopening until the player happened to
        // press Escape at exactly the right moment.
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                return;
            }

            bool anyMenuOpen = menuRoot.activeSelf || (breakSeedMenu != null && breakSeedMenu.IsOpen);
            if (!anyMenuOpen && Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
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

            // A grafted weapon goes straight into inventory if a slot is free. Only if
            // all 3 slots are full does it fall to the ground as a WeaponPickup instead.
            if (!weaponInventory.TryAddWeapon(result))
            {
                GameObject prefabToSpawn = result.placeholderPrefab;
                if (prefabToSpawn == null)
                {
                    Debug.LogWarning($"WeaponData '{result.weaponName}' has no placeholderPrefab assigned -- falling back to the shared placeholder.");
                    prefabToSpawn = fallbackPlaceholderPrefab;
                }

                if (prefabToSpawn != null)
                {
                    GameObject spawned = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
                    WeaponPickup pickup = spawned.GetComponent<WeaponPickup>();
                    if (pickup != null)
                    {
                        pickup.weapon = result;
                    }
                    else
                    {
                        Debug.LogWarning($"Spawned placeholder for '{result.weaponName}' has no WeaponPickup component -- it can't be picked up.");
                    }
                }
                else
                {
                    Debug.LogWarning("No placeholder prefab available to spawn (neither WeaponData.placeholderPrefab nor fallbackPlaceholderPrefab is set).");
                }
            }

            resultText.text = $"Crafted: {result.weaponName}!";
        }
    }
}
