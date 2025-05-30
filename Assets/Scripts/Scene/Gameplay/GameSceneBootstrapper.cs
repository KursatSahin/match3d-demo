using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Match3d.Core.Scene;

namespace Match3d.Scene
{
    public class GameSceneBootstrapper : SceneBootstrapper
    {
        public override UniTask InitializeAsync(CancellationToken token, IProgress<float> progress = null)
        {
            throw new NotImplementedException();
        }

        public void OnSceneActivated()
        {
            throw new NotImplementedException();
        }
    }
}