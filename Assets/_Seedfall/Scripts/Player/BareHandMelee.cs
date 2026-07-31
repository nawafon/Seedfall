using System;
using System.Collections;
using UnityEngine;
using Seedfall.Enemies;

namespace Seedfall.Player
{
    // The player's melee attack. Bare fists by default (the serialized stats below);
    // WeaponInventory calls SetStats to override with an equipped weapon's numbers, and
    // ResetToBareHandStats to restore these exact defaults when no weapon (or an empty
    // slot) is equipped. Kept as BareHandMelee rather than renamed to something more
    // general -- it's already attached to Player in the scene, and renaming the
    // file/class outside the Editor would regenerate the script's GUID and silently
    // detach the component.
    public class BareHandMelee : MonoBehaviour
    {
        [SerializeField] private float attackRange = 1.0f;    // distance in front of the player the hit check is centered on
        [SerializeField] private float attackRadius = 0.5f;   // radius of the hit-check sphere
        [SerializeField] private float attackCooldown = 0.5f; // seconds required between swings
        [SerializeField] private float attackDamage = 5f;     // damage dealt to EnemyHealth on a landed hit

        [Header("Hit feedback (juice only, no damage system yet)")]
        [SerializeField] private float hitFlashDuration = 0.1f;
        [SerializeField] private Color hitFlashColor = Color.white; // bare-hand default; weapons override via SetStats
        [SerializeField] private float shakeDuration = 0.1f;
        [SerializeField] private float shakeMagnitude = 0.15f;
        [SerializeField] private float shakeDamageReference = 5f; // damage shakeMagnitude above is tuned for (bare-hand damage) -- heavier weapons shake proportionally more
        [SerializeField] private MouseOrbitCamera orbitCamera;

        [Header("Hit-stop (freeze frame)")]
        [SerializeField] private float hitStopMinDuration = 0.06f; // real-time seconds, low damage
        [SerializeField] private float hitStopMaxDuration = 0.14f; // real-time seconds, at/above hitStopHeavyDamageThreshold
        [SerializeField] private float hitStopTimeScale = 0.05f;
        [SerializeField] private float hitStopHeavyDamageThreshold = 20f;

        private float _lastAttackTime = -999f;
        private Coroutine _hitStopCoroutine;

        // Current melee range/radius, read by AttackRangeIndicator to draw a live ground
        // ring and hit-zone marker -- stays accurate across weapon equip/wilt since these
        // read the same fields SetStats and ResetToBareHandStats both write.
        public float AttackRange => attackRange;
        public float AttackRadius => attackRadius;

        // Fired once per swing that actually lands a hit (cooldown passed AND at least
        // one non-self collider was hit) -- never for a swing at empty air, and never
        // more than once even if a swing overlaps multiple colliders. WeaponInventory
        // subscribes to decrement the equipped weapon's durability -- kept as an event
        // rather than WeaponInventory reaching in here so BareHandMelee stays ignorant
        // of weapons/durability entirely.
        public event Action OnHitLanded;

        // Cached at Awake so ResetToBareHandStats always restores the true serialized
        // defaults, even after SetStats has overwritten the active fields above.
        private float _bareHandRange;
        private float _bareHandRadius;
        private float _bareHandCooldown;
        private float _bareHandDamage;
        private Color _bareHandFlashColor;

        private void Awake()
        {
            _bareHandRange = attackRange;
            _bareHandRadius = attackRadius;
            _bareHandCooldown = attackCooldown;
            _bareHandDamage = attackDamage;
            _bareHandFlashColor = hitFlashColor;
        }

        public void SetStats(float range, float radius, float cooldown, float damage, Color flashColor)
        {
            attackRange = range;
            attackRadius = radius;
            attackCooldown = cooldown;
            attackDamage = damage;
            hitFlashColor = flashColor;
        }

