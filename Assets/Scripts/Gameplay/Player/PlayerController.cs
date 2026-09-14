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
        [SerializeField] private CarVisuals carVisuals;
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private CameraShake cameraShake;

        [Header("Movement Settings")]
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

        private void Start()
        {
            _gameManager.OnGameStarted += HandleGameStart;
            _gameManager.OnGameWon += HandleGameEnd;
            _gameManager.OnGameLost += HandleGameEnd;
            _gameManager.OnGameRestarted += HandleGameRestart;

            healthComponent.OnDied += HandleDeath;
            healthComponent.OnDamaged += HandleHit;

            carVisuals.OnFinishReached += HandleFinish;
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
                carVisuals.UpdateWheels(deltaDistance);
                carVisuals.UpdateWave(transform.position.z);
            }
        }

        private void OnDestroy()
        {
            if (_gameManager)
            {
                _gameManager.OnGameStarted -= HandleGameStart;
                _gameManager.OnGameWon -= HandleGameEnd;
                _gameManager.OnGameLost -= HandleGameEnd;
                _gameManager.OnGameRestarted -= HandleGameRestart;
            }

            healthComponent.OnDied -= HandleDeath;
            healthComponent.OnDamaged -= HandleHit;

            carVisuals.OnFinishReached -= HandleFinish;
        }

        private void HandleGameStart()
        {
            _isMoving = true;
            carVisuals.StartWaving(transform.position.z);
        }

        private void HandleGameEnd()
        {
            _isMoving = false;
            carVisuals.StopWaving();
        }

        private void HandleHit()
        {
            cameraShake.Shake();
        }

        private void HandleDeath()
        {
            HandleGameEnd();
            _gameManager.LoseGame();
        }

        private void HandleFinish()
        {
            HandleGameEnd();
            _gameManager.WinGame();
        }

        private void HandleGameRestart()
        {
            HandleGameEnd();
            _currentSpeed = 0f;
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            carVisuals.ResetPosition();
            healthComponent.ResetHealth();
        }
    }
}
