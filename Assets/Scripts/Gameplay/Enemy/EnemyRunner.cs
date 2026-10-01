using DG.Tweening;
using Gameplay.Combat;
using UnityEngine;

namespace Gameplay.Enemy
{
    public class EnemyRunner : EnemyBase
    {
        private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");

        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private HealthComponent healthComponent;

        [Header("Enemy Settings")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float rotationSpeed = 100f;
        [SerializeField] private int damage = 1;

        [Header("Hit Settings")]
        [SerializeField] private ParticleSystem hitParticle;
        [SerializeField] private GameObject deathParticlePrefab;
        [SerializeField] private float hitPunchScale = 1.15f;
        [SerializeField] private float hitPunchDuration = 0.12f;

        private Transform _target;
        private bool _isActive;
        private Vector3 _baseScale;

        private void Awake()
        {
            if (!animator)
            {
                animator = GetComponentInChildren<Animator>();
            }

            healthComponent.OnDied += HandleDeath;
            healthComponent.OnDamaged += HandleHit;

            _baseScale = transform.localScale;
        }

        private void Update()
        {
            if (!_isActive || !_target) return;

            var direction = _target.position - transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude < 0.001f) return;

            var targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.position += transform.forward * (speed * Time.deltaTime);
        }

        private void OnDestroy()
        {
            healthComponent.OnDied -= HandleDeath;
            healthComponent.OnDamaged -= HandleHit;

            transform.DOKill();
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

        public override void Activate(Transform target)
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

        public override void ResetEnemy(Vector3 spawnPosition, Quaternion spawnRotation)
        {
            transform.SetPositionAndRotation(spawnPosition, spawnRotation);

            StopHitPunch();

            _isActive = false;
            _target = null;

            if (animator)
            {
                animator.SetBool(IsRunningHash, false);
            }

            healthComponent.ResetHealth();

            gameObject.SetActive(true);
        }

        private void HandleHit()
        {
            if (hitParticle)
            {
                hitParticle.Play();
            }

            if (gameObject.activeInHierarchy)
            {
                // Punch is relative to the current scale, so always start it from the base one.
                StopHitPunch();
                transform.DOPunchScale(_baseScale * (hitPunchScale - 1f), hitPunchDuration)
                    .SetLink(gameObject);
            }
        }

        private void StopHitPunch()
        {
            transform.DOKill();
            transform.localScale = _baseScale;
        }

        private void HandleDeath()
        {
            // Pooled enemies are only deactivated, so the tween has to be killed manually.
            StopHitPunch();

            if (deathParticlePrefab)
            {
                Instantiate(deathParticlePrefab, transform.position, transform.rotation);
            }

            gameObject.SetActive(false);
        }
    }
}
