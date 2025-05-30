using Match3d.Core.Common;
using Match3d.Core.DataManager;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Match3d.Gameplay
{
    public class RootLifetimeScope : LifetimeScope
    {
        #region Inspector

        [SerializeField] private MonoGameObjectPool _gameItemPool;

        #endregion
        
        protected override void Configure(IContainerBuilder builder)
        {
            var gameItemPoolInstance = Instantiate(_gameItemPool);
            DontDestroyOnLoad(gameItemPoolInstance);

            builder.Register<IDataManager, DataManager>(Lifetime.Singleton);
            builder.Register<IIUIViewFactory, AddressableUIViewFactory>(Lifetime.Singleton);

            builder.RegisterInstance(gameItemPoolInstance).AsSelf().AsImplementedInterfaces();
        }
    }
}