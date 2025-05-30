using System;
using Cysharp.Threading.Tasks;
using Match3d.Core;
using Match3d.Core.Common;
using Match3d.Core.DataManager;
using Match3d.Core.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

namespace Match3d.Scene
{
    public class HomeView : BaseUiView
    {
        #region Inspector

        [SerializeField] private Button _playButton;
        [SerializeField] private Button _restartDataButton;

        #endregion

        [Inject] private IDataManager _dataManager;

        private void OnEnable()
        {
            _playButton.onClick.AddListener(OnPlayButtonClicked);
            _restartDataButton.onClick.AddListener(OnRestartDataButtonClicked);
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveListener(OnPlayButtonClicked);
            _restartDataButton.onClick.RemoveListener(OnRestartDataButtonClicked);
        }

        private async void OnPlayButtonClicked()
        {
            try
            {
                await SceneLoader.LoadSceneAsync(GameConstants.SceneNames.Game, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();
            }
            catch (Exception)
            {
                // TODO : Handle loading error, e.g., show a message to the user
            }
        }

        private void OnRestartDataButtonClicked()
        {
            _dataManager.Reset();
            SceneManager.LoadScene(GameConstants.SceneNames.Splash);
        }
    }
}