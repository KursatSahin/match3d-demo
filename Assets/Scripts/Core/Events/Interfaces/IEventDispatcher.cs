using System;
using Core.Services;

namespace Match3d.Core.Events
{
    public interface IEventDispatcher : IService, IDisposable
    {
        void AddListener<TEvent>(Action<TEvent> listener) where TEvent : IEvent;
        void RemoveListener<TEvent>(Action<TEvent> listener) where TEvent : IEvent;
        void Dispatch(IEvent eventToDispatch);
        void ClearListenersForEvent<TEvent>() where TEvent : IEvent;
        void ClearAllListeners();
    }
} 