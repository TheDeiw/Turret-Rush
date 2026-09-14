using UnityEngine;
using UnityEngine.Pool;
using Gameplay.Combat;

namespace Gameplay.Turret
{
    public class Bullet : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TrailRenderer trailRenderer;

        [Header("Settings")]
        [SerializeField] private float speed = 35f;
        [SerializeField] private float maxLifetime = 2.5f;
        [SerializeField] private int damage = 1;
        [SerializeField] private float hitRadius = 0.15f;

        private IObjectPool<Bullet> _pool;
        private float _timer;

        public void Init(IObjectPool<Bullet> pool)
        {
            _pool = pool;
        }

        private void OnEnable()
        {
            _timer = 0f;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= maxLifetime)
            {
                ReturnToPool();
                return;
            }

            var previousPosition = transform.position;
            var direction = transform.forward;
            var distance = speed * Time.deltaTime;

            if (Physics.SphereCast(
                    previousPosition,
                    hitRadius, direction,
                    out var hit, distance,
                    ~0, QueryTriggerInteraction.Collide
                    ) &&
                hit.collider.TryGetComponent<IDamageable>(out var damageable)
                )
            {
                transform.position = hit.point;
                damageable.TakeDamage(damage);
                ReturnToPool();
                return;
            }

            transform.position = previousPosition + direction * distance;
        }

        public void ReturnToPool()
        {
            if (trailRenderer)
            {
                trailRenderer.Clear();
            }
            if (gameObject.activeSelf)
            {
                _pool?.Release(this);
            }
        }
    }
}
