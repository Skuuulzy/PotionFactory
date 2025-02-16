using System.Collections.Generic;
using UnityEngine;

namespace SOWorkflow.EventSystem
{
    /// <summary>
    /// Represents an event channel that enables decoupled communication between systems using the observer pattern.
    /// </summary>
    [CreateAssetMenu(menuName = "SO Workflow/SO Channel", fileName = "New Channel")]
    public class SOChannel : ScriptableObject
    {
        [SerializeField, TextArea] private string _description;
        [SerializeField] private bool _logTraffic;
        
        private readonly HashSet<SOChannelListener> _observers = new();
        
        public void Raise()
        {
            if (_observers.Count == 0)
            {
                Debug.LogWarning($"[SOWorkflow] {name} was raised, but there is no observers.");
                return;
            }

            if (_logTraffic)
            {
                Debug.Log($"[SOWorkflow] {name} raised !");
            }
            
            foreach (var observer in _observers)
            {
                observer.Raise();
            }
        }

        public void Register(SOChannelListener observer)
        {
            if (_logTraffic)
            {
                Debug.Log($"[SOWorkflow] {observer.name} is now observing {name}.");
            }
            
            _observers.Add(observer);
        }

        public void Deregister(SOChannelListener observer)
        {
            if (_logTraffic)
            {
                Debug.Log($"[SOWorkflow] {observer.name} has stopped observing {name}.");
            }
            
            _observers.Remove(observer);
        }
    }
}