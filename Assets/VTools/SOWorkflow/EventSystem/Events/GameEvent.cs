using System.Collections.Generic;
using UnityEngine;

namespace VTools.SoWorkflow.EventSystem
{
    public class GameEvent<T> : BaseScriptableObject
    {
        private readonly List<IGameEventListener<T>> _listeners = new();

        public void Register(GameEventListener<T> observer) => _listeners.Add(observer);
        public void Deregister(GameEventListener<T> observer) => _listeners.Remove(observer);
        
        public void Raise(T data)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].OnEventRaised(data);
            }
        }
    }

    [CreateAssetMenu(menuName = "SoWorkflow/Game Event", fileName = "New Game Event")]
    public class GameEvent : GameEvent<Empty>
    {
        public void Raise() => Raise(Empty.Default);
    }

    public struct Empty { public static Empty Default => default; }
}