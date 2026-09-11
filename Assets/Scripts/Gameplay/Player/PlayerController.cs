using Core;
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

        [Header("Settings")]
        [SerializeField] private float speed = 10f;

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
            }
        }

        private void StartMoving()
        {
            _isMoving = true;
            _currentSpeed = speed;
            carLogic.StartWaving(transform.position.z);
        }

        private void StopMoving()
        {
            _isMoving = false;
            carLogic.StopWaving();
        }

        private void HandleDeath()
        {
            StopMoving();
            _gameManager.LoseGame();
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
            if (!_isMoving)
            {
                _currentSpeed = Mathf.Lerp(_currentSpeed, 0f, Time.deltaTime * 5f);
                if (_currentSpeed < 0.01f) _currentSpeed = 0f;
            }

            if (_currentSpeed > 0f)
            {
                transform.Translate(Vector3.forward * (_currentSpeed * Time.deltaTime));
                carLogic.UpdateWave(transform.position.z);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Finish"))
            {
                StopMoving();
                _gameManager.WinGame();
            }
        }
    }
}