using System;
using UnityEditor;
using UnityEngine;

namespace SOWorkflow.SharedValues
{
    public class SOSharedValue<T> : ScriptableObject
    {
        [SerializeField, TextArea] 
        private string _description;
        
        [SerializeField] 
        protected T _value;
        
        [SerializeField, Tooltip("Do you want to serialize the value, if true the value will saved across play sessions.")] 
        private bool _persistent;
        
        public event Action<T> OnValueUpdated;

        public T Value => _value;

        public virtual void Set(T value, bool notify = true)
        {
            _value = value;
            
            if (notify)
            {
                OnValueUpdated?.Invoke(_value);
            }
        }
        
        public void ToDefault(bool notify)
        {
            if (_persistent)
            {
                return;
            }
            
            _value = default;
            if (notify)
            {
                OnValueUpdated?.Invoke(_value);
            }
        }
        
#if UNITY_EDITOR
        private void OnEnable()
        {
            // Subscribe to play mode state changes
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            // Unsubscribe when the asset is disabled
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            // Reset value to default when exiting play mode
            if (stateChange == PlayModeStateChange.ExitingPlayMode)
            {
                ToDefault(false);
            }
        }
#endif
    }
}