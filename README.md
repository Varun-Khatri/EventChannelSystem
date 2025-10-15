# Event System

A robust, type-safe event system for Unity games. Features channel-based communication with support for multiple parameters and automatic memory management.

## 🎯 Features

- **Channel-Based Events** - Organize events by integer channel IDs
- **Type-Safe** - Generic methods for 0-4 parameters with compile-time safety
- **Memory Efficient** - Automatic cleanup of unused channels
- **Thread-Safe Publishing** - Safe listener modification during event publication
- **Debug Support** - Built-in debug logging and monitoring
- **Dual Implementation** - Choose between MonoBehaviour or pure C# service

## 🏗️ Architecture

The system uses a clean interface-based design with two implementations:
IEventService ← EventService (pure C#) ← EventChannelManager (MonoBehaviour)


### Core Components

- **`IEventService`** - Main interface for event subscription and publication
- **`EventChannel`** - Handles listener management and safe event dispatching
- **`EventService`** - Pure C# implementation for non-Unity contexts
- **`EventChannelManager`** - MonoBehaviour implementation for Unity scenes

## 📦 Installation

1. Download or clone this repository
2. Add the files to your Unity project (2019.4+ recommended)
3. Choose your implementation:
   - Use `EventService` for pure C# systems
   - Use `EventChannelManager` for MonoBehaviour-based systems

## 🚀 Quick Start

### 1. Define Your Event Channels

```csharp
public static class EventChannels
{
    public const int ON_PLAYER_DIED = 100;
    public const int ON_PLAYER_HEALTH_CHANGED = 101;
    public const int ON_SCORE_CHANGED = 102;
    public const int ON_ITEM_COLLECTED = 103;
}
```
### 2. Using EventChannelManager (MonoBehaviour)

**Subscriber:**

```csharp
public class UIManager : MonoBehaviour
{
    [SerializeField] private EventChannelManager _eventManager;
    
    private void Start()
    {
        // Subscribe to events
        _eventManager.Subscribe<int>(EventChannels.ON_PLAYER_HEALTH_CHANGED, OnHealthChanged);
        _eventManager.Subscribe<int>(EventChannels.ON_SCORE_CHANGED, OnScoreChanged);
        _eventManager.Subscribe<string, int>(EventChannels.ON_ITEM_COLLECTED, OnItemCollected);
    }
    
    private void OnHealthChanged(int newHealth)
    {
        Debug.Log($"Health updated: {newHealth}");
        // Update health bar UI
    }
    
    private void OnScoreChanged(int newScore)
    {
        Debug.Log($"Score: {newScore}");
        // Update score display
    }
    
    private void OnItemCollected(string itemName, int quantity)
    {
        Debug.Log($"Collected {quantity} {itemName}");
        // Show collection popup
    }
    
    private void OnDestroy()
    {
        // Always unsubscribe to prevent memory leaks
        _eventManager.Unsubscribe<int>(EventChannels.ON_PLAYER_HEALTH_CHANGED, OnHealthChanged);
        _eventManager.Unsubscribe<int>(EventChannels.ON_SCORE_CHANGED, OnScoreChanged);
        _eventManager.Unsubscribe<string, int>(EventChannels.ON_ITEM_COLLECTED, OnItemCollected);
    }
}
```
**Publisher:**

```csharp
public class PlayerController : MonoBehaviour
{
    [SerializeField] private EventChannelManager _eventManager;
    
    private void TakeDamage(int damage)
    {
        // Calculate new health...
        int newHealth = 75;
        
        // Publish health change event
        _eventManager.Publish(EventChannels.ON_PLAYER_HEALTH_CHANGED, newHealth);
    }
    
    private void CollectItem(string itemName, int quantity)
    {
        // Publish item collection with two parameters
        _eventManager.Publish(EventChannels.ON_ITEM_COLLECTED, itemName, quantity);
    }
    
    private void Die()
    {
        // Publish no-parameter event
        _eventManager.Publish(EventChannels.ON_PLAYER_DIED);
    }
}
```

### 3. Using EventService (Pure C#)
```csharp
public class GameManager
{
    private IEventService _eventService = new EventService();
    
    public void Initialize()
    {
        // Subscribe to events
        _eventService.Subscribe<string>(EventChannels.ON_GAME_STATE_CHANGED, OnGameStateChanged);
    }
    
    private void OnGameStateChanged(string newState)
    {
        Console.WriteLine($"Game state changed to: {newState}");
    }
    
    public void ChangeState(string state)
    {
        // Publish event
        _eventService.Publish(EventChannels.ON_GAME_STATE_CHANGED, state);
    }
}
```
### 4. Using Dependency Injection

```csharp
public class AchievementSystem : MonoBehaviour
{
    [Inject] private IEventService _eventService;
    
    private void Start()
    {
        _eventService.Subscribe(EventChannels.ON_PLAYER_DIED, OnPlayerDied);
        _eventService.Subscribe<string, int>(EventChannels.ON_ITEM_COLLECTED, OnItemCollected);
    }
    
    private void OnPlayerDied()
    {
        UnlockAchievement("First Death");
    }
    
    private void OnItemCollected(string itemName, int quantity)
    {
        if (itemName == "Coin" && quantity >= 100)
        {
            UnlockAchievement("Coin Collector");
        }
    }
    
    private void UnlockAchievement(string achievementName)
    {
        Debug.Log($"Achievement Unlocked: {achievementName}");
    }
}
```

## 📖 API Reference
**Subscription Methods**
```csharp
// Different parameter counts supported
Subscribe(int channelId, Action listener)
Subscribe<T>(int channelId, Action<T> listener)
Subscribe<T1, T2>(int channelId, Action<T1, T2> listener)
// ... up to 4 parameters
```
**Publication Methods**
```csharp
Publish(int channelId)
Publish<T>(int channelId, T eventData)
Publish<T1, T2>(int channelId, T1 param1, T2 param2)
// ... up to 4 parameters
```

## 🔧 Configuration
**Event Channels**
Define your event channels as static constants:

```csharp
public static class EventChannels
{
    public const int ON_PLAYER_DIED = 100;
    public const int ON_PLAYER_HEALTH_CHANGED = 101;
    public const int ON_GAME_STATE_CHANGED = 102;
    // Add your custom channels...
}
```
**Debug Mode**

Enable debug logging in EventChannelManager:

```csharp
[SerializeField] private bool _enableDebug = true;
```

## 💡 Best Practices
- Use Meaningful Channel IDs - Create a central static class for all event IDs
- Always Unsubscribe - Prevent memory leaks by unsubscribing in OnDestroy()
- Keep Events Simple - Use events for communication, not for complex logic
- Use Dependency Injection - Inject IEventService for better testability

**Example: Proper Cleanup**
```csharp
public class UIHandler : MonoBehaviour
{
    private void OnEnable()
    {
        _eventService.Subscribe<int>(EventChannels.ON_SCORE_CHANGED, UpdateScore);
    }
    
    private void OnDisable()
    {
        _eventService.Unsubscribe<int>(EventChannels.ON_SCORE_CHANGED, UpdateScore);
    }
}
```

## 🛡️ Safety Features
- Null Safety - Protected against null listeners
- Duplicate Prevention - Prevents multiple subscriptions of same listener
- Cleanup Automation - Automatically removes channels with no listeners
- Exception Handling - Catches and logs exceptions during event publication

## 🎯 Use Cases
- UI Updates - Update health bars, scores, and menus
- Game State Management - Handle game pauses, level changes, etc.
- Achievement System - Trigger achievements on specific events
- Audio System - Play sounds based on game events
- Analytics - Track player actions and game metrics

## 🤝 Contributing
This system is part of my professional portfolio. Feel free to:

- Use in your personal or commercial projects
- Fork and modify for your specific needs
- Suggest improvements or report issues


