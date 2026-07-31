using System.Collections;
using UnityEngine;
using Seedfall.Plants;

namespace Seedfall.Enemies
{
    // Wraps a single arena enemy slot so the arena stays playable for an extended session
    // instead of going empty after 3 kills. Polls for its current enemy being destroyed
    // (killed or healed -- EnemyHealth's death/heal sequence always ends in
    // Destroy(gameObject)) and spawns a fresh one at the same spot after a delay. Same
    // prefab/seedDrop every time -- a respawn wrapper around what already exists, not a
    // new spawning system.
    public class EnemySpawnPoint : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private SeedData seedDrop;
        [SerializeField] private float respawnDelay = 2f;

        private GameObject _currentEnemy;
        private bool _respawnPending;

        private void Start()
        {
            SpawnEnemy();
        }

        private void Update()
        {
            if (_currentEnemy == null && !_respawnPending)
            {
                _respawnPending = true;
                StartCoroutine(RespawnRoutine());
            }
        }

        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(respawnDelay);
            SpawnEnemy();
            _respawnPending = false;
        }

        private void SpawnEnemy()
        {
            _currentEnemy = Instantiate(enemyPrefab, transform.position, transform.rotation, transform);
            EnemyHealth health = _currentEnemy.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.SetSeedDrop(seedDrop);
            }
        }
    }
}
