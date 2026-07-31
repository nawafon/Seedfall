using UnityEngine;
using Seedfall.Enemies;

namespace Seedfall.Player
{
    // H-key heal action, standalone from PlayerInteract's E/F handling (healing is its own
    // system, not folded into the plot/weapon arbiter). Respects the same "is a menu open"
    // contract every other input consumer checks against Cursor.lockState.
    [RequireComponent(typeof(PlayerSeedInventory))]
    public class PlayerHealAction : MonoBehaviour
    {
        [SerializeField] private KeyCode healKey = KeyCode.H;
        [SerializeField] private float healRange = 2.5f;

        private PlayerSeedInventory _inventory;

        private void Awake()
        {
            _inventory = GetComponent<PlayerSeedInventory>();
        }

        private void Update()
        {
            if (!Input.GetKeyDown(healKey) || Cursor.lockState != CursorLockMode.Locked)
            {
                return;
            }

            EnemyHealth nearest = FindNearestDownedEnemy();
            if (nearest == null)
            {
                Debug.Log("No healable enemy nearby");
                return;
            }

            nearest.TryHeal(_inventory);
        }

        private EnemyHealth FindNearestDownedEnemy()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, healRange);
            EnemyHealth nearest = null;
            float nearestDistanceSqr = float.MaxValue;

            foreach (Collider hit in hits)
            {
                EnemyHealth health = hit.GetComponent<EnemyHealth>();
                if (health == null || health.State != EnemyState.Downed)
                {
                    continue;
                }

                float distanceSqr = (hit.transform.position - transform.position).sqrMagnitude;
                if (distanceSqr < nearestDistanceSqr)
                {
                    nearestDistanceSqr = distanceSqr;
                    nearest = health;
                }
            }

            return nearest;
        }
    }
}
