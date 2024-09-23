/*******************************************************
Product - Audio Sync Pro
  Publisher - TelePresent Games
              http://TelePresentGames.dk
  Author    - Martin Hansen
  Created   - 2024
  (c) 2024 Martin Hansen. All rights reserved.
/*******************************************************/

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace TelePresent.AudioSyncPro
{
    public class AudioSyncProSettings : EditorWindow
    {
        private static Color waveformOutlineColor = DefaultOutlineColor;
        private static Color waveformFillColor = DefaultFillColor;

        private static readonly Color DefaultOutlineColor = new Color32(0xFF, 0xD6, 0x07, 0xFF);
        private static readonly Color DefaultFillColor = new Color32(0xFF, 0xBB, 0x00, 0xFF);

        private List<AudioSourcePlus> audioSourcePlusList = new List<AudioSourcePlus>();
        private List<AudioSource> audioSourceList = new List<AudioSource>();

        private bool showAudioSourcePlusSection = false;
        private bool showAudioSourceSection = false;
        private Vector2 scrollPosition;

        [MenuItem("Tools/TelePresent/Audio Sync Pro Settings")]
        public static void ShowWindow()
        {
            GetWindow<AudioSyncProSettings>("Audio Sync Pro Settings");
        }

        static AudioSyncProSettings()
        {
            LoadSettings();
        }

        private void OnEnable()
        {
            InitializeLists();
        }

        private void OnGUI()
        {
            GUIStyle boldLargeStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 12,
                padding = new RectOffset(20, 10, 5, 5)
            };

            // Begin scroll view
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));

            GUILayout.BeginHorizontal();
            GUILayout.Label("Audio Sync Pro Settings", EditorStyles.boldLabel, GUILayout.Height(30));
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Docs", GUILayout.Width(70), GUILayout.Height(25)))
            {
                Application.OpenURL("https://telepresentgames.dk/Unity%20Asset/Audio%20Sync%20Pro%20Documentation.pdf");
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.LabelField("Thank you for choosing Audio Source Plus!", boldLargeStyle);
            EditorGUILayout.Space(20);

            DrawWaveformSettingsSection();
            DrawAudioSourcePlusSection();
            DrawAudioSourceSection();

            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("© 2024 TelePresent Games", EditorStyles.centeredGreyMiniLabel);

            // End scroll view
            EditorGUILayout.EndScrollView();
        }

        private void DrawWaveformSettingsSection()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("Waveform Settings", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Customize the look of your waveform!", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space(10);

            // Outline Color Field
            waveformOutlineColor = EditorGUILayout.ColorField("Waveform Outline Color", waveformOutlineColor);
            if (GUI.changed)
            {
                SaveSettings();
            }
            EditorGUILayout.Space(5);

            // Fill Color Field
            waveformFillColor = EditorGUILayout.ColorField("Waveform Fill Color", waveformFillColor);
            if (GUI.changed)
            {
                SaveSettings();
            }

            GUILayout.EndVertical();
            EditorGUILayout.Space(20);
        }

        private void InitializeLists()
        {
            if (audioSourcePlusList.Count == 0)
            {
                audioSourcePlusList.Add(null);
            }

            if (audioSourceList.Count == 0)
            {
                audioSourceList.Add(null);
            }
        }

        private static void LoadSettings()
        {
            if (EditorPrefs.HasKey("WaveformOutlineColor"))
            {
                ColorUtility.TryParseHtmlString("#" + EditorPrefs.GetString("WaveformOutlineColor"), out waveformOutlineColor);
            }
            else
            {
                waveformOutlineColor = DefaultOutlineColor;
            }

            if (EditorPrefs.HasKey("WaveformFillColor"))
            {
                ColorUtility.TryParseHtmlString("#" + EditorPrefs.GetString("WaveformFillColor"), out waveformFillColor);
            }
            else
            {
                waveformFillColor = DefaultFillColor;
            }
        }

        private void SaveSettings()
        {
            EditorPrefs.SetString("WaveformOutlineColor", ColorUtility.ToHtmlStringRGBA(waveformOutlineColor));
            EditorPrefs.SetString("WaveformFillColor", ColorUtility.ToHtmlStringRGBA(waveformFillColor));
        }

        public static Color WaveformOutlineColor => waveformOutlineColor;
        public static Color WaveformFillColor => waveformFillColor;

        private void DrawAudioSourcePlusSection()
        {
            GUILayout.BeginVertical("box");

            showAudioSourcePlusSection = EditorGUILayout.Foldout(showAudioSourcePlusSection, "Modify Audio Source Plus Components", true);
            if (showAudioSourcePlusSection)
            {
                EditorGUILayout.Space(10);
                DrawAudioSourcePlusList();

                if (GUILayout.Button("Add AudioSourcePlus", GUILayout.Height(25)))
                {
                    audioSourcePlusList.Add(null);
                }
                EditorGUILayout.Space(10);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Gather All AudioSourcePlus from Scene", GUILayout.Height(25)))
                {
                    GatherAllAudioSourcePlus();
                }
                if (GUILayout.Button("Revert listed to Audio Sources", GUILayout.Height(25)))
                {
                    RevertAudioSourcePlusComponents();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            EditorGUILayout.Space(20);
        }

        private void DrawAudioSourceSection()
        {
            GUILayout.BeginVertical("box");

            showAudioSourceSection = EditorGUILayout.Foldout(showAudioSourceSection, "Modify Audio Source Components", true);
            if (showAudioSourceSection)
            {
                EditorGUILayout.Space(10);
                DrawAudioSourceList();

                if (GUILayout.Button("Add AudioSource", GUILayout.Height(25)))
                {
                    audioSourceList.Add(null);
                }
                EditorGUILayout.Space(10);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Gather All AudioSources in Scene", GUILayout.Height(25)))
                {
                    GatherAllAudioSources();
                }
                if (GUILayout.Button("Convert All to AudioSourcePlus", GUILayout.Height(25)))
                {
                    ConvertAllToAudioSourcePlus();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            EditorGUILayout.Space(20);
        }

        private void DrawAudioSourcePlusList()
        {
            for (int i = 0; i < audioSourcePlusList.Count; i++)
            {
                GUILayout.BeginHorizontal();

                audioSourcePlusList[i] = (AudioSourcePlus)EditorGUILayout.ObjectField(
                    $"AudioSourcePlus {i + 1}",
                    audioSourcePlusList[i],
                    typeof(AudioSourcePlus),
                    true
                );

                if (audioSourcePlusList[i] != null && GUILayout.Button("Revert to AudioSource", GUILayout.Width(150)))
                {
                    RevertAudioSourcePlusComponent(audioSourcePlusList[i]);
                    audioSourcePlusList.RemoveAt(i);
                    i--;
                }

                if (GUILayout.Button("X", GUILayout.Width(30)))
                {
                    audioSourcePlusList.RemoveAt(i);
                    i--;
                }

                GUILayout.EndHorizontal();
            }
        }

        private void DrawAudioSourceList()
        {
            for (int i = 0; i < audioSourceList.Count; i++)
            {
                GUILayout.BeginHorizontal();

                audioSourceList[i] = (AudioSource)EditorGUILayout.ObjectField(
                    $"AudioSource {i + 1}",
                    audioSourceList[i],
                    typeof(AudioSource),
                    true
                );

                if (audioSourceList[i] != null && GUILayout.Button("Make AudioSourcePlus", GUILayout.Width(150)))
                {
                    ConvertToAudioSourcePlus(audioSourceList[i]);
                    audioSourceList.RemoveAt(i);
                    i--;
                }

                if (GUILayout.Button("X", GUILayout.Width(30)))
                {
                    audioSourceList.RemoveAt(i);
                    i--;
                }

                GUILayout.EndHorizontal();
            }
        }

        private void GatherAllAudioSourcePlus()
        {
            AudioSourcePlus[] audioSources = Object.FindObjectsByType<AudioSourcePlus>(FindObjectsSortMode.None);
            audioSourcePlusList.Clear();
            audioSourcePlusList.AddRange(audioSources);
        }

        private void GatherAllAudioSources()
        {
            AudioSource[] allAudioSources = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            audioSourceList.Clear();

            foreach (AudioSource audioSource in allAudioSources)
            {
                if (audioSource.GetComponent<AudioSourcePlus>() == null)
                {
                    audioSourceList.Add(audioSource);
                }
            }
        }

        private void ConvertToAudioSourcePlus(AudioSource audioSource)
        {
            if (audioSource != null)
            {
                Undo.RegisterCompleteObjectUndo(audioSource.gameObject, "Convert to AudioSourcePlus");

                AudioSourcePlus audioSourcePlus = audioSource.gameObject.AddComponent<AudioSourcePlus>();
                audioSourcePlus.audioSource = audioSource;

                EditorUtility.SetDirty(audioSourcePlus);
                audioSourcePlusList.Add(audioSourcePlus);
            }
        }

        private void ConvertAllToAudioSourcePlus()
        {
            for (int i = 0; i < audioSourceList.Count; i++)
            {
                ConvertToAudioSourcePlus(audioSourceList[i]);
            }
            audioSourceList.Clear();
        }

        private void RevertAudioSourcePlusComponent(AudioSourcePlus audioSourcePlus)
        {
            if (audioSourcePlus != null)
            {
                if (audioSourcePlus.audioSource != null)
                {
                    Undo.RegisterCompleteObjectUndo(audioSourcePlus.audioSource, "Revert AudioSourcePlus");
                    audioSourcePlus.audioSource.hideFlags = HideFlags.None;
                    EditorUtility.SetDirty(audioSourcePlus.audioSource);
                }

                Undo.RegisterCompleteObjectUndo(audioSourcePlus, "Revert AudioSourcePlus");
                audioSourcePlus.skipCustomDestruction = true;
                bool enabledState = audioSourcePlus.enabled;
                AudioSource audioSource = audioSourcePlus.audioSource;

                Undo.DestroyObjectImmediate(audioSourcePlus);
                audioSource.enabled = enabledState;
            }
        }

        private void RevertAudioSourcePlusComponents()
        {
            foreach (var audioSourcePlus in audioSourcePlusList)
            {
                RevertAudioSourcePlusComponent(audioSourcePlus);
            }
            audioSourcePlusList.Clear();
        }
    }
}
