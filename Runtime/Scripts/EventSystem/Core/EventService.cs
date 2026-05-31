using System;
using System.Collections.Generic;

namespace VK.Events
{
    public sealed class EventService : IEventService
    {
        private readonly Dictionary<int, EventChannel> _channels = new(64);

        public void Subscribe(int channelId, Action listener)
        {
            GetOrCreateChannel(channelId).Subscribe(listener);
        }

        public void Unsubscribe(int channelId, Action listener)
        {
            if (_channels.TryGetValue(channelId, out var channel))
                channel.Unsubscribe(listener);
        }

        public void Publish(int channelId)
        {
            if (_channels.TryGetValue(channelId, out var channel))
                channel.Publish();
        }

        public void Subscribe<T>(int channelId, Action<T> listener)
        {
            GetOrCreateChannel(channelId).Subscribe(listener);
        }

        public void Unsubscribe<T>(int channelId, Action<T> listener)
        {
            if (_channels.TryGetValue(channelId, out var channel))
                channel.Unsubscribe(listener);
        }

        public void Publish<T>(int channelId, T eventData)
        {
            if (_channels.TryGetValue(channelId, out var channel))
                channel.Publish(eventData);
        }

        public void Subscribe<T1, T2>(int channelId, Action<T1, T2> listener)
        {
            GetOrCreateChannel(channelId).Subscribe(listener);
        }

        public void Unsubscribe<T1, T2>(int channelId, Action<T1, T2> listener)
        {
            if (_channels.TryGetValue(channelId, out var channel))
                channel.Unsubscribe(listener);
        }

        public void Publish<T1, T2>(int channelId, T1 p1, T2 p2)
        {
            if (_channels.TryGetValue(channelId, out var channel))
                channel.Publish(p1, p2);
        }

        public void Subscribe<T1, T2, T3>(int channelId, Action<T1, T2, T3> listener)
        {
            GetOrCreateChannel(channelId).Subscribe(listener);
        }

        public void Unsubscribe<T1, T2, T3>(int channelId, Action<T1, T2, T3> listener)
        {
            if (_channels.TryGetValue(channelId, out var channel))
                channel.Unsubscribe(listener);
        }

        public void Publish<T1, T2, T3>(int channelId, T1 p1, T2 p2, T3 p3)
        {
            if (_channels.TryGetValue(channelId, out var channel))
                channel.Publish(p1, p2, p3);
        }

        public void Subscribe<T1, T2, T3, T4>(int channelId, Action<T1, T2, T3, T4> listener)
        {
            GetOrCreateChannel(channelId).Subscribe(listener);
        }

        public void Unsubscribe<T1, T2, T3, T4>(int channelId, Action<T1, T2, T3, T4> listener)
        {
            if (_channels.TryGetValue(channelId, out var channel))
                channel.Unsubscribe(listener);
        }

        public void Publish<T1, T2, T3, T4>(int channelId, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            if (_channels.TryGetValue(channelId, out var channel))
                channel.Publish(p1, p2, p3, p4);
        }

        private EventChannel GetOrCreateChannel(int channelId)
        {
            if (_channels.TryGetValue(channelId, out var channel))
                return channel;

            channel = new EventChannel();
            _channels.Add(channelId, channel);
            return channel;
        }

        public void ClearAll()
        {
            foreach (var channel in _channels.Values)
                channel.RemoveAllListeners();

            _channels.Clear();
        }
    }
}