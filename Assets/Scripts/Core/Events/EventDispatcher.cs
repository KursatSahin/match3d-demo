using System;
using System.Collections.Generic;
using UnityEngine;
using Match3d.Core.Events;

namespace Match3d.Core.Events
{
    /// Central event system for dispatching IEvent types.
    public class EventDispatcher : IEventDispatcher
    {
        // Listeners for each event type.
        private readonly Dictionary<Type, Action<IEvent>> _eventListeners;

        public EventDispatcher()
        {
            _eventListeners = new();
        }

        /// Adds a listener for an event type.
        public void AddListener<TEvent>(Action<TEvent> listener) where TEvent : IEvent
        {
            Type eventType = typeof(TEvent);
            if (listener == null)
            {
                Debug.LogError($"[EventDispatcher] Null listener cannot be added for event type {eventType.Name}.");
                return;
            }
            if (!_eventListeners.TryGetValue(eventType, out var existingAction))
            {
                _eventListeners[eventType] = (eventData) => listener((TEvent)eventData);
            }
            else
            {
                _eventListeners[eventType] = existingAction + ((eventData) => listener((TEvent)eventData));
            }
        }
            
        /// Removes a listener for an event type.
        public void RemoveListener<TEvent>(Action<TEvent> listener) where TEvent : IEvent
        {
            Type eventType = typeof(TEvent);
            if (listener == null)
            {
                Debug.LogWarning($"[EventDispatcher] Attempted to remove a null listener for event type {eventType.Name}.");
                return;
            }
            if (_eventListeners.TryGetValue(eventType, out var existingAction))
            {
                _eventListeners[eventType] = existingAction - ((eventData) => listener((TEvent)eventData));
                if (_eventListeners[eventType] == null)
                {
                    _eventListeners.Remove(eventType);
                }
            }
        }

        /// Dispatches an event to listeners.
        public void Dispatch(IEvent eventToDispatch)
        {
            if (eventToDispatch == null)
            {
                Debug.LogError("[EventDispatcher] Cannot dispatch a null event.");
                return;
            }
            Type eventType = eventToDispatch.GetType();
            if (_eventListeners.TryGetValue(eventType, out var listeners))
            {
                listeners?.Invoke(eventToDispatch);
            }
        }

        /// Clears listeners for a specific event type.
        public void ClearListenersForEvent<TEvent>() where TEvent : IEvent
        {
            Type eventType = typeof(TEvent);
            if (_eventListeners.ContainsKey(eventType))
            {
                _eventListeners.Remove(eventType);
            }
        }

        /// Clears all listeners.
        public void ClearAllListeners()
        {
            _eventListeners.Clear();
        }

        public void Dispose() => _eventListeners.Clear();
    }
}