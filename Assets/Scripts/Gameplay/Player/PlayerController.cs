using Core;
using Gameplay.CameraActions;
using Gameplay.Combat;
using UnityEngine;
using Zenject;

namespace Gameplay.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CarLogic carLogic;
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private CameraShake cameraShake;

        [Header("Settings")]
        [SerializeField] private float speed = 10f;
        [SerializeField] private float speedChangeRate = 5f;

        private GameManager _gameManager;
        private bool _isMoving;
        private float _currentSpeed;

        [Inject]
        public void Construct(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        private void Awake()
        {
            if (!healthComponent)
            {
                healthComponent = GetComponentInChildren<HealthComponent>();
            }
            if (!carLogic)
            {
                carLogic = GetComponentInChildren<CarLogic>();
            }
            if (!cameraShake)
            {
                cameraShake = GetComponentInChildren<CameraShake>();
            }
        }

        private void Start()
        {
            _gameManager.OnGameStarted += StartMoving;
            _gameManager.OnGameWon += StopMoving;
            _gameManager.OnGameLost += StopMoving;
            _gameManager.OnGameRestart += ResetPlayer;

            if (healthComponent)
            {
                healthComponent.OnDeath += HandleDeath;
                healthComponent.OnDamaged += HandleHit;
            }

            if (carLogic)
            {
                carLogic.OnFinishReached += HandleFinish;
            }
        }

        private void OnDestroy()
        {
            if (_gameManager)
            {
                _gameManager.OnGameStarted -= StartMoving;
                _gameManager.OnGameWon -= StopMoving;
                _gameManager.OnGameLost -= StopMoving;
                _gameManager.OnGameRestart -= ResetPlayer;
            }

            if (healthComponent)
            {
                healthComponent.OnDeath -= HandleDeath;
                healthComponent.OnDamaged -= HandleHit;
            }

            if (carLogic)
            {
                carLogic.OnFinishReached -= HandleFinish;
            }
        }

        private void StartMoving()
        {
            _isMoving = true;
            carLogic.StartWaving(transform.position.z);
        }

        private void StopMoving()
        {
            _isMoving = false;
            carLogic.StopWaving();
        }

        private void HandleHit()
        {
            cameraShake.Shake();
        }

        private void HandleDeath()
        {
            StopMoving();
            _gameManager.LoseGame();
        }

        private void HandleFinish()
        {
            StopMoving();
            _gameManager.WinGame();
        }

        private void ResetPlayer()
        {
            StopMoving();
            _currentSpeed = 0f;
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            if (carLogic)
            {
                carLogic.ResetPosition();
            }
            if (healthComponent)
            {
                healthComponent.ResetHealth();
            }
        }

        private void Update()
        {
            var targetSpeed = _isMoving ? speed : 0f;
            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
            if (_currentSpeed < 0.01f) _currentSpeed = 0f;

            if (_currentSpeed > 0f)
            {
                var deltaDistance = _currentSpeed * Time.deltaTime;
                transform.Translate(Vector3.forward * deltaDistance);
                carLogic.UpdateWheels(deltaDistance);
                carLogic.UpdateWave(transform.position.z);
            }
        }
    }
}