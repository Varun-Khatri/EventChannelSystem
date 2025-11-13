using System;

namespace VK.Events
{
    public interface IEventService
    {
        void Subscribe(int channelId, Action listener);
        void Subscribe<T>(int channelId, Action<T> listener);
        void Subscribe<T1, T2>(int channelId, Action<T1, T2> listener);
        void Subscribe<T1, T2, T3>(int channelId, Action<T1, T2, T3> listener);
        void Subscribe<T1, T2, T3, T4>(int channelId, Action<T1, T2, T3, T4> listener);

        void Unsubscribe(int channelId, Action listener);
        void Unsubscribe<T>(int channelId, Action<T> listener);
        void Unsubscribe<T1, T2>(int channelId, Action<T1, T2> listener);
        void Unsubscribe<T1, T2, T3>(int channelId, Action<T1, T2, T3> listener);
        void Unsubscribe<T1, T2, T3, T4>(int channelId, Action<T1, T2, T3, T4> listener);

        void Publish(int channelId);
        void Publish<T>(int channelId, T eventData);
        void Publish<T1, T2>(int channelId, T1 param1, T2 param2);
        void Publish<T1, T2, T3>(int channelId, T1 param1, T2 param2, T3 param3);
        void Publish<T1, T2, T3, T4>(int channelId, T1 param1, T2 param2, T3 param3, T4 param4);
    }
}