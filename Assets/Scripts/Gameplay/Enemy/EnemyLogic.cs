using Gameplay.Combat;
using UnityEngine;

namespace Gameplay.Enemy
{
    public class EnemyLogic : MonoBehaviour, IEnemy
    {
        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private HealthComponent healthComponent;

        [Header("Enemy Settings")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float rotationSpeed = 100f;
        [SerializeField] private int damage = 1;

        private Transform _target;
        private bool _isActive;

        private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");

        private void Awake()
        {
            if (!animator)
            {
                animator = GetComponentInChildren<Animator>();
            }

            if (!healthComponent)
            {
                healthComponent = GetComponent<HealthComponent>();
            }
            healthComponent.OnDeath += HandleDeath;
        }

        public void Activate(Transform target)
        {
            if (_isActive) return;

            _isActive = true;
            _target = target;

            // Animation
            if (animator)
            {
                animator.SetBool(IsRunningHash, true);
            }
        }

        public void ResetEnemy(Vector3 spawnPosition, Quaternion spawnRotation)
        {
            transform.SetPositionAndRotation(spawnPosition, spawnRotation);

            _isActive = false;
            _target = null;

            if (animator)
            {
                animator.SetBool(IsRunningHash, false);
            }

            if (healthComponent)
            {
                healthComponent.ResetHealth();
            }

            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (!_isActive || !_target) return;

            var direction = _target.position - transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude < 0.001f) return;

            var targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            transform.position += transform.forward * (speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.TryGetComponent<IDamageable>(out var carHealth))
                {
                    carHealth.TakeDamage(damage);
                }

                HandleDeath();
            }
        }

        private void HandleDeath()
        {
            // Other death logic add here
            gameObject.SetActive(false);
        }
    }
}
