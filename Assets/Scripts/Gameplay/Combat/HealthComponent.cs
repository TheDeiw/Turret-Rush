using System;
using UnityEngine;

namespace Gameplay.Combat
{
    public delegate void HealthChangedHandler(int currentHealth, int maxHealth);

    public class HealthComponent : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private int maxHealth = 3;

        public event HealthChangedHandler OnHealthChanged;
        public event Action OnDied;
        public event Action OnDamaged;
        private int _currentHealth;
        public bool IsAlive => _currentHealth > 0;

        private void Awake()
        {
            ResetHealth();
        }

        public void ResetHealth()
        {
            _currentHealth = maxHealth;
            OnHealthChanged?.Invoke(_currentHealth, maxHealth);
        }

        public void TakeDamage(int damage)
        {
            if (!IsAlive)
            {
                return;
            }

            _currentHealth = Mathf.Max(0, _currentHealth - damage);
            OnHealthChanged?.Invoke(_currentHealth, maxHealth);
            OnDamaged?.Invoke();
            //Debug.Log($"HealthComponent: Took {damage} damage. Current health: {CurrentHealth}/{maxHealth}");

            if (_currentHealth <= 0)
            {
                OnDied?.Invoke();
            }
        }
    }
}