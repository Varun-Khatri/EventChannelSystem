using System;
using System.Collections.Generic;
using UnityEngine;

namespace VK.Events
{
    public class EventChannelManager : MonoBehaviour, IEventService
    {
        private readonly Dictionary<int, EventChannel> _channels = new Dictionary<int, EventChannel>();
        [SerializeField] private bool _enableDebug = false;

        // Zero parameters
        public void Subscribe(int channelId, Action listener)
        {
            GetOrCreateChannel(channelId).Subscribe(listener);
            if (_enableDebug) Debug.Log($"Subscribed {channelId}");
        }

        public void Unsubscribe(int channelId, Action listener)
        {
            if (_channels.TryGetValue(channelId, out var channel))
            {
                channel.Unsubscribe(listener);
                CheckAndCleanupChannel(channelId, channel);
                if (_enableDebug) Debug.Log($"Unsubscribed {channelId}");
            }
        }

        public void Publish(int channelId)
        {
            if (_channels.TryGetValue(channelId, out var channel))
            {
                channel.Publish();
                if (_enableDebug) Debug.Log($"Published {channelId}");
            }
        }

        // One parameter
        public void Subscribe<T>(int channelId, Action<T> listener)
        {
            GetOrCreateChannel(channelId).Subscribe(listener);
            if (_enableDebug) Debug.Log($"Subscribed {channelId}");
        }

        public void Unsubscribe<T>(int channelId, Action<T> listener)
        {
            if (_channels.TryGetValue(channelId, out var channel))
            {
                channel.Unsubscribe(listener);
                CheckAndCleanupChannel(channelId, channel);
                if (_enableDebug) Debug.Log($"Unsubscribed {channelId}");
            }
        }

        public void Publish<T>(int channelId, T eventData)
        {
            if (_channels.TryGetValue(channelId, out var channel))
            {
                channel.Publish(eventData);
                if (_enableDebug) Debug.Log($"Published {channelId}");
            }
        }

        // Two parameters
        public void Subscribe<T1, T2>(int channelId, Action<T1, T2> listener)
        {
            GetOrCreateChannel(channelId).Subscribe(listener);
            if (_enableDebug) Debug.Log($"Subscribed {channelId}");
        }

        public void Unsubscribe<T1, T2>(int channelId, Action<T1, T2> listener)
        {
            if (_channels.TryGetValue(channelId, out var channel))
            {
                channel.Unsubscribe(listener);
                CheckAndCleanupChannel(channelId, channel);
                if (_enableDebug) Debug.Log($"Unsubscribed {channelId}");
            }
        }

        public void Publish<T1, T2>(int channelId, T1 param1, T2 param2)
        {
            if (_channels.TryGetValue(channelId, out var channel))
            {
                channel.Publish(param1, param2);
                if (_enableDebug) Debug.Log($"Published {channelId}");
            }
        }

        // Three parameters
        public void Subscribe<T1, T2, T3>(int channelId, Action<T1, T2, T3> listener)
        {
            GetOrCreateChannel(channelId).Subscribe(listener);
            if (_enableDebug) Debug.Log($"Subscribed {channelId}");
        }

        public void Unsubscribe<T1, T2, T3>(int channelId, Action<T1, T2, T3> listener)
        {
            if (_channels.TryGetValue(channelId, out var channel))
            {
                channel.Unsubscribe(listener);
                CheckAndCleanupChannel(channelId, channel);
                if (_enableDebug) Debug.Log($"Unsubscribed {channelId}");
            }
        }

        public void Publish<T1, T2, T3>(int channelId, T1 param1, T2 param2, T3 param3)
        {
            if (_channels.TryGetValue(channelId, out var channel))
            {
                channel.Publish(param1, param2, param3);
                if (_enableDebug) Debug.Log($"Published {channelId}");
            }
        }

        // Four parameters
        public void Subscribe<T1, T2, T3, T4>(int channelId, Action<T1, T2, T3, T4> listener)
        {
            GetOrCreateChannel(channelId).Subscribe(listener);
            if (_enableDebug) Debug.Log($"Subscribed {channelId}");
        }

        public void Unsubscribe<T1, T2, T3, T4>(int channelId, Action<T1, T2, T3, T4> listener)
        {
            if (_channels.TryGetValue(channelId, out var channel))
            {
                channel.Unsubscribe(listener);
                CheckAndCleanupChannel(channelId, channel);
                if (_enableDebug) Debug.Log($"Unsubscribed {channelId}");
            }
        }

        public void Publish<T1, T2, T3, T4>(int channelId, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            if (_channels.TryGetValue(channelId, out var channel))
            {
                channel.Publish(param1, param2, param3, param4);
                if (_enableDebug) Debug.Log($"Published {channelId}");
            }
        }

        private EventChannel GetOrCreateChannel(int channelId)
        {
            if (!_channels.TryGetValue(channelId, out var channel))
            {
                channel = new EventChannel();
                _channels[channelId] = channel;
            }
            return channel;
        }

        private void CheckAndCleanupChannel(int channelId, EventChannel channel)
        {
            if (!channel.HasListeners)
                _channels.Remove(channelId);
        }

        public void ClearAll()
        {
            foreach (var channel in _channels.Values)
                channel.RemoveAllListeners();
            _channels.Clear();
        }
    }
}