using System.Collections.Generic;
using UnityEngine;

namespace TelePresent.AudioSyncPro
{
    [ASP_ReactorCategory("Transforms")]
    public class ASPR_RotateOnBeats : MonoBehaviour, ASP_IAudioReaction
    {
        [HideInInspector]
        public new string name = "Rotate On Beats!";
        [HideInInspector]
        public string info = "This Component adds a rotation offset whenever there's a beat!";

        public bool useTargetTransform = true;
        [HideInInspector]
        public List<Transform> affectedTransforms;
        public float rotationMultiplier = 10.0f; // Adjust this value to control the magnitude of the rotation offset
        [ASP_FloatSlider(0.01f, 20f)]
        public float resetSpeed = 3.5f; // Adjust this value to control how quickly the rotation resets on beat

        public bool RandomRotationDirection = true; // Toggle between random rotation and specified direction
        public Vector3 rotationDirection = Vector3.up; // Specified direction to rotate towards when RandomRotationDirection is off
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

        private Dictionary<Transform, Quaternion> initialRotations = new Dictionary<Transform, Quaternion>();
        private bool isInitialized = false;

        public void Initialize(Vector3 initialPosition, Vector3 initialScale, Quaternion initialRotation)
        {
            affectedTransforms ??= new List<Transform>(); // Initialize if null

            if (useTargetTransform)
            {
                affectedTransforms.Clear();
            }
            initialRotations.Clear();
            foreach (var transform in affectedTransforms)
            {
                if (transform != null && !initialRotations.ContainsKey(transform))
                {
                    initialRotations[transform] = transform.localRotation;
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
                if (!initialRotations.ContainsKey(targetTransform))
                {
                    initialRotations[targetTransform] = targetTransform.localRotation;
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

                    Vector3 direction = RandomRotationDirection ? Random.onUnitSphere : rotationDirection;
                    Quaternion rotationOffset = Quaternion.Euler(direction * rotationMultiplier);

                    transform.localRotation *= rotationOffset; // Apply the rotation offset
                }

                lastBeatTime = Time.time; // Update the last beat time
            }
            else
            {
                // Smoothly reset rotations to initial state
                foreach (var transform in affectedTransforms)
                {
                    if (transform == null || !initialRotations.ContainsKey(transform)) continue;

                    transform.localRotation = Quaternion.Lerp(transform.localRotation, initialRotations[transform], resetSpeed * Time.deltaTime);
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
                if (transform != null && initialRotations.ContainsKey(transform))
                {
                    transform.localRotation = initialRotations[transform];
                }
            }
        }
    }
}
