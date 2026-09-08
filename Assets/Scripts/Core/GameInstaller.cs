using Zenject;
using Services.Input;
using UnityEngine;

namespace Core
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameManager gameManager;
        public override void InstallBindings()
        {
            BindInputService();
            BindGameManager();
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
    }
}

