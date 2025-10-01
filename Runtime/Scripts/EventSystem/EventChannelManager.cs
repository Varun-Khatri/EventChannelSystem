using System;
using System.Collections.Generic;

namespace VK.Events
{
    public interface IEventService
    {
        void Subscribe<T>(int channelId, Action<T> listener);
        void Unsubscribe<T>(int channelId, Action<T> listener);
        void Publish<T>(int channelId, T eventData);
        void Subscribe(int channelId, Action listener);
        void Unsubscribe(int channelId, Action listener);
        void Publish(int channelId);
    }

    public class EventChannelManager : IEventService
    {
        // Much more efficient than Dictionary<string, IEventChannel>
        private readonly Dictionary<int, IEventChannel> _channels = new Dictionary<int, IEventChannel>();

        // Object pool for channels to reduce GC allocations
        private readonly Dictionary<Type, Stack<IEventChannel>> _channelPool = new Dictionary<Type, Stack<IEventChannel>>();

        public void Subscribe<T>(int channelId, Action<T> listener)
        {
            if (!_channels.TryGetValue(channelId, out IEventChannel channel))
            {
                channel = GetOrCreateChannel<T>();
                _channels[channelId] = channel;
            }

            var typedChannel = (EventChannel<T>)channel;
            typedChannel.Subscribe(listener);
        }

        public void Unsubscribe<T>(int channelId, Action<T> listener)
        {
            if (_channels.TryGetValue(channelId, out IEventChannel channel))
            {
                var typedChannel = (EventChannel<T>)channel;
                typedChannel.Unsubscribe(listener);

                // Return to pool if empty
                if (typedChannel.ListenerCount == 0)
                {
                    _channels.Remove(channelId);
                    ReturnToPool(typedChannel);
                }
            }
        }

        public void Publish<T>(int channelId, T eventData)
        {
            if (_channels.TryGetValue(channelId, out IEventChannel channel))
            {
                var typedChannel = (EventChannel<T>)channel;
                typedChannel.Publish(eventData);
            }
        }

        public void Subscribe(int channelId, Action listener)
        {
            if (!_channels.TryGetValue(channelId, out IEventChannel channel))
            {
                channel = GetOrCreateVoidChannel();
                _channels[channelId] = channel;
            }

            var typedChannel = (VoidEventChannel)channel;
            typedChannel.Subscribe(listener);
        }

        public void Unsubscribe(int channelId, Action listener)
        {
            if (_channels.TryGetValue(channelId, out IEventChannel channel))
            {
                var typedChannel = (VoidEventChannel)channel;
                typedChannel.Unsubscribe(listener);

                if (typedChannel.ListenerCount == 0)
                {
                    _channels.Remove(channelId);
                    ReturnToPool(typedChannel);
                }
            }
        }

        public void Publish(int channelId)
        {
            if (_channels.TryGetValue(channelId, out IEventChannel channel))
            {
                var typedChannel = (VoidEventChannel)channel;
                typedChannel.Publish();
            }
        }

        // Object pooling for channels
        private EventChannel<T> GetOrCreateChannel<T>()
        {
            var type = typeof(EventChannel<T>);
            if (_channelPool.TryGetValue(type, out Stack<IEventChannel> pool) && pool.Count > 0)
            {
                return (EventChannel<T>)pool.Pop();
            }
            return new EventChannel<T>();
        }

        private VoidEventChannel GetOrCreateVoidChannel()
        {
            var type = typeof(VoidEventChannel);
            if (_channelPool.TryGetValue(type, out Stack<IEventChannel> pool) && pool.Count > 0)
            {
                return (VoidEventChannel)pool.Pop();
            }
            return new VoidEventChannel();
        }

        private void ReturnToPool(IEventChannel channel)
        {
            var type = channel.GetType();
            if (!_channelPool.TryGetValue(type, out Stack<IEventChannel> pool))
            {
                pool = new Stack<IEventChannel>();
                _channelPool[type] = pool;
            }
            pool.Push(channel);
        }

        // Cleanup method
        public void ClearAll()
        {
            _channels.Clear();
            _channelPool.Clear();
        }
    }
}