        public void ResetToBareHandStats()
        {
            attackRange = _bareHandRange;
            attackRadius = _bareHandRadius;
            attackCooldown = _bareHandCooldown;
            attackDamage = _bareHandDamage;
            hitFlashColor = _bareHandFlashColor;
        }

        private void Update()
        {
            // Left mouse button triggers the attack.
            if (Input.GetMouseButtonDown(0))
            {
                TryAttack();
            }
        }

        private void TryAttack()
        {
            if (Time.time < _lastAttackTime + attackCooldown)
            {
                return; // still on cooldown, ignore this click
            }

            _lastAttackTime = Time.time;

            // Hit check: OverlapSphere at a point in front of the player.
            // Chosen over a single Raycast because a raycast is a thin line that can miss a target
            // whose collider isn't exactly on that line (e.g. slightly off to the side, or large/irregular
            // shaped). A sphere at the attack point covers a small area in front of the player instead,
            // which is more forgiving and closer to how a melee swing actually behaves.
            Vector3 attackPoint = transform.position + transform.forward * attackRange;
            Collider[] hits = Physics.OverlapSphere(attackPoint, attackRadius);

            bool hitLanded = false;

            foreach (Collider hit in hits)
            {
                if (hit.transform == transform)
                {
                    continue; // don't hit ourselves
                }

                hitLanded = true;
                OnHit(hit);
            }

            if (hitLanded)
            {
                ApplyHitFeedback();
                OnHitLanded?.Invoke();
            }
        }

        private void OnHit(Collider hit)
        {
            Debug.Log($"BareHandMelee hit: {hit.name}");

            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }

            Renderer renderer = hit.GetComponent<Renderer>();
            if (renderer != null)
            {
                StartCoroutine(FlashRoutine(renderer));
            }
        }

        // Fires once per landed SWING (not once per collider hit) so a cleave connecting
        // with multiple targets doesn't stack shake/hit-stop -- both scale with the current
        // weapon's damage so heavier weapons read as heavier hits.
        private void ApplyHitFeedback()
        {
            if (orbitCamera != null)
            {
                float magnitude = shakeMagnitude * Mathf.Max(1f, attackDamage / shakeDamageReference);
                orbitCamera.Shake(shakeDuration, magnitude);
            }

            if (_hitStopCoroutine != null)
            {
                StopCoroutine(_hitStopCoroutine);
            }
            _hitStopCoroutine = StartCoroutine(HitStopRoutine());
        }

        // Uses WaitForSecondsRealtime (unscaled) so the freeze actually resumes -- a
        // scaled wait would never elapse while Time.timeScale itself is near zero.
        private IEnumerator HitStopRoutine()
        {
            float t = Mathf.Clamp01(attackDamage / hitStopHeavyDamageThreshold);
            float duration = Mathf.Lerp(hitStopMinDuration, hitStopMaxDuration, t);

            Time.timeScale = hitStopTimeScale;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1f;
            _hitStopCoroutine = null;
        }

        // Mirrors the _BaseColor/_Color property check already used by
        // PlantPlot.TintRenderer so this works under URP's Lit shader.
        private IEnumerator FlashRoutine(Renderer renderer)
        {
            Material mat = renderer.material;
            bool useBaseColor = mat.HasProperty("_BaseColor");
            bool useLegacyColor = !useBaseColor && mat.HasProperty("_Color");
            if (!useBaseColor && !useLegacyColor)
            {
                yield break;
            }

            string colorProperty = useBaseColor ? "_BaseColor" : "_Color";
            Color original = mat.GetColor(colorProperty);
            float halfDuration = hitFlashDuration / 2f;
            float elapsed = 0f;

            while (elapsed < hitFlashDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed < halfDuration
                    ? elapsed / halfDuration
                    : 1f - (elapsed - halfDuration) / halfDuration;
                mat.SetColor(colorProperty, Color.Lerp(original, hitFlashColor, t));
                yield return null;
            }

            mat.SetColor(colorProperty, original);
        }
    }
}
