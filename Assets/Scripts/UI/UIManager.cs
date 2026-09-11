using Core;
using UnityEngine;
using Zenject;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private GameManager _gameManager;

        private static readonly int HideStartHash = Animator.StringToHash("HideStart");
        private static readonly int ShowWinHash = Animator.StringToHash("ShowWin");
        private static readonly int ShowLoseHash = Animator.StringToHash("ShowLose");
        private static readonly int ResetHash = Animator.StringToHash("Reset");

        [Inject]
        public void Construct(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        private void Awake()
        {
            if (!animator)
            {
                animator = GetComponent<Animator>();
            }
        }

        private void Start()
        {
            _gameManager.OnGameStarted += HandleGameStarted;
            _gameManager.OnGameWon += HandleGameWon;
            _gameManager.OnGameLost += HandleGameLost;
            _gameManager.OnGameRestart += HandleGameRestart;
        }

        private void OnDestroy()
        {
            if (_gameManager)
            {
                _gameManager.OnGameStarted -= HandleGameStarted;
                _gameManager.OnGameWon -= HandleGameWon;
                _gameManager.OnGameLost -= HandleGameLost;
                _gameManager.OnGameRestart -= HandleGameRestart;
            }
        }

        private void HandleGameStarted() => animator.SetTrigger(HideStartHash);
        private void HandleGameWon() => animator.SetTrigger(ShowWinHash);
        private void HandleGameLost() => animator.SetTrigger(ShowLoseHash);
        private void HandleGameRestart() => animator.SetTrigger(ResetHash);
    }
}