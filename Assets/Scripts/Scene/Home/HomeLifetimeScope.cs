using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Match3d.Scene
{
    public class HomeLifetimeScope : LifetimeScope
    {
        #region Inspector

        [SerializeField] private HomeSceneBootstrapper _homeSceneBootstrapper;

        #endregion

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_homeSceneBootstrapper);
        }
    }
}