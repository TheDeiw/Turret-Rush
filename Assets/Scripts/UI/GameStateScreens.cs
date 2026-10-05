using Core;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace UI
{
    public class GameStateScreens : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup startScreen;
        [SerializeField] private CanvasGroup winScreen;
        [SerializeField] private CanvasGroup loseScreen;

        [Header("Animation Settings")]
        [SerializeField] private float showDuration = 0.35f;
        [SerializeField] private float hideDuration = 0.2f;
        [SerializeField] private float showStartScale = 0.8f;

        private GameManager _gameManager;

        [Inject]
        public void Construct(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        private void Awake()
        {
            ResetScreens();
        }

        private void Start()
        {
            _gameManager.OnGameStarted += HandleGameStarted;
            _gameManager.OnGameWon += HandleGameWon;
            _gameManager.OnGameLost += HandleGameLost;
            _gameManager.OnGameRestarted += HandleGameRestart;
        }

        private void OnDestroy()
        {
            if (_gameManager)
            {
                _gameManager.OnGameStarted -= HandleGameStarted;
                _gameManager.OnGameWon -= HandleGameWon;
                _gameManager.OnGameLost -= HandleGameLost;
                _gameManager.OnGameRestarted -= HandleGameRestart;
            }

            DOTween.Kill(this);
        }

        private void HandleGameStarted() => Hide(startScreen);
        private void HandleGameWon() => Show(winScreen);
        private void HandleGameLost() => Show(loseScreen);
        private void HandleGameRestart() => ResetScreens();

        private void Show(CanvasGroup screen)
        {
            DOTween.Kill(this);

            screen.alpha = 0f;
            screen.transform.localScale = Vector3.one * showStartScale;

            DOTween.Sequence()
                .Append(screen.DOFade(1f, showDuration))
                .Join(screen.transform.DOScale(1f, showDuration).SetEase(Ease.OutBack))
                .SetTarget(this)
                .SetLink(gameObject);
        }

        private void Hide(CanvasGroup screen)
        {
            DOTween.Kill(this);

            screen.DOFade(0f, hideDuration)
                .SetTarget(this)
                .SetLink(gameObject);
        }

        private void ResetScreens()
        {
            DOTween.Kill(this);

            SetInstant(startScreen, true);
            SetInstant(winScreen, false);
            SetInstant(loseScreen, false);
        }

        private static void SetInstant(CanvasGroup screen, bool visible)
        {
            screen.alpha = visible ? 1f : 0f;
            screen.transform.localScale = Vector3.one;
        }
    }
}