using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 0.5f;

        public event Action OnLevelLoadStart;
        public event Action OnLevelLoadComplete;

        public async UniTask PlayTransitionAsync(Action onScreenCovered = null, CancellationToken cancellationToken = default)
        {
            OnLevelLoadStart?.Invoke();
            await UniTask.Delay(TimeSpan.FromSeconds(fadeDuration), cancellationToken: cancellationToken);

            onScreenCovered?.Invoke();

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);

            OnLevelLoadComplete?.Invoke();
            await UniTask.Delay(TimeSpan.FromSeconds(fadeDuration), cancellationToken: cancellationToken);
        }
    }
}
