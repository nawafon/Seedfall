using UnityEngine;

namespace Seedfall.Player
{
    // The player's starting "weapon": bare fists.
    // Later this will be swapped out when a grown weapon is equipped,
    // but that swap system is a later step -- this script only does the standalone attack.
    public class BareHandMelee : MonoBehaviour
    {
        [SerializeField] private float attackRange = 1.0f;    // distance in front of the player the hit check is centered on
        [SerializeField] private float attackRadius = 0.5f;   // radius of the hit-check sphere
        [SerializeField] private float attackCooldown = 0.5f; // seconds required between swings

        private float _lastAttackTime = -999f;

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

                Debug.Log($"BareHandMelee hit: {hit.name}");
            }
        }
    }
}
