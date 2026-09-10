using UnityEngine;

namespace Gameplay.Turret
{
    public class EnemyDetector : MonoBehaviour
    {
        [SerializeField] private Transform carTransform;

        private void Awake()
        {
            if (!carTransform)
            {
                carTransform = transform.root;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Gameplay.Enemy.IEnemy>(out var enemy))
            {
                enemy.Activate(carTransform);
            }
        }
    }
}