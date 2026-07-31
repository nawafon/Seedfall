using System.Collections;
using UnityEngine;
using Seedfall.Plants;
using Seedfall.Player;

namespace Seedfall.Enemies
{
    public enum EnemyState
    {
        Active,
        Downed
    }

    // Enemy HP + the downed-before-death clamp. A landed hit that would otherwise kill an
    // Active enemy outright instead clamps its HP to the heal threshold and downs it, so
    // every weapon (including a one-hit-kill-capable swing) produces a downed window
    // before death -- see TakeDamage. Killing a downed enemy yields nothing; healing (H
    // key, via PlayerHealAction) is the only seed source, which is the intended
    // "heal for resources" incentive.
    [RequireComponent(typeof(Renderer))]
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 30f;
        [Range(0f, 1f)]
        [SerializeField] private float healableThreshold = 0.34f;
        [Range(0f, 1f)]
        [SerializeField] private float healSeedDropChance = 0.5f;
        [SerializeField] private SeedData seedDrop;
        [SerializeField] private Color corruptedColor = new Color(0.7f, 0f, 0.9f);
        [SerializeField] private Color downedColor = new Color(0.2f, 0.9f, 0.3f);
        [SerializeField] private EnemyController controller;
        [SerializeField] private Renderer targetRenderer;

        [Header("Knockback")]
        [SerializeField] private float knockbackDistance = 0.6f;
        [SerializeField] private float knockbackDuration = 0.1f;

        [Header("Death/heal pop")]
        [SerializeField] private float deathPopDuration = 0.12f;
        [SerializeField] private Color killFlashColor = Color.white;
        [SerializeField] private Color healFlashColor = new Color(1f, 0.95f, 0.4f); // warm gold -- distinct from the kill flash

        private float _currentHealth;
        private EnemyState _state;
        private Collider _collider;
        private Coroutine _knockbackCoroutine;
        private bool _isDying;

        public float CurrentHealth => _currentHealth;
        public EnemyState State => _state;

        // Lets EnemySpawnPoint configure a freshly-instantiated enemy's drop type without
        // needing Editor-only SerializedObject access -- safe to call any time before
        // TryHeal ever reads seedDrop (i.e. any time before this instance goes Downed).
        public void SetSeedDrop(SeedData drop)
        {
            seedDrop = drop;
        }

        private void Awake()
        {
            _currentHealth = maxHealth;
            _state = EnemyState.Active;

            if (controller == null)
            {
                controller = GetComponent<EnemyController>();
            }
            if (targetRenderer == null)
            {
                targetRenderer = GetComponent<Renderer>();
            }
            _collider = GetComponent<Collider>();
        }

        private void Start()
        {
            TintRenderer(corruptedColor);
        }

        public void TakeDamage(float amount)
        {
            if (_isDying)
            {
                return; // already mid death/heal pop -- ignore further hits
            }

            if (_state == EnemyState.Downed)
            {
                ApplyKnockback();
                Kill();
                return;
            }

            ApplyKnockback();

            float thresholdHealth = Mathf.Max(1f, maxHealth * healableThreshold);
            float wouldBeHealth = _currentHealth - amount;

            if (wouldBeHealth <= thresholdHealth)
            {
                _currentHealth = thresholdHealth;
                EnterDowned();
            }
            else
            {
                _currentHealth = wouldBeHealth;
            }
        }

        // Shoves the enemy a short distance directly away from the player, eased over
        // knockbackDuration. Kinematic -- moves the transform directly rather than going
        // through physics forces. EnemyController pauses its own chase write for the same
        // window (SetKnockedBack) so the two don't fight over transform.position.
        private void ApplyKnockback()
        {
            Transform playerTransform = controller != null ? controller.PlayerTransform : null;
            if (playerTransform == null)
            {
                return;
            }

            Vector3 away = transform.position - playerTransform.position;
            away.y = 0f;
            if (away.sqrMagnitude < 0.0001f)
            {
                away = -transform.forward;
            }
            away.Normalize();

            if (_knockbackCoroutine != null)
            {
                StopCoroutine(_knockbackCoroutine);
            }
            _knockbackCoroutine = StartCoroutine(KnockbackRoutine(away));
        }

        private IEnumerator KnockbackRoutine(Vector3 direction)
        {
            if (controller != null)
            {
                controller.SetKnockedBack(true);
            }

            Vector3 start = transform.position;
            Vector3 end = start + direction * knockbackDistance;
            float elapsed = 0f;

            while (elapsed < knockbackDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / knockbackDuration);
                float eased = 1f - (1f - t) * (1f - t); // ease-out -- fast start, settles at the end
                transform.position = Vector3.Lerp(start, end, eased);
                yield return null;
            }

            transform.position = end;
            _knockbackCoroutine = null;

            if (controller != null)
            {
                controller.SetKnockedBack(false);
            }
        }

        private void EnterDowned()
        {
            _state = EnemyState.Downed;
            if (controller != null)
            {
                controller.Stop();
            }
            TintRenderer(downedColor);
            Debug.Log($"{name} downed -- H to heal or hit to kill");
        }

        private void Kill()
        {
            Debug.Log($"{name} killed -- no reward");
            _isDying = true;
            StartCoroutine(DeathSequence(killFlashColor));
        }

        // Only meaningful on a Downed enemy. Rolls healSeedDropChance for a seed drop, then
        // removes the enemy either way -- healing always ends the encounter.
        public bool TryHeal(PlayerSeedInventory inventory)
        {
            if (_state != EnemyState.Downed || _isDying)
            {
                return false;
            }

            bool dropped = Random.value < healSeedDropChance;
            if (dropped && seedDrop != null && inventory != null)
            {
                inventory.AddSeed(seedDrop);
                Debug.Log($"Healed {name} -- seed dropped: {seedDrop.DisplayName}");
            }
            else
            {
                Debug.Log($"Healed {name} -- no seed this time");
            }

            _isDying = true;
            StartCoroutine(DeathSequence(healFlashColor));
            return true;
        }

        // Shared kill/heal exit: a bright flash (color distinguishes which happened) while
        // shrinking to nothing over deathPopDuration with an ease-in (slow start, fast
        // finish -- reads as a rapid collapse), then destroyed. Replaces an instant Destroy
        // so both outcomes get a moment of readable feedback instead of just vanishing.
        private IEnumerator DeathSequence(Color flashColor)
        {
            if (_collider != null)
            {
                _collider.enabled = false; // stop registering further hits/heals immediately
            }

            TintRenderer(flashColor);

            Vector3 startScale = transform.localScale;
            float elapsed = 0f;

            while (elapsed < deathPopDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / deathPopDuration);
                float eased = t * t; // ease-in
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, eased);
                yield return null;
            }

            Destroy(gameObject);
        }

        private void TintRenderer(Color color)
        {
            if (targetRenderer == null)
            {
                return;
            }
            Material mat = targetRenderer.material;
            if (mat.HasProperty("_BaseColor"))
            {
                mat.SetColor("_BaseColor", color);
            }
            else if (mat.HasProperty("_Color"))
            {
                mat.SetColor("_Color", color);
            }
        }
    }
}
