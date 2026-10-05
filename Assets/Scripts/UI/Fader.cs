using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class Fader : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 0.5f;

        private void Awake()
        {
            canvasGroup.alpha = 0f;
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }

        public UniTask FadeInAsync(CancellationToken cancellationToken) => FadeAsync(1f, cancellationToken);
        public UniTask FadeOutAsync(CancellationToken cancellationToken) => FadeAsync(0f, cancellationToken);

        private UniTask FadeAsync(float alpha, CancellationToken token)
        {
            DOTween.Kill(this);

            return canvasGroup.DOFade(alpha, fadeDuration)
                .SetEase(Ease.InOutSine)
                .SetTarget(this)
                .SetLink(gameObject)
                .ToUniTask(cancellationToken: token);
        }
    }
}