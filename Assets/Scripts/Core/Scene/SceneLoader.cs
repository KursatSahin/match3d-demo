using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Match3d.Core.Scene
{
    /// <summary>
    /// Handles asynchronous scene loading and management.
    /// </summary>
    public static class SceneLoader
    {
        /// <summary>
        /// Loads a scene asynchronously, initializes it, and unloads the previous scene.
        /// </summary>
        /// <param name="name">The name of the scene to load.</param>
        /// <param name="token">Cancellation token for async operations.</param>
        /// <param name="progress">Optional progress reporter.</param>
        /// <param name="options">Optional scene options.</param>
        public static async UniTask LoadSceneAsync(string name, CancellationToken token, IProgress<float> progress = null, ISceneOptions options = null)
        {
            try
            {
                var oldScene = SceneManager.GetActiveScene();
                if (oldScene.name == name)
                {
                    Debug.Log($"Scene '{name}' is already active. Skipping load.");
                    return;
                }

                var loadSceneAsync = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
                if (loadSceneAsync == null)
                {
                    Debug.LogError($"Failed to start loading scene '{name}'.");
                    return;
                }
                loadSceneAsync.allowSceneActivation = false;

                await WaitForSceneLoadProgress(loadSceneAsync, progress, token);

                var loadedScene = SceneManager.GetSceneByName(name);
                loadSceneAsync.allowSceneActivation = true;

                await UniTask.WaitUntil(() => loadedScene.isLoaded, cancellationToken: token);

                var sceneInitializer = await InitializeSceneBootstrapper(loadedScene, token, options, progress);
                progress?.Report(1f);
                await UniTask.Yield(cancellationToken: token);

                SceneManager.SetActiveScene(loadedScene);
                token = (sceneInitializer as MonoBehaviour)?.GetCancellationTokenOnDestroy() ?? token;

                var unloadSceneAsync = SceneManager.UnloadSceneAsync(oldScene);
                await unloadSceneAsync.ToUniTask(cancellationToken: token);

                sceneInitializer?.OnSceneActivated();
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning($"Scene loading for '{name}' was canceled.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception during scene loading: {ex}");
            }
        }

        /// <summary>
        /// Waits for the scene to reach 90% load progress.
        /// </summary>
        private static async UniTask WaitForSceneLoadProgress(AsyncOperation loadSceneAsync, IProgress<float> progress, CancellationToken token)
        {
            await UniTask.WaitUntil(() =>
            {
                progress?.Report(loadSceneAsync.progress / 2);
                return loadSceneAsync.progress >= 0.9f;
            }, cancellationToken: token);
        }

        /// <summary>
        /// Finds and initializes the first ISceneBootstrapper in the scene.
        /// </summary>
        private static async UniTask<ISceneBootstrapper> InitializeSceneBootstrapper(UnityEngine.SceneManagement.Scene loadedScene, CancellationToken token, ISceneOptions options, IProgress<float> progress)
        {
            var rootObjects = loadedScene.GetRootGameObjects();
            foreach (var rootObject in rootObjects)
            {
                if (rootObject.TryGetComponent(out ISceneBootstrapper sceneInitializer))
                {
                    await sceneInitializer.InitializeAsync(token, options, progress);
                    return sceneInitializer;
                }
            }
            Debug.LogWarning($"No ISceneBootstrapper found in scene '{loadedScene.name}'.");
            return null;
        }
    }
}