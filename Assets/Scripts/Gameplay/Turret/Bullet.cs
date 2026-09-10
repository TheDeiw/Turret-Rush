using UnityEngine;
using UnityEngine.Pool;

namespace Gameplay.Turret
{
    public class Bullet : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TrailRenderer trailRenderer;

        [Header("Settings")]
        [SerializeField] private float speed = 35f;
        [SerializeField] private float maxLifetime = 2.5f;
        //[SerializeField] private int damage = 1;

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
            transform.position += transform.forward * (speed * Time.deltaTime);

            _timer += Time.deltaTime;
            if (_timer >= maxLifetime)
            {
                ReturnToPool();
            }
        }

        private void ReturnToPool()
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
