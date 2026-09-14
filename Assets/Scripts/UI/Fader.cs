using Core;
using UnityEngine;
using Zenject;

namespace UI
{
    public class Fader : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private static readonly int FadeInHash = Animator.StringToHash("FadeIn");
        private static readonly int FadeOutHash = Animator.StringToHash("FadeOut");

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
            _levelLoader.OnLevelLoadStart += HandleFadeIn;
            _levelLoader.OnLevelLoadComplete += HandleFadeOut;
        }

        private void OnDestroy()
        {
            if (_levelLoader)
            {
                _levelLoader.OnLevelLoadStart -= HandleFadeIn;
                _levelLoader.OnLevelLoadComplete -= HandleFadeOut;
            }
        }

        private void HandleFadeIn() => animator.SetTrigger(FadeInHash);
        private void HandleFadeOut() => animator.SetTrigger(FadeOutHash);
    }
}