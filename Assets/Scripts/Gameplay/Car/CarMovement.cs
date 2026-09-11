using UnityEngine;
using Zenject;
using Core;

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
        private float _startZPosition;
        private float _startXPosition;
        private float _currentSpeed;

        private GameManager _gameManager;

        [Inject]
        public void Construct(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        private void Start()
        {
            _gameManager.OnGameStarted += HandleGameStarted;
            _gameManager.OnGameWon += StopMovement;
            _gameManager.OnGameLost += StopMovement;
        }

        private void OnDestroy()
        {
            if (_gameManager == null)
            {
                return;
            }
            _gameManager.OnGameStarted -= HandleGameStarted;
            _gameManager.OnGameWon -= StopMovement;
            _gameManager.OnGameLost -= StopMovement;
        }

        private void HandleGameStarted()
        {
            _startZPosition = transform.position.z;
            _startXPosition = transform.position.x;
            _isMoving = true;
            _currentSpeed = speed;
        }

        private void StopMovement()
        {
            _isMoving = false;
        }


        private void Update()
        {
            if (!_isMoving)
            {
                _currentSpeed = Mathf.Lerp(_currentSpeed, 0f, Time.deltaTime * 2f);
                if (_currentSpeed < 0.01f)
                    _currentSpeed = 0f;
            }
            else
            {
                CarWaving();
            }

            if (_currentSpeed > 0f)
            {
                transform.Translate(Vector3.forward * (_currentSpeed * Time.deltaTime));
            }
        }

        private void CarWaving()
        {
            float distanceTraveled = transform.position.z - _startZPosition;
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