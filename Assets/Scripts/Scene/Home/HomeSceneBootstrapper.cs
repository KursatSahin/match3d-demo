using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Match3d.Core.DataManager;
using Match3d.Core.Scene;
using VContainer;

namespace Match3d.Scene
{
    public class HomeSceneBootstrapper: SceneBootstrapper
    {
        private const string viewKey = "HomeUIView";
        
        [Inject] private IDataManager _dataManager;
        
        public override async UniTask InitializeAsync(CancellationToken token, IProgress<float> progress = null)
        {
            try
            {
                var view = await uiViewFactory.CreateAsync(viewKey, _sceneEnvironment, container, token);
                if (view == null)
                {
                    throw new Exception($"Couldn't create the view. viewKey: {viewKey}");
                }
                
                view.SetUICamera(_uiCamera);
                view.GameObject.SetActive(true);

                var data = _dataManager.Load();
                if (data == null)
                {
                    throw new Exception("Couldn't load data.");
                }

                if (data.currentLevel == 0)
                {
                    data.currentLevel++;
                    _dataManager.Save(data);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public void OnSceneActivated()
        {
            throw new NotImplementedException();
        }
    }
}