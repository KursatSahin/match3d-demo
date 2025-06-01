using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Match3d.Core;
using Match3d.Core.Scene;
using UnityEngine;

public class LoaderSceneBootstrapper : MonoBehaviour, ISceneBootstrapper
{
    #region Inspector

    [SerializeField] private LoaderView _LoaderView;

    #endregion

    private ISceneOptions _options;
        
    public UniTask InitializeAsync(CancellationToken token, ISceneOptions options = null, IProgress<float> progress = null)
    {
        _options = options;
        return UniTask.CompletedTask;
    }

    public UniTask InitializeAsync(CancellationToken token, IProgress<float> progress = null)
    {
        throw new NotImplementedException();
    }

    public void OnSceneActivated()
    {
        if (_options is not LoaderSceneOptions loaderSceneOptions)
        {
            return;
        }

        if (loaderSceneOptions.SceneName == GameConstants.SceneNames.Game)
        {
            _LoaderView.LoadGameScene();
        }
        else
        {
            _LoaderView.LoadHomeScene();
        }
    }
}