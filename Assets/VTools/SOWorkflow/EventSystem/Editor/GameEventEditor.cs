using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using Object = UnityEngine.Object;

namespace VTools.SoWorkflow.EventSystem
{
    [CustomEditor(typeof(GameEvent<>), true)]
    public class GameEventEditor : Editor
    {
        private List<Object> _eventObservers = new();
        private List<Object> _eventInvokers = new();
        private Type _eventType;
        private MethodInfo _raiseMethod;
        private object _defaultValue;

        private void OnEnable()
        {
            _eventType = target.GetType();

            // Get the `Raise(T)` method from GameEvent<T>
            _raiseMethod = _eventType.GetMethod("Raise");

            // Try to determine the default value for T (if possible)
            Type genericType = _eventType.BaseType?.GetGenericArguments()[0];
            if (genericType != null)
            {
                _defaultValue = genericType.IsValueType ? Activator.CreateInstance(genericType) : null;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI(); // Draw default inspector

            if (_raiseMethod == null)
            {
                Debug.LogError($"[GameEventEditor] Could not find Raise() method on {_eventType}.");
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Observers", EditorStyles.boldLabel);
            DisplayObjectsAsSelectable(_eventObservers);

            if (GUILayout.Button("Refresh Observers"))
            {
                _eventObservers = FindListeners(target);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Invokers", EditorStyles.boldLabel);
            DisplayObjectsAsSelectable(_eventInvokers);

            if (GUILayout.Button("Refresh Invokers"))
            {
                _eventInvokers = FindInvokers(target);
            }

            if (!Application.isPlaying)
            {
                return;
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Invoke with default"))
            {
                if (_raiseMethod != null)
                {
                    _raiseMethod.Invoke(target, new[] { _defaultValue });
                }
            }
        }

        /// <summary>
        /// Finds all GameEventListeners in the loaded scene that reference this event.
        /// </summary>
        private static List<Object> FindListeners(object so)
        {
            var listeners = new List<Object>();

            // Find all GameEventListener instances in the loaded scene
            var allListeners = FindObjectsByType<GameEventListener>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var listener in allListeners)
            {
                if (listener.GameEvent == (GameEvent<Empty>)so && !listeners.Contains(listener))
                {
                    listeners.Add(listener);
                }
            }

            return listeners;
        }

        /// <summary>
        /// Finds all scene GameObjects with MonoBehaviours that reference this event.
        /// </summary>
        private static List<Object> FindInvokers(object so)
        {
            var invokers = new List<Object>();
            var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (var obj in allObjects)
            {
                // Skip assets, only process scene objects
                if (!obj.scene.isLoaded) continue;

                var components = obj.GetComponents<MonoBehaviour>();

                foreach (var component in components)
                {
                    if (component == null || component.GetType() == typeof(GameEventListener))
                        continue;

                    // Use SerializedObject to check for references
                    var currentSerializedObject = new SerializedObject(component);
                    var property = currentSerializedObject.GetIterator();

                    while (property.Next(true))
                    {
                        if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == (Object)so)
                        {
                            invokers.Add(component);
                        }
                    }
                }
            }

            return invokers;
        }

        /// <summary>
        /// Displays a list of objects in the inspector, making them selectable.
        /// </summary>
        private static void DisplayObjectsAsSelectable(List<Object> objects)
        {
            if (objects.Count == 0)
            {
                EditorGUILayout.LabelField("None found. Try Refresh.");
            }
            else
            {
                foreach (var obj in objects)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(obj, typeof(Object), true);

                    if (GUILayout.Button("Select", GUILayout.MaxWidth(60)))
                    {
                        Selection.activeObject = obj;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }
}