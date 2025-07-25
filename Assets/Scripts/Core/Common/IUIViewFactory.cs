﻿using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Match3d.Core.Common
{
    public interface IUIViewFactory
    {
        public UniTask<IUIView> CreateAsync(string address, Transform parent, IObjectResolver container, CancellationToken token);
    }
}