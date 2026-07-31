using System;
using UnityEngine;

namespace Seedfall.Player
{
    // Player HP. Enemy contact damage flows through here; death resets HP and teleports
    // home via the same CharacterController disable/move/enable pattern ExpeditionPortal
    // uses (CharacterController fights a direct transform write otherwise). Never touches
    // inventory or weapons -- death is not a loss condition at MVP, just a reset back to
    // the plot area.
    [RequireComponent(typeof(CharacterController))]
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private Transform respawnPoint;

        [Header("Hurt feedback")]
        [SerializeField] private MouseOrbitCamera orbitCamera;
        [SerializeField] private float hurtShakeDuration = 0.15f;
        [SerializeField] private float hurtShakeMagnitude = 0.12f;

        private CharacterController _controller;
        private PlayerController _playerController;
        private float _currentHealth;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => maxHealth;

        // Fired whenever CurrentHealth changes (damage or respawn reset) -- PlayerHUD
        // subscribes to drive the HP bar instead of polling every frame.
        public event Action<float, float> OnHealthChanged;

        // Fired only on actual damage (not the respawn reset) -- PlayerHUD subscribes to
        // trigger the red screen-edge flash.
        public event Action<float> OnDamaged;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _playerController = GetComponent<PlayerController>();
            _currentHealth = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            _currentHealth -= amount;
            Debug.Log($"Player took {amount} damage -- {_currentHealth}/{maxHealth} HP");
            OnHealthChanged?.Invoke(_currentHealth, maxHealth);
            OnDamaged?.Invoke(amount);

            if (orbitCamera != null)
            {
                orbitCamera.Shake(hurtShakeDuration, hurtShakeMagnitude);
            }

            if (_currentHealth <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log("Player died -- respawning at plot area");
            _currentHealth = maxHealth;
            OnHealthChanged?.Invoke(_currentHealth, maxHealth);

            if (respawnPoint == null)
            {
                return;
            }

            _controller.enabled = false;
            transform.position = respawnPoint.position;
            transform.rotation = respawnPoint.rotation;
            _controller.enabled = true;

            // Without this, residual downward fall speed from before death carries into
            // the new position and can tunnel through thin ground geometry on the very
            // next physics step.
            if (_playerController != null)
            {
                _playerController.ResetVerticalVelocity();
            }
        }
    }
}
