using System;
using Services.Input;
using UnityEngine;
using Zenject;

namespace Core
{
    public enum GameState
    {
        WaitingToStart,
        Playing,
        Win,
        Lose
    }

    public class GameManager : MonoBehaviour
    {
        public event Action OnGameStarted;
        public event Action OnGameWon;
        public event Action OnGameLost;
        public event Action OnGameRestart;

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
            else if (!_isRestarting && (CurrentState == GameState.Win || CurrentState == GameState.Lose))
            {
                RestartGame(restartSameLayout: CurrentState == GameState.Lose);
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
            CurrentState = GameState.Win;
            OnGameWon?.Invoke();
        }

        public void LoseGame()
        {
            if (CurrentState != GameState.Playing) return;
            CurrentState = GameState.Lose;
            OnGameLost?.Invoke();
        }

        private async void RestartGame(bool restartSameLayout)
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

                    OnGameRestart?.Invoke();
                });
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