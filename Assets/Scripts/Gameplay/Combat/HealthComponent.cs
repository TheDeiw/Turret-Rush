using System;
using UnityEngine;

namespace Gameplay.Combat
{
    public class HealthComponent : MonoBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 3;

        public event Action<int, int> OnHealthChange;
        public event Action OnDeath;
        private int _currentHealth;
        public bool IsAlive => _currentHealth > 0;

        private void Awake()
        {
            ResetHealth();
        }

        public void ResetHealth()
        {
            _currentHealth = maxHealth;
            OnHealthChange?.Invoke(_currentHealth, maxHealth);
        }

        public void TakeDamage(int damage)
        {
            if (!IsAlive)
            {
                return;
            }

            _currentHealth = Mathf.Max(0, _currentHealth - damage);
            OnHealthChange?.Invoke(_currentHealth, maxHealth);

            //Debug.Log($"HealthComponent: Took {damage} damage. Current health: {CurrentHealth}/{maxHealth}");

            if (_currentHealth <= 0)
            {
                OnDeath?.Invoke();
            }
        }
    }
}