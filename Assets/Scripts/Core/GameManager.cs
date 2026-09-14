using System;
using Cysharp.Threading.Tasks;
using Services.Input;
using UnityEngine;
using Zenject;

namespace Core
{
    public enum GameState
    {
        WaitingToStart,
        Playing,
        Won,
        Lost
    }

    public class GameManager : MonoBehaviour
    {
        public event Action OnGameStarted;
        public event Action OnGameWon;
        public event Action OnGameLost;
        public event Action OnGameRestarted;

        private GameState CurrentState { get; set; } = GameState.WaitingToStart;
        private bool _isRestarting = false;

        private MainInputSystem _inputSystem;
        private LevelLoader _levelLoader;
        private LevelGenerator _levelGenerator;

        [Inject]
        public void Construct(MainInputSystem inputSystem, LevelLoader levelLoader, LevelGenerator levelGenerator)
        {
            _inputSystem = inputSystem;
            _levelLoader = levelLoader;
            _levelGenerator = levelGenerator;
        }

        private void Start()
        {
            _levelGenerator.GenerateLevel();
            _inputSystem.Player.Tap.performed += HandleTap;
        }

        private void OnDestroy()
        {
            if (_inputSystem != null)
            {
                _inputSystem.Player.Tap.performed -= HandleTap;
            }
        }

        private void HandleTap(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (CurrentState == GameState.WaitingToStart)
            {
                StartGame();
            }
            else if (!_isRestarting && (CurrentState == GameState.Won || CurrentState == GameState.Lost))
            {
                RestartGameAsync(restartSameLayout: CurrentState == GameState.Lost).Forget();
            }
        }

        private void StartGame()
        {
            CurrentState = GameState.Playing;
            OnGameStarted?.Invoke();
        }

        public void WinGame()
        {
            if (CurrentState != GameState.Playing) return;
            CurrentState = GameState.Won;
            OnGameWon?.Invoke();
        }

        public void LoseGame()
        {
            if (CurrentState != GameState.Playing) return;
            CurrentState = GameState.Lost;
            OnGameLost?.Invoke();
        }

        private async UniTaskVoid RestartGameAsync(bool restartSameLayout)
        {
            _isRestarting = true;
            try
            {
                await _levelLoader.PlayTransitionAsync(onScreenCovered: () =>
                {
                    if (restartSameLayout)
                    {
                        _levelGenerator.ResetEnemies();
                    }
                    else
                    {
                        _levelGenerator.GenerateLevel();
                    }

                    OnGameRestarted?.Invoke();
                }, destroyCancellationToken);
                CurrentState = GameState.WaitingToStart;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                _isRestarting = false;
            }
        }

    }
}