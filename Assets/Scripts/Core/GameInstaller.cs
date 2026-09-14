using Core.Level;
using Zenject;
using Services.Input;
using UnityEngine;

namespace Core
{
    public class GameInstaller : MonoInstaller
    {
        [Header("References")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private LevelLoader levelLoader;
        [SerializeField] private LevelGenerator levelGenerator;

        public override void InstallBindings()
        {
            BindInputService();
            BindGameManager();
            BindLevelSystem();
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
            Container.Bind<LevelGenerator>().FromInstance(levelGenerator).AsSingle();
            Container.Bind<LevelLoader>().FromInstance(levelLoader).AsSingle();
        }
    }
}

