using System.Collections;
using System.Collections.Generic;
using Match3d.Core.Common;
using Match3d.Scene;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VContainer;

public class EndGamePopupFailView : BaseUiView
{
    #region Inspector

    [SerializeField] private Button _confirmButton;

    #endregion
        
    [Inject] private GameLogicManager _gameLogicManager;

    private void OnEnable()
    {
        _confirmButton.onClick.AddListener(OnConfirmButtonClicked);
    }

    private void OnDisable()
    {
        _confirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
    }

    private void OnConfirmButtonClicked()
    {
        _gameLogicManager.EndGameplay();
    }
}