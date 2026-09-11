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

        public event Action OnLevelReset;

        private LevelGenerator _levelGenerator;
        private bool _isLoading;

        [Inject]
        public void Construct(LevelGenerator levelGenerator)
        {
            _levelGenerator = levelGenerator;
        }

        public async UniTask ReloadLevelAsync(CancellationToken cancellationToken = default)
        {
            if (_isLoading)
            {
                return;
            }
            _isLoading = true;

            await UniTask.Delay(TimeSpan.FromSeconds(fadeDuration), cancellationToken: cancellationToken);

            _levelGenerator.GenerateLevel();
            OnLevelReset?.Invoke();

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);
            await UniTask.Delay(TimeSpan.FromSeconds(fadeDuration), cancellationToken: cancellationToken);

            _isLoading = false;
        }
    }
}

