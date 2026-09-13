using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Core
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 0.5f;

        public event Action OnLevelLoadStart;
        public event Action OnLevelLoadComplete;

        private LevelGenerator _levelGenerator;
        private bool _isLoading;

        [Inject]
        public void Construct(LevelGenerator levelGenerator)
        {
            _levelGenerator = levelGenerator;
        }

        public async UniTask LoadLevelAsync(Action onScreenCovered = null, bool restartLevel = false, CancellationToken cancellationToken = default)
        {
            if (_isLoading)
            {
                return;
            }
            _isLoading = true;

            OnLevelLoadStart?.Invoke();
            await UniTask.Delay(TimeSpan.FromSeconds(fadeDuration), cancellationToken: cancellationToken);

            onScreenCovered?.Invoke();
            if (restartLevel)
            {
                _levelGenerator.ResetEnemies();
            }
            else
            {
                _levelGenerator.GenerateLevel();
            }

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);

            OnLevelLoadComplete?.Invoke();
            await UniTask.Delay(TimeSpan.FromSeconds(fadeDuration), cancellationToken: cancellationToken);

            _isLoading = false;
        }
    }
}

