using System;
using UnityEngine;

namespace Seedfall.World
{
    // Single source of truth for "is the player currently on an expedition" (i.e. in the
    // arena, not the plot area). ExpeditionPortal calls SetOnExpedition after each
    // teleport; farm/craft entry points (GraftMenuUI, BreakSeedMenuUI, PlayerInteract)
    // read IsOnExpedition to gate home-only actions. Weapon wilt stays hit-only and does
    // NOT read this -- expedition state never triggers a wilt.
    public class ExpeditionManager : MonoBehaviour
    {
        public static ExpeditionManager Instance { get; private set; }

        private bool _isOnExpedition;

        public bool IsOnExpedition => _isOnExpedition;

        // PlayerHUD subscribes to show/hide the "ON EXPEDITION" label instead of polling
        // IsOnExpedition every frame.
        public event Action<bool> OnExpeditionStateChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Duplicate ExpeditionManager in scene -- destroying the extra one.");
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void SetOnExpedition(bool value)
        {
            _isOnExpedition = value;
            Debug.Log(value
                ? "Expedition started -- farming/crafting locked until you return."
                : "Back home -- farming/crafting available.");
            OnExpeditionStateChanged?.Invoke(value);
        }
    }
}
