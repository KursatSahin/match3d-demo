using Match3d.Core.Utils;
using Match3d.Gameplay.Item;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Match3d.Scene
{
    public class GameLifeTimeScope : LifetimeScope
    {
        #region Inspector

        [SerializeField] private GameSceneBootstrapper _gameSceneBootstrapper;
        [SerializeField] private GameLogicManager _gameManager;
        [SerializeField] private ItemDataProvider _itemDataProvider;

        #endregion
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<LevelLoader>(Lifetime.Scoped);
            builder.Register<SlotContainer>(Lifetime.Scoped);
            builder.Register<GameDataContainer>(Lifetime.Scoped);
            builder.Register<Timer>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
            
            builder.RegisterComponent(_gameSceneBootstrapper);
            builder.RegisterComponent(_gameManager);
            builder.RegisterComponent(_itemDataProvider);
        }
    }
}