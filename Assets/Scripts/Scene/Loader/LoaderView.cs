using System;
using Cysharp.Threading.Tasks;
using Match3d.Core;
using Match3d.Core.Scene;
using Match3d.Scene;
using UnityEngine;

public class LoaderView : MonoBehaviour
{
    #region Inspector

    [SerializeField] private ProgressBarView _progressBarView;

    #endregion

    public void LoadGameScene()
    {
        SceneLoader.LoadSceneAsync(GameConstants.SceneNames.Game, this.GetCancellationTokenOnDestroy(), new Progress<float>(progress => _progressBarView.SetFillAmount(progress))).Forget();
    }

    public void LoadHomeScene()
    {
        SceneLoader.LoadSceneAsync(GameConstants.SceneNames.Home, this.GetCancellationTokenOnDestroy(), new Progress<float>(progress => _progressBarView.SetFillAmount(progress))).Forget();
    }
}