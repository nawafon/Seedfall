using UnityEngine;
using Seedfall.Player;

namespace Seedfall.Enemies
{
    // Chases the player on the XZ plane and deals contact damage on touch. Finds the
    // player once via FindFirstObjectByType in Awake -- no per-instance scene wiring
    // needed, since there's exactly one PlayerHealth in the scene. Stops chasing/damaging
    // entirely once EnemyHealth reports Downed.
    [RequireComponent(typeof(EnemyHealth))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float detectionRadius = 12f;
        [SerializeField] private float touchRange = 1.3f;
        [SerializeField] private float contactDamage = 10f;
        [SerializeField] private float contactDamageCooldown = 1.0f;

        private EnemyHealth _health;
        private Transform _playerTransform;
        private PlayerHealth _playerHealth;
        private float _lastContactTime = -999f;
        private bool _isKnockedBack;

        // Read by EnemyHealth to compute a knockback direction away from the player --
        // avoids EnemyHealth needing its own FindFirstObjectByType lookup.
        public Transform PlayerTransform => _playerTransform;

        private void Awake()
        {
            _health = GetComponent<EnemyHealth>();

            PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
            if (player != null)
            {
                _playerHealth = player;
                _playerTransform = player.transform;
            }
        }

        // Called by EnemyHealth on entering Downed. Update() already gates on
        // EnemyHealth.State != Active, so this is just an explicit hook -- kept separate
        // from EnemyHealth so it doesn't need to know how movement is implemented.
        public void Stop()
        {
        }

        // EnemyHealth's knockback coroutine moves transform.position directly over a short
        // window -- this pauses the normal chase write so the two don't fight over the
        // same frame (only reachable while still Active; a hit that downs the enemy
        // already stops movement via the State check below on its own).
        public void SetKnockedBack(bool value)
        {
            _isKnockedBack = value;
        }

        private void Update()
        {
            if (_health.State != EnemyState.Active || _playerTransform == null || _isKnockedBack)
            {
                return;
            }

            Vector3 toPlayer = _playerTransform.position - transform.position;
            toPlayer.y = 0f;
            float distance = toPlayer.magnitude;

            if (distance > detectionRadius)
            {
                return;
            }

            if (distance > touchRange)
            {
                Vector3 direction = toPlayer.normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
            else if (Time.time >= _lastContactTime + contactDamageCooldown)
            {
                _lastContactTime = Time.time;
                _playerHealth.TakeDamage(contactDamage);
            }
        }
    }
}
