using Zenject;

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
            // Container.Bind<IInputService>().To<MobileInputService>().AsSingle();
        }

        private void BindGameManager()
        {
            // Container.Bind<GameManager>().AsSingle();
        }
    }
}

