using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Combat;
using UnityEngine;

namespace Gameplay.Enemy
{
    public class EnemyLogic : EnemyBase
    {
        [Header("Components")]
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
        private CancellationTokenSource _hitPunchCts;

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
            healthComponent.OnDamaged += HandleHit;

            _baseScale = transform.localScale;
        }

        private void OnDestroy()
        {
            _hitPunchCts?.Cancel();
            _hitPunchCts?.Dispose();
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

            _hitPunchCts?.Cancel();
            _hitPunchCts?.Dispose();
            _hitPunchCts = null;
            transform.localScale = _baseScale;

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
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.position += transform.forward * (speed * Time.deltaTime);
        }

        private void HandleHit()
        {
            if (hitParticle)
            {
                hitParticle.Play();
            }

            if (gameObject.activeInHierarchy)
            {
                _hitPunchCts?.Cancel();
                _hitPunchCts?.Dispose();
                _hitPunchCts = new CancellationTokenSource();
                HitPunchAsync(_hitPunchCts.Token).Forget();
            }
        }

        private async UniTaskVoid HitPunchAsync(CancellationToken cancellationToken)
        {
            var elapsed = 0f;
            while (elapsed < hitPunchDuration)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / hitPunchDuration;
                transform.localScale = _baseScale * Mathf.Lerp(hitPunchScale, 1f, t);
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }

            transform.localScale = _baseScale;
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
            if (deathParticlePrefab)
            {
                Instantiate(deathParticlePrefab, transform.position, transform.rotation);
            }

            gameObject.SetActive(false);
        }
    }
}
