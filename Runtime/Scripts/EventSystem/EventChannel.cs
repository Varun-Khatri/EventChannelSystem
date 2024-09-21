using System;

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
}
