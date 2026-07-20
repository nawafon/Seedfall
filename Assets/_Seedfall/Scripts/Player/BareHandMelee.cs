using System.Collections;
using UnityEngine;

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

        [Header("Hit feedback (juice only, no damage system yet)")]
        [SerializeField] private float hitFlashDuration = 0.1f;
        [SerializeField] private float shakeDuration = 0.1f;
        [SerializeField] private float shakeMagnitude = 0.08f;
        [SerializeField] private MouseOrbitCamera orbitCamera;

        private float _lastAttackTime = -999f;

        // Cached at Awake so ResetToBareHandStats always restores the true serialized
        // defaults, even after SetStats has overwritten the active fields above.
        private float _bareHandRange;
        private float _bareHandRadius;
        private float _bareHandCooldown;

        private void Awake()
        {
            _bareHandRange = attackRange;
            _bareHandRadius = attackRadius;
            _bareHandCooldown = attackCooldown;
        }

        public void SetStats(float range, float radius, float cooldown)
        {
            attackRange = range;
            attackRadius = radius;
            attackCooldown = cooldown;
        }

        public void ResetToBareHandStats()
        {
            attackRange = _bareHandRange;
            attackRadius = _bareHandRadius;
            attackCooldown = _bareHandCooldown;
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

            foreach (Collider hit in hits)
            {
                if (hit.transform == transform)
                {
                    continue; // don't hit ourselves
                }

                OnHit(hit);
            }
        }

        private void OnHit(Collider hit)
        {
            Debug.Log($"BareHandMelee hit: {hit.name}");

            Renderer renderer = hit.GetComponent<Renderer>();
            if (renderer != null)
            {
                StartCoroutine(FlashRoutine(renderer));
            }

            if (orbitCamera != null)
            {
                orbitCamera.Shake(shakeDuration, shakeMagnitude);
            }
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
                mat.SetColor(colorProperty, Color.Lerp(original, Color.white, t));
                yield return null;
            }

            mat.SetColor(colorProperty, original);
        }
    }
}
