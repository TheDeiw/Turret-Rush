using System;
using UnityEngine;

namespace Gameplay.Combat
{
    public class HealthComponent : MonoBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 3;

        public event Action<int, int> OnHealthChange;
        public event Action OnDeath;
        public int CurrentHealth { get; private set; }
        public bool IsAlive => CurrentHealth > 0;

        private void Awake()
        {
            ResetHealth();
        }

        public void ResetHealth()
        {
            CurrentHealth = maxHealth;
            OnHealthChange?.Invoke(CurrentHealth, maxHealth);
        }

        public void TakeDamage(int damage)
        {
            if (!IsAlive) return;

            CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
            OnHealthChange?.Invoke(CurrentHealth, maxHealth);

            //Debug.Log($"HealthComponent: Took {damage} damage. Current health: {CurrentHealth}/{maxHealth}");

            if (CurrentHealth <= 0)
            {
                OnDeath?.Invoke();
            }
        }
    }
}