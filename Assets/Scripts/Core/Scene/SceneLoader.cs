using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Match3d.Core.Scene
{
    /// <summary>
    /// Handles asynchronous scene loading and management
    /// </summary>
    public static class SceneLoader
    {
        public static async UniTask LoadSceneAsync(string name, CancellationToken token, IProgress<float> progress = null, ISceneOptions options = null)
        {
            try
            {
                var oldScene = SceneManager.GetActiveScene();
                if (oldScene.name == name)
                {
                    //TODO: Logger
                    return;
                }

                var loadSceneAsync = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
                if (loadSceneAsync == null)
                {
                    //TODO: Logger
                    return;
                }
                loadSceneAsync.allowSceneActivation = false;

                await UniTask.WaitUntil(() =>
                {
                    progress?.Report(loadSceneAsync.progress / 2);
                    return loadSceneAsync.progress >= 0.9f;
                }, cancellationToken: token);

                var loadedScene = SceneManager.GetSceneByName(name);
                loadSceneAsync.allowSceneActivation = true;

                await UniTask.WaitUntil(() => loadedScene.isLoaded, cancellationToken: token);

                var rootObjects = loadedScene.GetRootGameObjects();
                ISceneBootstrapper sceneInitializer = null;
                foreach (var rootObject in rootObjects)
                {
                    if (!rootObject.TryGetComponent(out sceneInitializer))
                    {
                        continue;
                    }

                    await sceneInitializer.InitializeAsync(token, options, progress);
                    break;
                }
                progress?.Report(1f);
                await UniTask.Yield(cancellationToken: token);

                SceneManager.SetActiveScene(loadedScene);
                token = (sceneInitializer as MonoBehaviour).GetCancellationTokenOnDestroy();

                var unloadSceneAsync = SceneManager.UnloadSceneAsync(oldScene);
                await unloadSceneAsync.ToUniTask(cancellationToken: token);

                sceneInitializer?.OnSceneActivated();
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}