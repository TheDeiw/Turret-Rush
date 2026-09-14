using UnityEngine;
using Zenject;
using Services.Input;

namespace Gameplay.Turret
{
    public class TurretAiming : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private float maxRotationAngle = 75f;
        
        private MainInputSystem _inputSystem;
        private float _currentAngle = 0f;

        [Inject]
        public void Construct(MainInputSystem inputSystem)
        {
            _inputSystem = inputSystem;
        }

        private void Update()
        {
            RotateTurret();
        }

        public void ResetRotation()
        {
            _currentAngle = 0f;
            transform.localRotation = Quaternion.identity;
        }

        private void RotateTurret()
        {
            // Get touch coordinates
            Vector2 touchPosition = _inputSystem.Player.PointerPosition.ReadValue<Vector2>();

            if (touchPosition.sqrMagnitude < 0.01f) return;

            // Normalize the touch position
            var halfScreenWidth = Screen.width * 0.5f;
            var normalizedX = (touchPosition.x - halfScreenWidth) / halfScreenWidth;
            normalizedX = Mathf.Clamp(normalizedX, -1f, 1f);

            // Calculate the target angle based on the normalized touch position
            float targetAngle = normalizedX * maxRotationAngle;
            _currentAngle = Mathf.Lerp(_currentAngle, targetAngle, Time.deltaTime * rotationSpeed);

            gameObject.transform.localRotation = Quaternion.Euler(0f, _currentAngle, 0f);
        }
    }
}

