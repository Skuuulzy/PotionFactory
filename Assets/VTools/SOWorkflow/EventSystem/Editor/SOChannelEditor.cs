using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SOWorkflow.EventSystem
{
    [CustomEditor(typeof(SOChannel))]
    public class SOChannelEditor : Editor
    {
        private List<Object> _eventObservers = new();
        private List<Object> _eventInvokers = new();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // Get the SOChannel target
            var eventChannel = (SOChannel)target;

            EditorGUILayout.Space();

            // Display observers
            EditorGUILayout.LabelField("Observers", EditorStyles.boldLabel);
            DisplayObjectsAsSelectable(_eventObservers);

            if (GUILayout.Button("Refresh Observers"))
            {
                _eventObservers = new List<Object>(FindListeners(eventChannel));
            }

            EditorGUILayout.Space();

            // Display Invokers
            EditorGUILayout.LabelField("Invokers", EditorStyles.boldLabel);
            DisplayObjectsAsSelectable(_eventInvokers);

            if (GUILayout.Button("Refresh Invokers"))
            {
                _eventInvokers = new List<Object>(FindInvokers(eventChannel));
            }

            EditorGUILayout.Space();
            
            // Button to invoke the event
            if (GUILayout.Button("Raise"))
            {
                eventChannel.Raise();
            }
        }

        private static List<Object> FindListeners(SOChannel soChannel)
        {
            var listeners = new List<Object>();

            // Find all EventListenerBase instances in the loaded scene
            var allListeners = FindObjectsByType<SOChannelListener>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var listener in allListeners)
            {
                if (listener.SOChannel == soChannel && !listeners.Contains(listener))
                {
                    listeners.Add(listener);
                }
            }

            return listeners;
        }

        private static List<Object> FindInvokers(SOChannel soChannel)
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
                    if (component == null || component.GetType() == typeof(SOChannelListener))
                        continue;

                    // Use SerializedObject to check for references
                    var currentSerializedObject = new SerializedObject(component);
                    var property = currentSerializedObject.GetIterator();

                    while (property.Next(true))
                    {
                        if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == soChannel)
                        {
                            invokers.Add(component);
                        }
                    }
                }
            }

            return invokers;
        }

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