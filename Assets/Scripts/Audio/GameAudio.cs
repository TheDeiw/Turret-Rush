using Core;
using Core.Level;
using UnityEngine;
using Zenject;

namespace Audio
{
    public class GameAudio : MonoBehaviour
    {
        [Header("Music Levels")]
        [SerializeField, Range(0f, 1f)] private float menuMusicLevel = 0.5f;
        [SerializeField, Range(0f, 1f)] private float playingMusicLevel = 1f;
        [SerializeField, Range(0f, 1f)] private float resultMusicLevel = 0.2f;
        [SerializeField] private float musicFadeDuration = 0.5f;

        private AudioService _audioService;
        private GameManager _gameManager;
        private LevelLoader _levelLoader;

        [Inject]
        public void Construct(AudioService audioService, GameManager gameManager, LevelLoader levelLoader)
        {
            _audioService = audioService;
            _gameManager = gameManager;
            _levelLoader = levelLoader;
        }

        private void Start()
        {
            _gameManager.OnGameStarted += HandleGameStarted;
            _gameManager.OnGameWon += HandleGameWon;
            _gameManager.OnGameLost += HandleGameLost;
            _gameManager.OnGameRestarted += HandleGameRestarted;
            _levelLoader.OnTransitionStarted += HandleTransitionStarted;

            _audioService.SetMusicLevel(menuMusicLevel, musicFadeDuration);
        }

        private void OnDestroy()
        {
            if (_gameManager)
            {
                _gameManager.OnGameStarted -= HandleGameStarted;
                _gameManager.OnGameWon -= HandleGameWon;
                _gameManager.OnGameLost -= HandleGameLost;
                _gameManager.OnGameRestarted -= HandleGameRestarted;
            }

            if (_levelLoader)
            {
                _levelLoader.OnTransitionStarted -= HandleTransitionStarted;
            }
        }

        private void HandleGameStarted() => _audioService.SetMusicLevel(playingMusicLevel, musicFadeDuration);
        private void HandleGameWon() => PlayResult(SoundId.Win);
        private void HandleGameLost() => PlayResult(SoundId.Lose);
        private void HandleGameRestarted() => _audioService.SetMusicLevel(menuMusicLevel, musicFadeDuration);
        private void HandleTransitionStarted() => _audioService.Play(SoundId.Transition);

        private void PlayResult(SoundId jingle)
        {
            _audioService.SetMusicLevel(resultMusicLevel, musicFadeDuration);
            _audioService.Play(jingle);
        }
    }
}
