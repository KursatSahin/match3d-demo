using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Match3d.Core;
using Match3d.Core.Common;
using Match3d.Core.DataManager;
using Match3d.Core.Scene;
using Match3d.Gameplay;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace Match3d.Scene
{
    public class GameplayManager : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Transform _sceneEnvironment;

        #endregion
        
        //[Inject] private IObjectResolver _container;
        //[Inject] private IIUIViewFactory _viewFactory;
        //[Inject] private MonoGameObjectPool _gameItemPool;
        //[Inject] private IDataManager _dataManager;

        private const int MatchCount = 3;

        private static int _itemsLayerMask;

        private Camera _camera;
        private bool _isInputDisabled;
        private bool _isGamePaused;

        private void Awake()
        {
            _camera = Camera.main;
            _itemsLayerMask = LayerMask.GetMask("Items");
        }

        private void Start()
        {
            // throw new NotImplementedException();
        }

        private void OnEnable()
        {
            // throw new NotImplementedException();
        }

        private void OnDisable()
        {
            // throw new NotImplementedException();
        }

        private void FixedUpdate()
        {
            if (_isGamePaused)
            {
                return;
            }

            Physics.Simulate(Time.fixedDeltaTime);
        }

        public void Update()
        {
            // throw new NotImplementedException();
        }
    }
}