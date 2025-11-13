using System;
using System.Collections.Generic;
using UnityEngine;

namespace VK.Events
{
    public class EventChannel : IEventChannel
    {
        private readonly List<Action> _actionAddQueue = new();

        // Zero-parameter listeners
        private readonly List<Action> _actionListeners = new();
        private readonly List<Action> _actionRemoveQueue = new();
        private readonly List<Delegate> _genericAddQueue = new();

        // One-parameter generic listeners
        private readonly List<Delegate> _genericListeners = new();
        private readonly List<Delegate> _genericRemoveQueue = new();

        private bool _isPublishing;

        public int ListenerCount => _actionListeners.Count + _genericListeners.Count;
        public bool HasListeners => ListenerCount > 0;

        public void RemoveAllListeners()
        {
            _actionListeners.Clear();
            _genericListeners.Clear();
            _actionAddQueue.Clear();
            _actionRemoveQueue.Clear();
            _genericAddQueue.Clear();
            _genericRemoveQueue.Clear();
        }

        // Zero parameter methods (most common - optimized path)
        public void Subscribe(Action listener)
        {
            if (listener == null) return;

            if (_isPublishing)
            {
                _actionAddQueue.Add(listener);
            }
            else
            {
                if (!_actionListeners.Contains(listener))
                    _actionListeners.Add(listener);
            }
        }

        public void Unsubscribe(Action listener)
        {
            if (listener == null) return;

            if (_isPublishing)
                _actionRemoveQueue.Add(listener);
            else
                _actionListeners.Remove(listener);
        }

        public void Publish()
        {
            if (_actionListeners.Count == 0 && _actionAddQueue.Count == 0) return;

            _isPublishing = true;
            try
            {
                // Direct invocation - no boxing, no wrappers
                for (var i = 0; i < _actionListeners.Count; i++)
                    try
                    {
                        _actionListeners[i].Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error invoking event listener: {e}");
                    }

                ProcessActionQueues();
            }
            finally
            {
                _isPublishing = false;
            }
        }

        // One parameter generic methods
        public void Subscribe<T>(Action<T> listener)
        {
            if (listener == null) return;

            if (_isPublishing)
            {
                _genericAddQueue.Add(listener);
            }
            else
            {
                if (!_genericListeners.Contains(listener))
                    _genericListeners.Add(listener);
            }
        }

        public void Unsubscribe<T>(Action<T> listener)
        {
            if (listener == null) return;

            if (_isPublishing)
                _genericRemoveQueue.Add(listener);
            else
                _genericListeners.Remove(listener);
        }

        public void Publish<T>(T eventData)
        {
            if (_genericListeners.Count == 0 && _genericAddQueue.Count == 0) return;

            _isPublishing = true;
            try
            {
                // Type-safe invocation - no boxing for structs
                for (var i = 0; i < _genericListeners.Count; i++)
                    try
                    {
                        if (_genericListeners[i] is Action<T> typedListener) typedListener.Invoke(eventData);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error invoking event listener: {e}");
                    }

                ProcessGenericQueues();
            }
            finally
            {
                _isPublishing = false;
            }
        }

        // Two parameter methods using struct approach to avoid boxing
        public void Subscribe<T1, T2>(Action<T1, T2> listener)
        {
            // Convert to single parameter using struct to avoid object[] allocation
            Subscribe<EventData<T1, T2>>(data => listener(data.Param1, data.Param2));
        }

        public void Unsubscribe<T1, T2>(Action<T1, T2> listener)
        {
            // For unsubscribe, we need to find and remove the wrapped listener
            // This is complex, so for VR consider using single-parameter events with structs
            Debug.LogWarning(
                "Unsubscribe for multi-parameter events not fully implemented. Use single-parameter struct events for VR.");
        }

        public void Publish<T1, T2>(T1 param1, T2 param2)
        {
            // Use struct to avoid object[] allocation
            var eventData = new EventData<T1, T2> { Param1 = param1, Param2 = param2 };
            Publish(eventData);
        }

        // Three parameter methods
        public void Subscribe<T1, T2, T3>(Action<T1, T2, T3> listener)
        {
            Subscribe<EventData<T1, T2, T3>>(data => listener(data.Param1, data.Param2, data.Param3));
        }

        public void Unsubscribe<T1, T2, T3>(Action<T1, T2, T3> listener)
        {
            Debug.LogWarning(
                "Unsubscribe for multi-parameter events not fully implemented. Use single-parameter struct events for VR.");
        }

        public void Publish<T1, T2, T3>(T1 param1, T2 param2, T3 param3)
        {
            var eventData = new EventData<T1, T2, T3> { Param1 = param1, Param2 = param2, Param3 = param3 };
            Publish(eventData);
        }

        // Four parameter methods
        public void Subscribe<T1, T2, T3, T4>(Action<T1, T2, T3, T4> listener)
        {
            Subscribe<EventData<T1, T2, T3, T4>>(data => listener(data.Param1, data.Param2, data.Param3, data.Param4));
        }

        public void Unsubscribe<T1, T2, T3, T4>(Action<T1, T2, T3, T4> listener)
        {
            Debug.LogWarning(
                "Unsubscribe for multi-parameter events not fully implemented. Use single-parameter struct events for VR.");
        }

        public void Publish<T1, T2, T3, T4>(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            var eventData = new EventData<T1, T2, T3, T4>
                { Param1 = param1, Param2 = param2, Param3 = param3, Param4 = param4 };
            Publish(eventData);
        }

        private void ProcessActionQueues()
        {
            // Process removals first
            foreach (var listener in _actionRemoveQueue) _actionListeners.Remove(listener);
            _actionRemoveQueue.Clear();

            // Process additions
            foreach (var listener in _actionAddQueue)
                if (!_actionListeners.Contains(listener))
                    _actionListeners.Add(listener);
            _actionAddQueue.Clear();
        }

        private void ProcessGenericQueues()
        {
            // Process removals first
            foreach (var listener in _genericRemoveQueue) _genericListeners.Remove(listener);
            _genericRemoveQueue.Clear();

            // Process additions
            foreach (var listener in _genericAddQueue)
                if (!_genericListeners.Contains(listener))
                    _genericListeners.Add(listener);
            _genericAddQueue.Clear();
        }
    }

    // Structs for multi-parameter events to avoid boxing
    public struct EventData<T1, T2>
    {
        public T1 Param1;
        public T2 Param2;
    }

    public struct EventData<T1, T2, T3>
    {
        public T1 Param1;
        public T2 Param2;
        public T3 Param3;
    }

    public struct EventData<T1, T2, T3, T4>
    {
        public T1 Param1;
        public T2 Param2;
        public T3 Param3;
        public T4 Param4;
    }
}