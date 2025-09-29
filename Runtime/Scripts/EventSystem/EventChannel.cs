using System;
using UnityEngine;

namespace VK.Events
{
    // Base interface for event channels
    public interface IEventChannel { }

    // EventChannel class to handle events of type T
    public class EventChannel<T> : IEventChannel
    {
        private event Action<T> OnEventRaised;

        // Subscribe a listener to the event
        public void Subscribe(Action<T> listener)
        {
            OnEventRaised += listener;
        }

        // Unsubscribe a listener from the event
        public void Unsubscribe(Action<T> listener)
        {
            OnEventRaised -= listener;
        }

        // Publish the event, calling all listeners
        public void Publish(T eventData)
        {
            OnEventRaised?.Invoke(eventData);
        }
    }

    public class VoidEventChannel : IEventChannel
    {
        private event Action OnEventRaised;

        public void Subscribe(Action listener) => OnEventRaised += listener;
        public void Unsubscribe(Action listener) => OnEventRaised -= listener;
        public void Publish() => OnEventRaised?.Invoke();
    }

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
