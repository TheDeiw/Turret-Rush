using Audio;
using Core.Level;
using DG.Tweening;
using Zenject;
using Services.Input;
using UnityEngine;
using UI;

namespace Core
{
    public class GameInstaller : MonoInstaller
    {
        [Header("References")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private LevelLoader levelLoader;
        [SerializeField] private LevelGenerator levelGenerator;
        [SerializeField] private Fader fader;
        [SerializeField] private AudioService audioService;

        public override void InstallBindings()
        {
            InitTweens();
            BindInputService();
            BindGameManager();
            BindLevelSystem();
            BindAudio();
        }

        private void InitTweens()
        {
            DOTween.Init(recycleAllByDefault: true, useSafeMode: true);
            DOTween.SetTweensCapacity(200, 50);
        }

        private void BindInputService()
        {
            MainInputSystem gameInput = new MainInputSystem();
            gameInput.Enable();

            Container.Bind<MainInputSystem>().FromInstance(gameInput).AsSingle();
        }

        private void BindGameManager()
        {
            Container.Bind<GameManager>().FromInstance(gameManager).AsSingle();
        }

        private void BindLevelSystem()
        {
            Container.Bind<Fader>().FromInstance(fader).AsSingle();
            Container.Bind<LevelGenerator>().FromInstance(levelGenerator).AsSingle();
            Container.Bind<LevelLoader>().FromInstance(levelLoader).AsSingle();
        }

        private void BindAudio()
        {
            Container.Bind<AudioService>().FromInstance(audioService).AsSingle();
        }
    }
}

