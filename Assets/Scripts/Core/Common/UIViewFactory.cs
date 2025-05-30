using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;


namespace Match3d.Core.Common
{
    [UnityEngine.Scripting.Preserve]
    public class AddressableUIViewFactory : IIUIViewFactory
    {
        public async UniTask<IUIView> CreateAsync(string address, Transform parent, IObjectResolver container, CancellationToken token)
        {
            var handle = Addressables.InstantiateAsync(address, parent);
            
            try
            {
                var instance = await handle.ToUniTask<GameObject>(cancellationToken: token);
                var presenter = instance.GetComponent<IUIView>();
                if (presenter == null)
                {
                    throw new Exception($"Couldn't create the presenter. address: {address}");
                }

                container.InjectGameObject(instance);

                return presenter;
            }
            catch (Exception)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }

                throw;
            }
        }
    }
}