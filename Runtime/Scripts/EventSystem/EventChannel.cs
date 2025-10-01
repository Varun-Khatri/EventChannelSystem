using System;
using System.Collections.Generic;
using UnityEngine;

namespace VK.Events
{
    // Base interface for event channels
    public interface IEventChannel
    {
        int ListenerCount { get; }
        void RemoveAllListeners();
    }

    // EventChannel class to handle events of type T
    public class EventChannel<T> : IEventChannel
    {
        private event Action<T> OnEventRaised;

        // Tracks individual listeners for validation and bulk removal
        private HashSet<Action<T>> _listenersSet;

        public int ListenerCount => OnEventRaised?.GetInvocationList().Length ?? 0;

        public void Subscribe(Action<T> listener)
        {
            // Validation to prevent issues
            if (listener == null) throw new ArgumentNullException(nameof(listener));

            // Use set to check for duplicates before adding
            _listenersSet ??= new HashSet<Action<T>>();
            if (_listenersSet.Contains(listener))
            {
                Debug.LogWarning($"Listener is already subscribed to channel for type {typeof(T).Name}");
                return;
            }

            OnEventRaised += listener;
            _listenersSet.Add(listener);
        }

        public void Unsubscribe(Action<T> listener)
        {
            if (listener == null) return;

            // Check if actually subscribed before removal
            if (_listenersSet == null || !_listenersSet.Contains(listener))
            {
                Debug.LogWarning($"Attempted to unsubscribe a listener that was not subscribed to channel for type {typeof(T).Name}");
                return;
            }

            OnEventRaised -= listener;
            _listenersSet.Remove(listener);

            // Clean up hashset if empty to save memory
            if (_listenersSet.Count == 0)
                _listenersSet = null;
        }

        public void Publish(T eventData)
        {
            OnEventRaised?.Invoke(eventData);
        }

        public void RemoveAllListeners()
        {
            OnEventRaised = null;
            _listenersSet?.Clear();
            _listenersSet = null;
        }
    }

    public class VoidEventChannel : IEventChannel
    {
        private event Action OnEventRaised;
        private HashSet<Action> _listenersSet;

        public int ListenerCount => OnEventRaised?.GetInvocationList().Length ?? 0;

        public void Subscribe(Action listener)
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener));

            _listenersSet ??= new HashSet<Action>();
            if (_listenersSet.Contains(listener))
            {
                Debug.LogWarning("Listener is already subscribed to void event channel");
                return;
            }

            OnEventRaised += listener;
            _listenersSet.Add(listener);
        }

        public void Unsubscribe(Action listener)
        {
            if (listener == null) return;

            if (_listenersSet == null || !_listenersSet.Contains(listener))
            {
                Debug.LogWarning("Attempted to unsubscribe a listener that was not subscribed to void event channel");
                return;
            }

            OnEventRaised -= listener;
            _listenersSet.Remove(listener);

            if (_listenersSet.Count == 0)
                _listenersSet = null;
        }

        public void Publish()
        {
            OnEventRaised?.Invoke();
        }

        public void RemoveAllListeners()
        {
            OnEventRaised = null;
            _listenersSet?.Clear();
            _listenersSet = null;
        }
    }

    // Your specialized channel classes remain the same
    public class BoolEventChannel : EventChannel<bool> { }
    public class IntEventChannel : EventChannel<int> { }
    public class FloatEventChannel : EventChannel<float> { }
    public class DoubleEventChannel : EventChannel<double> { }
    public class LongEventChannel : EventChannel<long> { }
    public class StringEventChannel : EventChannel<string> { }
    public class Vector3EventChannel : EventChannel<Vector3> { }
    public class Vector2EventChannel : EventChannel<Vector2> { }
    public class QuaternionEventChannel : EventChannel<Quaternion> { }
    public class ColorEventChannel : EventChannel<Color> { }
    public class TransformEventChannel : EventChannel<Transform> { }
    public class GameObjectEventChannel : EventChannel<GameObject> { }
}