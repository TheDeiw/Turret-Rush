using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.CameraActions
{
    public class CameraShake : MonoBehaviour
    {
        [Header("Shake Settings")]
        [SerializeField] private float defaultDuration = 0.2f;
        [SerializeField] private float defaultStrength = 0.35f;

        private Vector3 _initialLocalPosition;
        private CancellationTokenSource _cts;

        private void Awake()
        {
            _initialLocalPosition = transform.localPosition;
        }

        public void Shake(float duration = -1f, float strength = -1f)
        {
            if (duration < 0) duration = defaultDuration;
            if (strength < 0) strength = defaultStrength;

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            ShakeRoutineAsync(duration, strength, _cts.Token).Forget();
        }

        private async UniTask ShakeRoutineAsync(float duration, float strength, CancellationToken ct)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;

                var currentStrength = Mathf.Lerp(strength, 0f, elapsed / duration);

                Vector2 randomCircle = Random.insideUnitCircle * currentStrength;
                transform.localPosition = _initialLocalPosition + new Vector3(randomCircle.x, randomCircle.y, 0f);

                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            transform.localPosition = _initialLocalPosition;
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}