using System.Collections.Generic;
using UnityEngine;

namespace TelePresent.AudioSyncPro
{
    [ASP_ReactorCategory("Transforms")]
    [ExecuteInEditMode] 
    public class ASPR_MoveOnBeats : MonoBehaviour, ASP_IAudioReaction
    {
        [HideInInspector]
        public new string name = "Move On Beats!";
        [HideInInspector]
        public string info = "This Component moves the transform in a specified direction whenever there's a beat!";

        public bool useTargetTransform = true;
        [HideInInspector]
        public List<Transform> affectedTransforms;

        [SerializeField]
        private Vector3 moveOffset = Vector3.up; // The direction and amount to move on a beat
        [ASP_FloatSlider(0.01f, 20f)]
        public float resetSpeed = 5f; // Speed to reset position back to original

        [ASP_FloatSlider(0.0f, 15f)]
        public float sensitivity = 1.0f; // Sensitivity slider
        [ASP_SpectrumDataSlider(0f, 1f, 0f, 0.1f, "spectrumDataForSlider")]
        public Vector4 frequencyRangeAndThreshold = new Vector4(0f, .1f, 0f, .06f); // Frequency range to analyze for beat detection

        [HideInInspector]
        [SerializeField] private bool isActive = true;

        public bool IsActive
        {
            get => isActive;
            set => isActive = value;
        }
        [HideInInspector]
        private float[] spectrumDataForSlider = new float[512];
        private float lastBeatTime = 0f; // Time of the last beat
        public float beatCooldown = 0.25f; // Cooldown duration in seconds

        private Dictionary<Transform, Vector3> initialPositions = new Dictionary<Transform, Vector3>();
        private bool isInitialized = false;

        public void Initialize(Vector3 initialPosition, Vector3 initialScale, Quaternion initialRotation)
        {
            affectedTransforms ??= new List<Transform>(); // Initialize if null

            if (useTargetTransform)
            {
                affectedTransforms.Clear();
            }
            initialPositions.Clear();
            foreach (var transform in affectedTransforms)
            {
                if (transform != null && !initialPositions.ContainsKey(transform))
                {
                    initialPositions[transform] = transform.localPosition;
                }
            }

            isInitialized = true;
        }

        public void React(AudioSourcePlus audioSourcePlus, Transform targetTransform, float rmsValue, float[] spectrumData)
        {
            if (!IsActive) return;

            if (useTargetTransform && !affectedTransforms.Contains(targetTransform))
            {
                affectedTransforms.Add(targetTransform);
                if (!initialPositions.ContainsKey(targetTransform))
                {
                    initialPositions[targetTransform] = targetTransform.localPosition;
                }
            }

            if (!isInitialized) return;

            UpdateSpectrumData(spectrumData);

            int minFrequencyIndex = Mathf.Clamp(Mathf.FloorToInt(frequencyRangeAndThreshold.x * (spectrumData.Length - 1)), 0, spectrumData.Length - 1);
            int maxFrequencyIndex = Mathf.Clamp(Mathf.CeilToInt(frequencyRangeAndThreshold.y * (spectrumData.Length - 1)), minFrequencyIndex, spectrumData.Length - 1);

            float averageSpectrumInWindow = 0f;
            int count = 0;

            for (int i = minFrequencyIndex; i <= maxFrequencyIndex; i++)
            {
                averageSpectrumInWindow += spectrumData[i];
                count++;
            }

            averageSpectrumInWindow *= sensitivity / count;
            frequencyRangeAndThreshold.z = averageSpectrumInWindow;

            if (!BeatCooldownActive() && frequencyRangeAndThreshold.z > frequencyRangeAndThreshold.w)
            {
                foreach (var transform in affectedTransforms)
                {
                    if (transform == null) continue;

                    Vector3 beatPosition = initialPositions[transform] + moveOffset;

                    transform.localPosition = beatPosition;
                }

                lastBeatTime = Time.time; // Update the last beat time
            }
            else
            {
                // Smoothly reset positions to initial state
                foreach (var transform in affectedTransforms)
                {
                    if (transform == null || !initialPositions.ContainsKey(transform)) continue;

                    transform.localPosition = Vector3.Lerp(transform.localPosition, initialPositions[transform], resetSpeed * Time.deltaTime);
                }
            }
        }

        bool BeatCooldownActive()
        {
            return Time.time - lastBeatTime < beatCooldown;
        }

        private void UpdateSpectrumData(float[] spectrumData)
        {
            for (int i = 0; i < spectrumDataForSlider.Length; i++)
            {
                spectrumDataForSlider[i] = i < spectrumData.Length ? spectrumData[i] : 0f;
            }
        }

        public void ResetToOriginalState(Transform targetTransform)
        {
            if (!isInitialized) return;

            foreach (var transform in affectedTransforms)
            {
                if (transform != null && initialPositions.ContainsKey(transform))
                {
                    transform.localPosition = initialPositions[transform];
                }
            }
        }
    }
}
