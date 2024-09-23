using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TelePresent.AudioSyncPro
{
    [ASP_ReactorCategory("Events")]
    public class ASPR_CallEventOnBeat : MonoBehaviour, ASP_IAudioReaction
    {
        [HideInInspector]
        public new string name = "Call Event On Beats!";
        public string info = "Calls a UnityEvent every time a beat is detected (Play Mode Only).";

        [SerializeField]
        private UnityEvent onBeatDetected;  // UnityEvent to invoke on each beat

        [ASP_FloatSlider(0f, 10f)]
        public float sensitivity = 1f;  // Sensitivity for beat detection

        [ASP_SpectrumDataSlider(0f, 1f, 0f, 0.1f, "spectrumDataForSlider")]
        public Vector4 frequencyRangeAndThreshold = new Vector4(0f, .25f, 0, .04f);  // Frequency range and threshold for beat detection
        [HideInInspector]
        [SerializeField] private bool isActive = true;

        public bool IsActive
        {
            get => isActive;
            set => isActive = value;
        }
        private float[] spectrumDataForSlider = new float[512];
        private float lastBeatTime = 0f;  // Time of the last beat
        public float beatCooldown = 0.25f;  // Cooldown duration in seconds

        public void Initialize(Vector3 initialPosition, Vector3 initialScale, Quaternion initialRotation)
        {
            // No setup needed
        }

        public void React(AudioSourcePlus audioSourcePlus, Transform targetTransform, float rmsValue, float[] spectrumData)
        {
            if (!IsActive) return;

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

            if (!BeatCooldownActive() && averageSpectrumInWindow > frequencyRangeAndThreshold.w)
            {
                TriggerEventOnBeat();
            }
        }

        private void TriggerEventOnBeat()
        {
            if (onBeatDetected != null)
            {
                    onBeatDetected.Invoke();  // Invoke the UnityEvent
                    lastBeatTime = Time.time;  // Update the last beat time
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
            // No reset needed
        }
    }
}
