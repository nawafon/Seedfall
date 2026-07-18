using UnityEngine;

namespace Seedfall.Core
{
    public class AutoDestructParticle : MonoBehaviour
    {
        public float lifetime = 2f;

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }
    }
}
