using System;
using UnityEngine;

namespace Gameplay.Enemy
{
    public class EnemyLogic : MonoBehaviour, IEnemy
    {
        [Header("Enemy Settings")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float rotationSpeed = 100f;
        [SerializeField] private Animator animator;

        //private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
        private Transform _target;
        private bool _isActive;

        private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");

        private void Awake()
        {
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
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
    }
}
