using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Seedfall.Player;
using Seedfall.Weapons;
using Seedfall.World;

namespace Seedfall.UI
{
    // Minimal always-on HUD: HP bar, equipped-weapon readout, and an expedition-state
    // label. Fully event-driven -- subscribes to PlayerHealth/WeaponInventory/
    // ExpeditionManager events and re-renders only when one fires, no per-frame polling.
    // Initial state is read directly from those sources at Start (rather than waiting for
    // the first event) so the HUD isn't blank/stale before anything changes.
    public class PlayerHUD : MonoBehaviour
    {
        [SerializeField] private Image healthFill;
        [SerializeField] private TMP_Text weaponText;
        [SerializeField] private GameObject expeditionLabel;
        [SerializeField] private Image hurtFlashOverlay;
        [SerializeField] private float hurtFlashPeakAlpha = 0.35f;
        [SerializeField] private float hurtFlashDuration = 0.25f;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private WeaponInventory weaponInventory;

        private WeaponData _currentWeaponData;
        private int _currentRemainingHits;
        private Coroutine _hurtFlashCoroutine;

        private void Start()
        {
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged += HandleHealthChanged;
                playerHealth.OnDamaged += HandleDamaged;
                HandleHealthChanged(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            }

            if (weaponInventory != null)
            {
                weaponInventory.OnEquipChanged += HandleEquipChanged;
                weaponInventory.OnDurabilityChanged += HandleDurabilityChanged;
                HandleEquipChanged(weaponInventory.GetEquipped());
                HandleDurabilityChanged(weaponInventory.GetEquippedRemainingHits());
            }

            if (ExpeditionManager.Instance != null)
            {
                ExpeditionManager.Instance.OnExpeditionStateChanged += HandleExpeditionStateChanged;
                HandleExpeditionStateChanged(ExpeditionManager.Instance.IsOnExpedition);
            }
            else if (expeditionLabel != null)
            {
                expeditionLabel.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged -= HandleHealthChanged;
                playerHealth.OnDamaged -= HandleDamaged;
            }
            if (weaponInventory != null)
            {
                weaponInventory.OnEquipChanged -= HandleEquipChanged;
                weaponInventory.OnDurabilityChanged -= HandleDurabilityChanged;
            }
            if (ExpeditionManager.Instance != null)
            {
                ExpeditionManager.Instance.OnExpeditionStateChanged -= HandleExpeditionStateChanged;
            }
        }

        private void HandleHealthChanged(float current, float max)
        {
            if (healthFill != null)
            {
                healthFill.fillAmount = max > 0f ? Mathf.Clamp01(current / max) : 0f;
            }
        }

        private void HandleEquipChanged(WeaponData data)
        {
            _currentWeaponData = data;
            RefreshWeaponText();
        }

        private void HandleDurabilityChanged(int remainingHits)
        {
            _currentRemainingHits = remainingHits;
            RefreshWeaponText();
        }

        private void RefreshWeaponText()
        {
            if (weaponText == null)
            {
                return;
            }
            weaponText.text = _currentWeaponData != null
                ? $"{_currentWeaponData.weaponName} {_currentRemainingHits}/{_currentWeaponData.maxHits}"
                : "Bare Hands";
        }

        private void HandleExpeditionStateChanged(bool isOnExpedition)
        {
            if (expeditionLabel != null)
            {
                expeditionLabel.SetActive(isOnExpedition);
            }
        }

        private void HandleDamaged(float amount)
        {
            if (hurtFlashOverlay == null)
            {
                return;
            }

            if (_hurtFlashCoroutine != null)
            {
                StopCoroutine(_hurtFlashCoroutine);
            }
            _hurtFlashCoroutine = StartCoroutine(HurtFlashRoutine());
        }

        private IEnumerator HurtFlashRoutine()
        {
            Color baseColor = hurtFlashOverlay.color;
            hurtFlashOverlay.color = new Color(baseColor.r, baseColor.g, baseColor.b, hurtFlashPeakAlpha);
            float elapsed = 0f;

            while (elapsed < hurtFlashDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / hurtFlashDuration);
                float alpha = Mathf.Lerp(hurtFlashPeakAlpha, 0f, t);
                Color current = hurtFlashOverlay.color;
                hurtFlashOverlay.color = new Color(current.r, current.g, current.b, alpha);
                yield return null;
            }

            Color final = hurtFlashOverlay.color;
            hurtFlashOverlay.color = new Color(final.r, final.g, final.b, 0f);
            _hurtFlashCoroutine = null;
        }
    }
}
