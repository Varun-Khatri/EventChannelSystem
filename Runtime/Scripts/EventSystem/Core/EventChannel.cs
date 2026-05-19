using System;
using System.Collections.Generic;
using UnityEngine;

namespace VK.Events
{
    public class EventChannel : IEventChannel
    {
        // ── Zero-parameter listeners ──────────────────────────────────────────
        private readonly List<Action> _actionListeners    = new(8);
        private readonly List<Action> _actionAddQueue     = new(4);
        private readonly List<Action> _actionRemoveQueue  = new(4);

        // ── Generic listeners keyed by exact Action<T> type ──────────────────
        // Dictionary<listenerType, List<Delegate>> avoids the per-invoke is-cast.
        private readonly Dictionary<Type, List<Delegate>> _typedListeners   = new(8);
        private readonly Dictionary<Type, List<Delegate>> _typedAddQueue    = new(4);
        private readonly Dictionary<Type, List<Delegate>> _typedRemoveQueue = new(4);

        // ── Wrapper registry for multi-param Unsubscribe ──────────────────────
        // Maps original Action<T1,T2,...> → the wrapper Action<EventData<...>>
        // so we can find and remove the closure on Unsubscribe.
        private readonly Dictionary<Delegate, Delegate> _wrapperRegistry = new(8);

        private bool _isPublishing;

        public int  ListenerCount => _actionListeners.Count + CountTypedListeners();
        public bool HasListeners  => _actionListeners.Count > 0 || _typedListeners.Count > 0;

        // ─────────────────────────────────────────────────────────────────────
        #region Zero-parameter

        public void Subscribe(Action listener)
        {
            if (listener == null) return;
            if (_isPublishing) { _actionAddQueue.Add(listener); return; }
            if (!_actionListeners.Contains(listener)) _actionListeners.Add(listener);
        }

        public void Unsubscribe(Action listener)
        {
            if (listener == null) return;
            if (_isPublishing) { _actionRemoveQueue.Add(listener); return; }
            _actionListeners.Remove(listener);
        }

