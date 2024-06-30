using Components.Machines;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MachineController))]
public class MachineControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MachineController controller = (MachineController)target;

        if (GUILayout.Button("Add Item"))
        {
            controller.Machine.AddItem();
            // Mark the target object as dirty to ensure changes are saved
            EditorUtility.SetDirty(controller);
        }
    }
}