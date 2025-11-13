using System;

namespace VK.Events
{
    public static class EventChannelExtensions
    {
        // Zero parameters
        public static void Subscribe(this EventChannel channel, Action listener)
        {
            channel.Subscribe(listener);
        }

        public static void Unsubscribe(this EventChannel channel, Action listener)
        {
            channel.Unsubscribe(listener);
        }

        public static void Publish(this EventChannel channel)
        {
            channel.Publish();
        }

        // One parameter
        public static void Subscribe<T>(this EventChannel channel, Action<T> listener)
        {
            channel.Subscribe(listener);
        }

        public static void Unsubscribe<T>(this EventChannel channel, Action<T> listener)
        {
            channel.Unsubscribe(listener);
        }

        public static void Publish<T>(this EventChannel channel, T param1)
        {
            channel.Publish(param1);
        }

        // Two parameters
        public static void Subscribe<T1, T2>(this EventChannel channel, Action<T1, T2> listener)
        {
            channel.Subscribe(listener);
        }

        public static void Unsubscribe<T1, T2>(this EventChannel channel, Action<T1, T2> listener)
        {
            channel.Unsubscribe(listener);
        }

        public static void Publish<T1, T2>(this EventChannel channel, T1 param1, T2 param2)
        {
            channel.Publish(param1, param2);
        }

        // Three parameters
        public static void Subscribe<T1, T2, T3>(this EventChannel channel, Action<T1, T2, T3> listener)
        {
            channel.Subscribe(listener);
        }

        public static void Unsubscribe<T1, T2, T3>(this EventChannel channel, Action<T1, T2, T3> listener)
        {
            channel.Unsubscribe(listener);
        }

        public static void Publish<T1, T2, T3>(this EventChannel channel, T1 param1, T2 param2, T3 param3)
        {
            channel.Publish(param1, param2, param3);
        }

        // Four parameters
        public static void Subscribe<T1, T2, T3, T4>(this EventChannel channel, Action<T1, T2, T3, T4> listener)
        {
            channel.Subscribe(listener);
        }

        public static void Unsubscribe<T1, T2, T3, T4>(this EventChannel channel, Action<T1, T2, T3, T4> listener)
        {
            channel.Unsubscribe(listener);
        }

        public static void Publish<T1, T2, T3, T4>(this EventChannel channel, T1 param1, T2 param2, T3 param3,
            T4 param4)
        {
            channel.Publish(param1, param2, param3, param4);
        }
    }
}