        public void Publish()
        {
            if (_actionListeners.Count == 0 && _actionAddQueue.Count == 0) return;
            _isPublishing = true;
            try
            {
                for (int i = 0; i < _actionListeners.Count; i++)
                    try { _actionListeners[i].Invoke(); }
                    catch (Exception e) { Debug.LogError($"[EventChannel] listener threw: {e}"); }
                ProcessActionQueues();
            }
            finally { _isPublishing = false; }
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────
        #region Single-parameter (type-bucketed, no cast per invoke)

        public void Subscribe<T>(Action<T> listener)
        {
            if (listener == null) return;
            var type = typeof(Action<T>);
            if (_isPublishing)
            {
                GetOrCreateList(_typedAddQueue, type).Add(listener);
                return;
            }
            var list = GetOrCreateList(_typedListeners, type);
            if (!list.Contains(listener)) list.Add(listener);
        }

        public void Unsubscribe<T>(Action<T> listener)
        {
            if (listener == null) return;
            var type = typeof(Action<T>);
            if (_isPublishing)
            {
                GetOrCreateList(_typedRemoveQueue, type).Add(listener);
                return;
            }
            if (_typedListeners.TryGetValue(type, out var list)) list.Remove(listener);
        }

        public void Publish<T>(T eventData)
        {
            var type = typeof(Action<T>);
            if (!_typedListeners.TryGetValue(type, out var list) || list.Count == 0)
            {
                // Still drain add-queue in case Subscribe was called during publish
                ProcessTypedQueues(type);
                return;
            }

            _isPublishing = true;
            try
            {
                for (int i = 0; i < list.Count; i++)
                    try { ((Action<T>)list[i]).Invoke(eventData); }
                    catch (Exception e) { Debug.LogError($"[EventChannel] listener threw: {e}"); }
                ProcessTypedQueues(type);
            }
            finally { _isPublishing = false; }
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────
        #region Multi-parameter (struct wrapper + registry so Unsubscribe works)

        public void Subscribe<T1, T2>(Action<T1, T2> listener)
        {
            if (listener == null) return;
            Action<EventData<T1, T2>> wrapper = data => listener(data.Param1, data.Param2);
            _wrapperRegistry[listener] = wrapper;
            Subscribe(wrapper);
        }

        public void Unsubscribe<T1, T2>(Action<T1, T2> listener)
        {
            if (listener == null) return;
            if (_wrapperRegistry.TryGetValue(listener, out var wrapper))
            {
                Unsubscribe((Action<EventData<T1, T2>>)wrapper);
                _wrapperRegistry.Remove(listener);
            }
        }

        public void Publish<T1, T2>(T1 p1, T2 p2) =>
            Publish(new EventData<T1, T2> { Param1 = p1, Param2 = p2 });

        public void Subscribe<T1, T2, T3>(Action<T1, T2, T3> listener)
        {
            if (listener == null) return;
            Action<EventData<T1, T2, T3>> wrapper = data => listener(data.Param1, data.Param2, data.Param3);
            _wrapperRegistry[listener] = wrapper;
            Subscribe(wrapper);
        }

        public void Unsubscribe<T1, T2, T3>(Action<T1, T2, T3> listener)
        {
            if (listener == null) return;
            if (_wrapperRegistry.TryGetValue(listener, out var wrapper))
            {
                Unsubscribe((Action<EventData<T1, T2, T3>>)wrapper);
                _wrapperRegistry.Remove(listener);
            }
        }

        public void Publish<T1, T2, T3>(T1 p1, T2 p2, T3 p3) =>
            Publish(new EventData<T1, T2, T3> { Param1 = p1, Param2 = p2, Param3 = p3 });

        public void Subscribe<T1, T2, T3, T4>(Action<T1, T2, T3, T4> listener)
        {
            if (listener == null) return;
            Action<EventData<T1, T2, T3, T4>> wrapper =
                data => listener(data.Param1, data.Param2, data.Param3, data.Param4);
            _wrapperRegistry[listener] = wrapper;
            Subscribe(wrapper);
        }

        public void Unsubscribe<T1, T2, T3, T4>(Action<T1, T2, T3, T4> listener)
        {
            if (listener == null) return;
            if (_wrapperRegistry.TryGetValue(listener, out var wrapper))
            {
                Unsubscribe((Action<EventData<T1, T2, T3, T4>>)wrapper);
                _wrapperRegistry.Remove(listener);
            }
        }

        public void Publish<T1, T2, T3, T4>(T1 p1, T2 p2, T3 p3, T4 p4) =>
            Publish(new EventData<T1, T2, T3, T4> { Param1 = p1, Param2 = p2, Param3 = p3, Param4 = p4 });

        #endregion

        // ─────────────────────────────────────────────────────────────────────
        #region Lifecycle

        public void RemoveAllListeners()
        {
            _actionListeners.Clear();
            _actionAddQueue.Clear();
            _actionRemoveQueue.Clear();
            _typedListeners.Clear();
            _typedAddQueue.Clear();
            _typedRemoveQueue.Clear();
            _wrapperRegistry.Clear();
        }

        #endregion

        // ─────────────────────────────────────────────────────────────────────
        #region Internals

        private void ProcessActionQueues()
        {
            for (int i = 0; i < _actionRemoveQueue.Count; i++) _actionListeners.Remove(_actionRemoveQueue[i]);
            _actionRemoveQueue.Clear();
            for (int i = 0; i < _actionAddQueue.Count; i++)
            {
                var l = _actionAddQueue[i];
                if (!_actionListeners.Contains(l)) _actionListeners.Add(l);
            }
            _actionAddQueue.Clear();
        }

        private void ProcessTypedQueues(Type type)
        {
            if (_typedRemoveQueue.TryGetValue(type, out var removeList) && removeList.Count > 0)
            {
                if (_typedListeners.TryGetValue(type, out var listeners))
                    for (int i = 0; i < removeList.Count; i++) listeners.Remove(removeList[i]);
                removeList.Clear();
            }
            if (_typedAddQueue.TryGetValue(type, out var addList) && addList.Count > 0)
            {
                var listeners = GetOrCreateList(_typedListeners, type);
                for (int i = 0; i < addList.Count; i++)
                {
                    var l = addList[i];
                    if (!listeners.Contains(l)) listeners.Add(l);
                }
                addList.Clear();
            }
        }

        private static List<Delegate> GetOrCreateList(Dictionary<Type, List<Delegate>> dict, Type type)
        {
            if (!dict.TryGetValue(type, out var list))
            {
                list = new List<Delegate>(4);
                dict[type] = list;
            }
            return list;
        }

        private int CountTypedListeners()
        {
            int count = 0;
            foreach (var list in _typedListeners.Values) count += list.Count;
            return count;
        }

        #endregion
    }

    // ── Payload structs (unchanged) ───────────────────────────────────────────
    public struct EventData<T1, T2>              { public T1 Param1; public T2 Param2; }
    public struct EventData<T1, T2, T3>          { public T1 Param1; public T2 Param2; public T3 Param3; }
    public struct EventData<T1, T2, T3, T4>      { public T1 Param1; public T2 Param2; public T3 Param3; public T4 Param4; }
}