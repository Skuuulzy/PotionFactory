using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(StudioController))]
public class StudioEditor : Editor
{
    public override void OnInspectorGUI()
    {
        #region GUIStyle
        //Remove button
        GUIStyle removeButton = new GUIStyle(GUI.skin.button);

        Texture2D textureRemove = new Texture2D(1, 1);
        textureRemove.SetPixel(0, 0, new Color(0.73f, 0.27f, 0.27f));
        textureRemove.Apply();

        removeButton.fixedWidth = 50;
        removeButton.fixedHeight = 15;

        removeButton.normal.background = textureRemove;

        //Camera preset non selected
        GUIStyle CameraPresetBox = new GUIStyle(GUI.skin.box);

        Texture2D normalCameraeRemove = new Texture2D(1, 1);
        normalCameraeRemove.SetPixel(0, 0, new Color(0.16f, 0.16f, 0.16f));
        normalCameraeRemove.Apply();

        Texture2D hoverCameraeRemove = new Texture2D(1, 1);
        hoverCameraeRemove.SetPixel(0, 0, new Color(0.19f, 0.19f, 0.19f));
        hoverCameraeRemove.Apply();

        CameraPresetBox.normal.background = normalCameraeRemove;
        CameraPresetBox.hover.background = hoverCameraeRemove;

        //Camera preset selected
        GUIStyle CameraPresetSelected = new GUIStyle(GUI.skin.box);

        Texture2D selectedCameraeRemove = new Texture2D(1, 1);
        selectedCameraeRemove.SetPixel(0, 0, new Color(0.26f, 0.26f, 0.22f));
        selectedCameraeRemove.Apply();

        CameraPresetSelected.normal.background = selectedCameraeRemove;
        #endregion

        StudioController studio = (StudioController)target;

        // Start by updating the serialized object
        serializedObject.Update();

        // Track if there are any changes
        EditorGUI.BeginChangeCheck();

        // Copy of the CameraPreset list for removing items
        List<int> indexesToRemove = new List<int>();

        EditorGUILayout.LabelField("CAMERA PRESETS", EditorStyles.boldLabel);

        for (int i = 0; i < studio.CameraPresets.Count; i++)
        {
            if (studio.CameraPresets[i].PresetName == studio.SelectedCameraPreset.PresetName)
            {
                EditorGUILayout.BeginVertical(CameraPresetSelected);
            }
            else
            {
                EditorGUILayout.BeginVertical(CameraPresetBox);
            }

            // Set up interface
            if (!studio.CameraPresets[i].IsSetup)
            {
                EditorGUILayout.LabelField("Choose a name for this camera preset:", EditorStyles.boldLabel);
                studio.CameraPresets[i].PresetName = EditorGUILayout.TextField(studio.CameraPresets[i].PresetName);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Set up!"))
                {
                    studio.SetUpPreset(studio.CameraPresets[i]);
                }

                if (GUILayout.Button(" - ", removeButton))
                {
                    indexesToRemove.Add(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField(studio.CameraPresets[i].PresetName, EditorStyles.boldLabel);

                // Remove interface
                if (studio.CameraPresets[i].WantRemoved)
                {
                    EditorGUILayout.HelpBox("Do you want to delete this camera preset?", MessageType.Warning);

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Cancel"))
                    {
                        studio.WantRemove(studio.CameraPresets[i]);
                    }

                    if (GUILayout.Button("Remove only in this list"))
                    {
                        indexesToRemove.Add(i);
                    }

                    if (GUILayout.Button("Remove in this list and hierarchy"))
                    {
                        studio.CameraPresets[i].WantDeleteFromHierarchie = true;
                        indexesToRemove.Add(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    studio.CameraPresets[i].PresetParent = (GameObject)EditorGUILayout.ObjectField("Parent", studio.CameraPresets[i].PresetParent, typeof(GameObject), true);
                    studio.CameraPresets[i].Camera = (Camera)EditorGUILayout.ObjectField("Camera", studio.CameraPresets[i].Camera, typeof(Camera), true);

                    EditorGUILayout.Space(10);
                    studio.CameraPresets[i].FilePath = EditorGUILayout.TextField("File path", studio.CameraPresets[i].FilePath);
                    studio.CameraPresets[i].ImageFormat = (ImageFormat)EditorGUILayout.EnumPopup("Image format", studio.CameraPresets[i].ImageFormat);

                    EditorGUILayout.Space(10);
                    EditorGUILayout.BeginHorizontal();
                    studio.CameraPresets[i].ImageSize = (ImageSize)EditorGUILayout.EnumPopup("Image size", studio.CameraPresets[i].ImageSize);
                    if (studio.CameraPresets[i].ImageSize == ImageSize.Custom)
                    {
                        studio.CameraPresets[i].ImageSizeValue = EditorGUILayout.Vector2IntField("", studio.CameraPresets[i].ImageSizeValue);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space(10);
                    EditorGUILayout.BeginHorizontal();
                    if (studio.SelectedCameraPreset.PresetName != studio.CameraPresets[i].PresetName)
                    {
                        if (GUILayout.Button("Select this camera preset"))
                        {
                            studio.SelectCameraPreset(studio.CameraPresets[i]);
                        }
                    }
                    else
                    {
                        GUILayout.FlexibleSpace();
                    }

                    if (GUILayout.Button(" - ", removeButton))
                    {
                        studio.WantRemove(studio.CameraPresets[i]);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
        }

        // Remove the presets after the loop
        foreach (int index in indexesToRemove)
        {
            studio.RemovePreset(studio.CameraPresets[index], studio.CameraPresets[index].WantDeleteFromHierarchie);
        }

        if (GUILayout.Button("ADD A CAMERA PRESET"))
        {
            studio.AddPreset();
        }

        EditorGUILayout.Space(15);
        DrawSeparator();

        EditorGUILayout.LabelField("CAPTURE", EditorStyles.boldLabel);

        EditorGUILayout.LabelField($"Selected camera preset → {studio.SelectedCameraPreset.PresetName}", EditorStyles.label);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Capture one"))
        {
            studio.TakeCapture(studio.SelectedCameraPreset.FilePath, studio.SelectedCameraPreset.ImageFormat, studio.SelectedCameraPreset.ImageSizeValue.x, studio.SelectedCameraPreset.ImageSizeValue.y);
        }

        if (GUILayout.Button("Capture all"))
        {
            studio.TakeAllCapture(studio.SelectedCameraPreset.FilePath, studio.SelectedCameraPreset.ImageFormat, studio.SelectedCameraPreset.ImageSizeValue.x, studio.SelectedCameraPreset.ImageSizeValue.y);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(15);
        DrawSeparator();

        studio.PresetsParentTransform = (Transform)EditorGUILayout.ObjectField("Camera presets parent", studio.PresetsParentTransform, typeof(Transform), true);
        studio.ObjectToCaptureParentTransform = (Transform)EditorGUILayout.ObjectField("Objects to capture parent", studio.ObjectToCaptureParentTransform, typeof(Transform), true);

        // Apply any modifications to the serialized object
        if (EditorGUI.EndChangeCheck())
        {
            // Mark the object as dirty to save changes
            EditorUtility.SetDirty(studio);
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawSeparator()
    {
        Rect rect = GUILayoutUtility.GetRect(0f, 1f);
        rect.width -= 10;
        EditorGUI.DrawRect(rect, Color.gray);
    }
}
