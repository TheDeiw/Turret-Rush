using DG.Tweening;
using Gameplay.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Combat.View
{
    public class HealthBar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private GameObject backgroundImage;
        [SerializeField] private Image fillImage;

        [Header("Bar Settings")]
        [SerializeField] private bool hideOnFullHealth = true;
        [SerializeField] private float fillDuration = 0.15f;

        private Transform _cameraTransform;

        private void Awake()
        {
            if (!healthComponent)
            {
                healthComponent = GetComponentInParent<HealthComponent>();
            }

            if (Camera.main)
            {
                _cameraTransform = Camera.main.transform;
            }

            if (backgroundImage)
            {
                backgroundImage.SetActive(false);
            }

        }

        private void OnEnable()
        {
            if (healthComponent)
            {
                healthComponent.OnHealthChanged += UpdateBar;

                // Pooled enemies reset health while inactive, so the bar has to catch up on enable.
                SetBar(healthComponent.CurrentHealth, healthComponent.MaxHealth, animate: false);
            }
        }

        private void LateUpdate()
        {
            if (_cameraTransform)
            {
                transform.forward = _cameraTransform.forward;
            }
        }

        private void OnDisable()
        {
            if (healthComponent)
            {
                healthComponent.OnHealthChanged -= UpdateBar;
            }

            if (fillImage)
            {
                fillImage.DOKill();
            }
        }

        private void UpdateBar(int current, int max) => SetBar(current, max, animate: true);

        private void SetBar(int current, int max, bool animate)
        {
            if (!fillImage)
            {
                return;
            }

            var fill = (float) current / max;
            fillImage.DOKill();

            // Only damage is animated; restored health (restart) snaps instantly.
            if (animate && fill < fillImage.fillAmount)
            {
                fillImage.DOFillAmount(fill, fillDuration).SetLink(gameObject);
            }
            else
            {
                fillImage.fillAmount = fill;
            }

            if (hideOnFullHealth && backgroundImage)
            {
                backgroundImage.SetActive(current < max && current > 0);
            }
        }
    }
}