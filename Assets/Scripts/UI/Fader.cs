using Core;
using Core.Level;
using UnityEngine;
using Zenject;

namespace UI
{
    public class Fader : MonoBehaviour
    {
        private static readonly int FadeInHash = Animator.StringToHash("FadeIn");
        private static readonly int FadeOutHash = Animator.StringToHash("FadeOut");

        [Header("References")]
        [SerializeField] private Animator animator;

        private LevelLoader _levelLoader;

        [Inject]
        public void Construct(LevelLoader levelLoader)
        {
            _levelLoader = levelLoader;
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
            _levelLoader.OnTransitionStarted += HandleFadeIn;
            _levelLoader.OnScreenCovered += HandleFadeOut;
        }

        private void OnDestroy()
        {
            if (_levelLoader)
            {
                _levelLoader.OnTransitionStarted -= HandleFadeIn;
                _levelLoader.OnScreenCovered -= HandleFadeOut;
            }
        }

        private void HandleFadeIn() => animator.SetTrigger(FadeInHash);
        private void HandleFadeOut() => animator.SetTrigger(FadeOutHash);
    }
}