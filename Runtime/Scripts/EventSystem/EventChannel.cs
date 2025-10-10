using System;
using System.Collections.Generic;
using UnityEngine;

namespace VK.Events
{
    public interface IEventChannel
    {
        int ListenerCount { get; }
        void RemoveAllListeners();
        bool HasListeners { get; }
    }

    public class EventChannel : IEventChannel
    {
        private class ListenerWrapper
        {
            public readonly Delegate Listener;
            public readonly Action<object[]> Invoker;

            public ListenerWrapper(Delegate listener, Action<object[]> invoker)
            {
                Listener = listener;
                Invoker = invoker;
            }
        }

        private List<ListenerWrapper> _listeners = new List<ListenerWrapper>();
        private Dictionary<Delegate, ListenerWrapper> _listenerLookup = new Dictionary<Delegate, ListenerWrapper>();
        private List<ListenerWrapper> _listenersToAdd = new List<ListenerWrapper>();
        private List<Delegate> _listenersToRemove = new List<Delegate>();
        private bool _isPublishing;

        public int ListenerCount => _listeners.Count + _listenersToAdd.Count - _listenersToRemove.Count;
        public bool HasListeners => ListenerCount > 0;

        public void SubscribeInternal(Delegate listener, Action<object[]> invoker)
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener));

            if (_listenerLookup.ContainsKey(listener))
            {
                Debug.LogWarning("Listener already subscribed to event channel");
                return;
            }

            var wrapper = new ListenerWrapper(listener, invoker);

            if (_isPublishing)
            {
                _listenersToAdd.Add(wrapper);
            }
            else
            {
                _listeners.Add(wrapper);
            }

            _listenerLookup[listener] = wrapper;
        }

        public void UnsubscribeInternal(Delegate listener)
        {
            if (listener == null) return;

            if (_listenerLookup.TryGetValue(listener, out var wrapper))
            {
                if (_isPublishing)
                {
                    _listenersToRemove.Add(listener);
                }
                else
                {
                    _listeners.Remove(wrapper);
                    _listenerLookup.Remove(listener);
                }
            }
        }

        public void Publish(params object[] parameters)
        {
            _isPublishing = true;

            try
            {
                // Publish to current listeners
                for (int i = 0; i < _listeners.Count; i++)
                {
                    try
                    {
                        _listeners[i].Invoker(parameters);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error invoking event listener: {e}");
                    }
                }

                // Process queued changes
                ProcessQueuedChanges();
            }
            finally
            {
                _isPublishing = false;
            }
        }

        private void ProcessQueuedChanges()
        {
            // Remove listeners
            foreach (var listener in _listenersToRemove)
            {
                if (_listenerLookup.TryGetValue(listener, out var wrapper))
                {
                    _listeners.Remove(wrapper);
                    _listenerLookup.Remove(listener);
                }
            }
            _listenersToRemove.Clear();

            // Add listeners
            foreach (var wrapper in _listenersToAdd)
            {
                _listeners.Add(wrapper);
            }
            _listenersToAdd.Clear();
        }

        public void RemoveAllListeners()
        {
            _listeners.Clear();
            _listenerLookup.Clear();
            _listenersToAdd.Clear();
            _listenersToRemove.Clear();
        }
    }

    // Extension methods for type-safe usage
    public static class EventChannelExtensions
    {
        // Zero parameters
        public static void Subscribe(this EventChannel channel, Action listener) =>
            channel.SubscribeInternal(listener, args => listener());

        public static void Unsubscribe(this EventChannel channel, Action listener) =>
            channel.UnsubscribeInternal(listener);

        public static void Publish(this EventChannel channel) =>
            channel.Publish(Array.Empty<object>());

        // One parameter
        public static void Subscribe<T>(this EventChannel channel, Action<T> listener) =>
            channel.SubscribeInternal(listener, args => listener((T)args[0]));

        public static void Unsubscribe<T>(this EventChannel channel, Action<T> listener) =>
            channel.UnsubscribeInternal(listener);

        public static void Publish<T>(this EventChannel channel, T param1) =>
            channel.Publish(param1);

        // Two parameters
        public static void Subscribe<T1, T2>(this EventChannel channel, Action<T1, T2> listener) =>
            channel.SubscribeInternal(listener, args => listener((T1)args[0], (T2)args[1]));

        public static void Unsubscribe<T1, T2>(this EventChannel channel, Action<T1, T2> listener) =>
            channel.UnsubscribeInternal(listener);

        public static void Publish<T1, T2>(this EventChannel channel, T1 param1, T2 param2) =>
            channel.Publish(param1, param2);

        // Three parameters
        public static void Subscribe<T1, T2, T3>(this EventChannel channel, Action<T1, T2, T3> listener) =>
            channel.SubscribeInternal(listener, args => listener((T1)args[0], (T2)args[1], (T3)args[2]));

        public static void Unsubscribe<T1, T2, T3>(this EventChannel channel, Action<T1, T2, T3> listener) =>
            channel.UnsubscribeInternal(listener);

        public static void Publish<T1, T2, T3>(this EventChannel channel, T1 param1, T2 param2, T3 param3) =>
            channel.Publish(param1, param2, param3);

        // Four parameters
        public static void Subscribe<T1, T2, T3, T4>(this EventChannel channel, Action<T1, T2, T3, T4> listener) =>
            channel.SubscribeInternal(listener, args => listener((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3]));

        public static void Unsubscribe<T1, T2, T3, T4>(this EventChannel channel, Action<T1, T2, T3, T4> listener) =>
            channel.UnsubscribeInternal(listener);

        public static void Publish<T1, T2, T3, T4>(this EventChannel channel, T1 param1, T2 param2, T3 param3, T4 param4) =>
            channel.Publish(param1, param2, param3, param4);
    }
}