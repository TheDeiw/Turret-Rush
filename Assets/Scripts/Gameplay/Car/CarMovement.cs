using UnityEngine;
using UnityEngine.InputSystem;
using Services.Input;
using Zenject;

namespace Gameplay.Car
{
    public class CarMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject carObject;

        [Header("Movement Settings")]
        [SerializeField] private float speed = 10f;
        [SerializeField] private float waveAmplitude;
        [SerializeField] private float waveFrequency;

        private bool _isMoving = false;
        private float _startXPosition;

        private MainInputSystem _gameInput;

        [Inject]
        public void Construct(MainInputSystem gameInput)
        {
            _gameInput = gameInput;
            _gameInput.Player.Tap.performed += OnTap;
        }

        private void Start()
        {
            _startXPosition = transform.position.x;
        }

        private void OnDestroy()
        {
            if (_gameInput != null)
            {
                _gameInput.Player.Tap.performed -= OnTap;
            }
        }

        private void OnTap(InputAction.CallbackContext context)
        {
            _isMoving = true;
        }

        private void Update()
        {
            if (!_isMoving)
            {
                return;
            }

            transform.Translate(Vector3.forward * (speed * Time.deltaTime));
            CarWaving();
        }

        private void CarWaving()
        {
            float distanceTraveled = transform.position.z - _startXPosition;
            float wave = Mathf.Sin(distanceTraveled * waveFrequency) * waveAmplitude;
            Vector3 newPosition = new Vector3(
                _startXPosition + wave,
                carObject.transform.position.y,
                carObject.transform.position.z
            );
            carObject.transform.position = newPosition;

            // Car rotation based on wave
            float slope = waveAmplitude * waveFrequency * Mathf.Cos(distanceTraveled * waveFrequency);
            float rotationAngle = Mathf.Atan(slope) * Mathf.Rad2Deg;

            carObject.transform.rotation = Quaternion.Euler(0f, rotationAngle, 0f);
        }

    }
}