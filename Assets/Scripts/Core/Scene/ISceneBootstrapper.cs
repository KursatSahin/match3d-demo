using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Match3d.Core.Common;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace Match3d.Core.Scene
{
    public interface ISceneBootstrapper
    {
        public UniTask InitializeAsync(CancellationToken token, IProgress<float> progress = null);
        public void OnSceneActivated();
    }
    
    public abstract class SceneBootstrapper : MonoBehaviour, ISceneBootstrapper 
    {
        #region Inspector

        [SerializeField] protected Transform _sceneEnvironment;
        [SerializeField] protected Camera _uiCamera;

        #endregion
        
        [Inject] protected IIUIViewFactory uiViewFactory;
        [Inject] protected IObjectResolver container;

        public abstract UniTask InitializeAsync(CancellationToken token, IProgress<float> progress = null);
        public virtual void OnSceneActivated(){}
    }
}