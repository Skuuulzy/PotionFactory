using UnityEngine;
using UnityEngine.Events;

namespace VTools.SoWorkflow.EventSystem
{
    public interface IGameEventListener<T>
    {
        void OnEventRaised(T data);
    }
    
    public abstract class GameEventListener<T> : MonoBehaviour, IGameEventListener<T>
    {
        [SerializeField] private GameEvent<T> _gameEvent;
        [SerializeField] private UnityEvent<T> _unityEvent;

        public GameEvent<T> GameEvent => _gameEvent;

        protected void OnEnable()
        {
            _gameEvent.Register(this);
        }

        protected void OnDisable()
        {
            _gameEvent.Deregister(this);
        }

        public void OnEventRaised(T value)
        {
            _unityEvent?.Invoke(value);
        }

    }
    public class GameEventListener : GameEventListener<Empty> { }
}