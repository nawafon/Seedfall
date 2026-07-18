using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Seedfall.Plants;
using Seedfall.Player;

namespace Seedfall.Tools
{
    // Breaks 1 held SeedData into 1 matching SeedCoreData, consuming 1 rock use.
    // Can be opened anywhere -- no proximity to a world object required (supersedes
    // the scrapped CrackingStone/CrackMenuUI design).
    public class BreakSeedMenuUI : MonoBehaviour
    {
        public GameObject menuRoot;
        public TMP_Dropdown seedTypeDropdown;
        public Button breakButton;
        public Button closeButton;
        public TMP_Text resultText;
        public PlayerSeedInventory playerInventory;
        public SeedCoreData coreGrowth;
        public SeedCoreData coreHeat;
        public SeedCoreData coreWind;

        private static readonly SeedCoreType[] DropdownOrder =
        {
            SeedCoreType.Growth,
            SeedCoreType.Heat,
            SeedCoreType.Wind
        };

        public bool IsOpen => menuRoot.activeSelf;

        private void Start()
        {
            List<string> options = new List<string> { "Growth", "Heat", "Wind" };
            seedTypeDropdown.ClearOptions();
            seedTypeDropdown.AddOptions(options);

            menuRoot.SetActive(false);

            breakButton.onClick.AddListener(OnBreakClicked);
            closeButton.onClick.AddListener(Close);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R) && !IsOpen)
            {
                Open();
            }

            if (Input.GetKeyDown(KeyCode.Escape) && IsOpen)
            {
                Close();
            }
        }

        public void Open()
        {
            // If some OTHER menu is open (cursor already unlocked), don't also open this
            // one on top of it -- only one menu at a time.
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                return;
            }

            if (playerInventory.GetRockUses() <= 0)
            {
                Debug.Log("You need a rock to break seeds.");
                return;
            }

            menuRoot.SetActive(true);
            resultText.text = "";

            // Same cursor-unlock fix just applied to the now-deleted CrackMenuUI --
            // MouseOrbitCamera locks/hides the cursor by default.
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Close()
        {
            menuRoot.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnBreakClicked()
        {
            SeedCoreType type = DropdownOrder[seedTypeDropdown.value];

            if (!playerInventory.HasSeedOfType(type))
            {
                resultText.text = "You don't have that seed.";
                return;
            }

            if (!playerInventory.UseRock())
            {
                resultText.text = "Out of rock uses — find another rock.";
                return;
            }

            playerInventory.RemoveSeedOfType(type);

            SeedCoreData matchingCore = GetCoreForType(type);
            playerInventory.AddSeedCore(matchingCore);

            resultText.text = $"Cracked open a {type} Seed → got a {type} Core! ({playerInventory.GetRockUses()} rock uses left)";
        }

        private SeedCoreData GetCoreForType(SeedCoreType type)
        {
            switch (type)
            {
                case SeedCoreType.Growth:
                    return coreGrowth;
                case SeedCoreType.Heat:
                    return coreHeat;
                case SeedCoreType.Wind:
                    return coreWind;
                default:
                    return null;
            }
        }
    }
}
