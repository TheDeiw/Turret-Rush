using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;
using Zenject;

namespace Core.Level
{
    public class LevelLoader : MonoBehaviour
    {

        public event Action OnTransitionStarted;
        public event Action OnScreenCovered;
        public event Action OnTransitionFinished;

        private Fader _fader;

        [Inject]
        public void Construct(Fader fader)
        {
            _fader = fader;
        }

        public async UniTask PlayTransitionAsync(Action onScreenCovered = null, CancellationToken cancellationToken = default)
        {
            OnTransitionStarted?.Invoke();
            await _fader.FadeInAsync(cancellationToken);

            // Screen is fully covered - run caller logic and notify listeners of the same moment.
            onScreenCovered?.Invoke();
            OnScreenCovered?.Invoke();

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);
            await _fader.FadeOutAsync(cancellationToken);

            OnTransitionFinished?.Invoke();
        }
    }
}
