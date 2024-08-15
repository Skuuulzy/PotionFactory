using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

[CustomEditor(typeof(StudioController))]
public class StudioEditor : Editor
{
    private int _removeButtonWidht = 50;
    public override void OnInspectorGUI()
    {
        StudioController studio = (StudioController)target;

        // Start by updating the serialized object
        serializedObject.Update();

        List<CameraPreset> CameraPresetToRemove = new List<CameraPreset>();

        //////////CAMERA PRESET
        EditorGUILayout.LabelField("CAMERA PRESETS", EditorStyles.boldLabel);

        foreach (CameraPreset preset in studio.CameraPresets)
        {
            EditorGUILayout.BeginVertical("box");
            //Set up interface
            if (!preset.IsSetup)
            {
                EditorGUILayout.LabelField("Choose a name for this camera preset:", EditorStyles.boldLabel);
                preset.PresetName = EditorGUILayout.TextField(preset.PresetName);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Set up!"))
                {
                    studio.SetUpPreset(preset);
                }

                if (GUILayout.Button(" - ", GUILayout.Width(_removeButtonWidht)))
                {
                    CameraPresetToRemove.Add(preset);
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField(preset.PresetName, EditorStyles.boldLabel);

                //Remove interface
                if (preset.WantRemoved)
                {
                    EditorGUILayout.HelpBox("Do you want to delete this camera preset?  ", MessageType.Warning);

                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("Cancel"))
                    {
                        studio.WantRemove(preset);
                    }

                    if (GUILayout.Button("Remove only in this list"))
                    {
                        CameraPresetToRemove.Add(preset);
                    }

                    if (GUILayout.Button("Remove in this list and hierarchy"))
                    {
                        preset.WantDeleteFromHierarchie = true;
                        CameraPresetToRemove.Add(preset);
                    }

                    EditorGUILayout.EndHorizontal();
                }
                //Default interface
                else
                {
                    preset.PresetParent = (GameObject)EditorGUILayout.ObjectField("Parent", preset.PresetParent, typeof(GameObject), true);
                    preset.Camera = (Camera)EditorGUILayout.ObjectField("Camera", preset.Camera, typeof(Camera), true);

                    EditorGUILayout.BeginHorizontal();
                    if (studio.SelectedCameraPreset != preset)
                    {
                        if (GUILayout.Button("Select this camera preset"))
                        {
                            studio.SelectCameraPreset(preset);
                        }
                    }
                    else
                    {
                        GUILayout.FlexibleSpace();
                    }

                    if (GUILayout.Button(" - ", GUILayout.Width(_removeButtonWidht)))
                    {
                        studio.WantRemove(preset);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
        }

        foreach (var presetToRemove in CameraPresetToRemove)
        {
            studio.RemovePreset(presetToRemove, presetToRemove.WantRemoved);
        }

        if (GUILayout.Button("ADD A CAMERA PRESET"))
        {
            studio.AddPreset();
        }

        EditorGUILayout.Space(10);
        DrawSeparator();

        //////////Capture
        EditorGUILayout.LabelField("CAPTURE", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Screenshot one")) //Voir pour avoir un hover
        {
            studio.TakeScreenshot(Application.dataPath, StudioController.ImageFormat.JPG, 200, 200);
        }

        if (GUILayout.Button("Screenshot all"))
        {
            studio.TakeScreenshot(Application.dataPath, StudioController.ImageFormat.PNG, 200, 200);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);
        DrawSeparator();

        //////////REFERENCES
        studio.PresetsParentTransform = (Transform)EditorGUILayout.ObjectField("Presets Parent", studio.PresetsParentTransform, typeof(Transform), true);

        // Apply any modifications to the serialized object
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSeparator()
    {
        Rect rect = GUILayoutUtility.GetRect(0f, 1f);
        rect.width -= 10;

        EditorGUI.DrawRect(rect, Color.gray);
    }
}
