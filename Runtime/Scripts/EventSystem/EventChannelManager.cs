using System;
using System.Collections.Generic;
using UnityEngine;

namespace VK.Events
{
    public class EventChannelManager : MonoBehaviour
    {
        // Dictionary to store event channels based on Channel IDs
        private readonly Dictionary<string, IEventChannel> _channels = new Dictionary<string, IEventChannel>();

        // Subscribe to a channel with a specific event type
        public void Subscribe<T>(string channelId, Action<T> listener)
        {
            if (!_channels.TryGetValue(channelId, out IEventChannel channel))
            {
                channel = new EventChannel<T>();
                _channels[channelId] = channel;
            }

            var typedChannel = channel as EventChannel<T>;
            typedChannel?.Subscribe(listener);
        }

        // Unsubscribe from a channel
        public void Unsubscribe<T>(string channelId, Action<T> listener)
        {
            if (_channels.TryGetValue(channelId, out IEventChannel channel))
            {
                var typedChannel = channel as EventChannel<T>;
                typedChannel?.Unsubscribe(listener);
            }
        }

        // Publish an event to a channel
        public void Publish<T>(string channelId, T eventData)
        {
            if (_channels.TryGetValue(channelId, out IEventChannel channel))
            {
                var typedChannel = channel as EventChannel<T>;
                typedChannel?.Publish(eventData);
            }
        }
    }
}
