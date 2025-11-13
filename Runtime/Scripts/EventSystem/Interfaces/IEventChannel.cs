namespace VK.Events
{
    public interface IEventChannel
    {
        int ListenerCount { get; }
        bool HasListeners { get; }
        void RemoveAllListeners();
    }
}