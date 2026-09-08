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

        public GameState CurrentState { get; private set; } = GameState.WaitingToStart;

        private MainInputSystem _inputSystem;

        [Inject]
        public void Construct(MainInputSystem inputSystem)
        {
            _inputSystem = inputSystem;
        }

        private void Start()
        {
            _inputSystem.Player.Tap.performed += HandleTap;
        }

        // private void OnDestroy()
        // {
        //     if (_inputSystem != null)
        //     {
        //         _inputSystem.Player.Tap.performed -= HandleTap;
        //     }
        // }

        private void HandleTap(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (CurrentState == GameState.WaitingToStart)
            {
                StartGame();
            }
            else if (CurrentState == GameState.Win || CurrentState == GameState.Lose)
            {
                RestartGame();
            }
        }

        public void StartGame()
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

        private void RestartGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }

    }
}