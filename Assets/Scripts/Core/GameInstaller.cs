using Zenject;
using Services.Input;

namespace Core
{
    public class GameInstaller : MonoInstaller
    {
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
            // Container.Bind<GameManager>().AsSingle();
        }
    }
}

