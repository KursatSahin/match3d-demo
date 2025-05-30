using UnityEngine;
using Cysharp.Threading.Tasks;
using Match3d.Core;
using System;
using Match3d.Core.Common;

namespace Match3d.Scene
{
    public class SplashView : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private ProgressBarView _progressBar;

        #endregion

        public async void Start()
        {
            await UniTask.Delay(500);
            SceneLoader.LoadSceneAsync(GameConstants.SceneNames.Home, this.GetCancellationTokenOnDestroy(), new Progress<float>(progress => _progressBar.SetFillAmount(progress))).Forget();
        }
    }
}
