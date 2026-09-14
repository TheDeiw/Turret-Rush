using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Combat
{
    public class HealthBar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private GameObject backgroundImage;
        [SerializeField] private Image fillImage;

        [Header("Settings")]
        [SerializeField] private bool hideOnFullHealth = true;

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
            }
        }

        private void OnDisable()
        {
            if (healthComponent)
            {
                healthComponent.OnHealthChanged -= UpdateBar;
            }
        }

        private void LateUpdate()
        {
            if (_cameraTransform)
            {
                transform.forward = _cameraTransform.forward;
            }
        }

        private void UpdateBar(int current, int max)
        {
            if (!fillImage)
            {
                return;
            }

            var fill = (float) current / max;
            fillImage.fillAmount = fill;

            if (hideOnFullHealth && backgroundImage)
            {
                backgroundImage.SetActive(current < max && current > 0);
            }
        }
    }
}