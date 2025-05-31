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
        /// <summary>
        /// Loads a scene asynchronously, initializes its bootstrapper, and unloads the current scene
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        /// <param name="cancellationToken">Token to cancel the operation</param>
        /// <param name="progress">Progress reporter</param>
        public static async UniTask LoadSceneAsync(string sceneName, CancellationToken cancellationToken = default, IProgress<float> progress = null)
        {
            try
            {
                var currentScene = SceneManager.GetActiveScene();
                
                // Validate inputs
                if (IsInvalidSceneRequest(sceneName, currentScene))
                {
                    return;
                }
                
                // Load the new scene
                var loadedScene = await LoadNewSceneAdditively(sceneName, cancellationToken, progress);
                if (!loadedScene.IsValid())
                {
                    return;
                }
                
                // Initialize the scene bootstrapper
                var sceneBootstrapper = await InitializeSceneBootstrapper(loadedScene, cancellationToken, progress);
                
                // Complete loading and activate the scene
                await ActivateSceneAndUnloadCurrent(loadedScene, currentScene, sceneBootstrapper, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Debug.LogError($"Scene {sceneName} loading canceled");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load scene {sceneName}: {ex.Message}");
            }
        }

        private static bool IsInvalidSceneRequest(string sceneName, UnityEngine.SceneManagement.Scene currentScene)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("Scene name is null or empty");
                return true;
            }

            if (currentScene.name == sceneName)
            {
                Debug.LogWarning($"Scene {sceneName} is already loaded");
                return true;
            }
            
            return false;
        }

        private static async UniTask<UnityEngine.SceneManagement.Scene> LoadNewSceneAdditively(string sceneName, CancellationToken cancellationToken, IProgress<float> progress)
        {
            var loadSceneAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            
            if (loadSceneAsync == null)
            {
                Debug.LogError($"Scene {sceneName} not found");
                return default;
            }

            loadSceneAsync.allowSceneActivation = false;
            
            // Wait until initial loading is almost complete
            await UniTask.WaitUntil(() => 
            {
                progress?.Report(loadSceneAsync.progress / 2);
                return loadSceneAsync.progress >= 0.9f;
            }, cancellationToken: cancellationToken);

            var loadedScene = SceneManager.GetSceneByName(sceneName);
            loadSceneAsync.allowSceneActivation = true;

            await UniTask.WaitUntil(() => loadedScene.isLoaded, cancellationToken: cancellationToken);
            
            return loadedScene;
        }

        private static async UniTask<ISceneBootstrapper> InitializeSceneBootstrapper(UnityEngine.SceneManagement.Scene loadedScene, CancellationToken cancellationToken, IProgress<float> progress)
        {
            var rootObjects = loadedScene.GetRootGameObjects();
            
            foreach (var rootObject in rootObjects)
            {
                if (rootObject.TryGetComponent(out ISceneBootstrapper sceneBootstrapper))
                {
                    await sceneBootstrapper.InitializeAsync(cancellationToken, progress);
                    return sceneBootstrapper;
                }
            }
            
            Debug.LogWarning($"No ISceneBootstrapper found in scene {loadedScene.name}");
            return null;
        }

        private static async UniTask ActivateSceneAndUnloadCurrent(UnityEngine.SceneManagement.Scene loadedScene, UnityEngine.SceneManagement.Scene currentScene, ISceneBootstrapper sceneBootstrapper, CancellationToken cancellationToken)
        {
            // Report 100% progress and wait one frame
            cancellationToken.ThrowIfCancellationRequested();
            SceneManager.SetActiveScene(loadedScene);
            
            // Use the bootstrapper's destruction as cancellation if available
            var unloadToken = sceneBootstrapper != null 
                ? (sceneBootstrapper as MonoBehaviour).GetCancellationTokenOnDestroy() 
                : cancellationToken;
            
            // Unload previous scene
            var unloadSceneAsync = SceneManager.UnloadSceneAsync(currentScene);
            await unloadSceneAsync.ToUniTask(cancellationToken: unloadToken);

            // Activate bootstrapper
            sceneBootstrapper?.OnSceneActivated();
        }
    }
}