using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 0.5f;

        public event Action OnTransitionStarted;
        public event Action OnScreenCovered;
        public event Action OnTransitionFinished;

        public async UniTask PlayTransitionAsync(Action onScreenCovered = null, CancellationToken cancellationToken = default)
        {
            OnTransitionStarted?.Invoke();
            await UniTask.Delay(TimeSpan.FromSeconds(fadeDuration), cancellationToken: cancellationToken);

            // Screen is fully covered - run caller logic and notify listeners of the same moment.
            onScreenCovered?.Invoke();
            OnScreenCovered?.Invoke();

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);
            await UniTask.Delay(TimeSpan.FromSeconds(fadeDuration), cancellationToken: cancellationToken);

            OnTransitionFinished?.Invoke();
        }
    }
}
