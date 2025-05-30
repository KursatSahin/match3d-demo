using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Match3d.Core.Scene
{
    public static class SceneLoader
    {
        public static async UniTask LoadSceneAsync(string sceneName, CancellationToken cancellationToken = default, IProgress<float> progress = null)
        {
            try
            {
                var currentScene = SceneManager.GetActiveScene();

                if (string.IsNullOrEmpty(sceneName))
                {
                    Debug.LogError("Scene name is null or empty");
                    return;
                }

                if (currentScene.name == sceneName)
                {
                    Debug.LogWarning($"Scene {sceneName} is already loaded");
                    return;
                }
                
                var loadSceneAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                if(loadSceneAsync == null)
                {
                    Debug.LogError($"Scene {sceneName} not found");
                    return;
                }

                loadSceneAsync.allowSceneActivation = false;

                await UniTask.WaitUntil(() => 
                {
                    progress?.Report(loadSceneAsync.progress / 2);
                    return loadSceneAsync.progress >= 0.9f;
                }, cancellationToken: cancellationToken);

                var loadedScene = SceneManager.GetSceneByName(sceneName);
                loadSceneAsync.allowSceneActivation = true;

                await UniTask.WaitUntil(() => loadedScene.isLoaded, cancellationToken: cancellationToken);

                var rootObjects = loadedScene.GetRootGameObjects();
                ISceneBootstrapper sceneBootstrapper = null;
                
                foreach (var rootObject in rootObjects)
                {
                    if (!rootObject.TryGetComponent(out sceneBootstrapper))
                    {
                        continue;
                    }
                    await sceneBootstrapper.InitializeAsync(cancellationToken, progress);
                    break;
                }
                
                progress?.Report(1f);
                await UniTask.Yield(cancellationToken: cancellationToken);

                SceneManager.SetActiveScene(loadedScene);
                cancellationToken = (sceneBootstrapper as MonoBehaviour).GetCancellationTokenOnDestroy();

                var unloadSceneAsync = SceneManager.UnloadSceneAsync(currentScene);
                await unloadSceneAsync.ToUniTask(cancellationToken: cancellationToken);

                sceneBootstrapper?.OnSceneActivated();
            }
            catch (OperationCanceledException exception)
            {
                Debug.LogError($"Scene {sceneName} loading canceled");
            }
        }
    }
}