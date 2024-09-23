using System.Collections.Generic;
using UnityEngine;

namespace TelePresent.AudioSyncPro
{
    [ASP_ReactorCategory("Physics")]
    public class ASPR_AddImpulseOnBeats : MonoBehaviour, ASP_IAudioReaction
    {
        [HideInInspector]
        public new string name = "Add Impulse On Beats!";
        public string info = "Apply an impulse to Rigidbodies on audio beats (Play Mode Only).";

        public List<Rigidbody> targetRigidbodies;

        public Vector3 impulseIntensity = new Vector3(10f, 10f, 10f); // Adjust this value to control the impulse force

        [ASP_FloatSlider(0f, 10f)]
        public float sensitivity = 1f; // Adjust this value to control how sensitive the beat detection is

        [ASP_SpectrumDataSlider(0f, 1, 0f, .1f, "spectrumDataForSlider")]
        public Vector4 frequencyRangeAndThreshold = new Vector4(0f, .25f, 0, .04f); // Frequency range to analyze for beat detection

        private Dictionary<Rigidbody, Vector3> initialVelocities = new Dictionary<Rigidbody, Vector3>();
        private bool isInitialized = false;
        [HideInInspector]
        [SerializeField] private bool isActive = true;

        public bool IsActive
        {
            get => isActive;
            set => isActive = value;
        }
        private float[] spectrumDataForSlider = new float[512];
        private float lastBeatTime = 0f; // Time of the last beat
        public float beatCooldown = 0.25f; // Cooldown duration in seconds

        public void Initialize(Vector3 initialPosition, Vector3 initialScale, Quaternion initialRotation)
        {
            foreach (var rigidbody in targetRigidbodies)
            {
                if (rigidbody != null && !initialVelocities.ContainsKey(rigidbody))
                {
                    initialVelocities[rigidbody] = rigidbody.velocity;
                }
            }

            isInitialized = true;
        }

        public void React(AudioSourcePlus audioSourcePlus, Transform targetTransform, float rmsValue, float[] spectrumData)
        {
            if (!IsActive) return;

            if (targetTransform.TryGetComponent<Rigidbody>(out Rigidbody targetRigidbody) && !targetRigidbodies.Contains(targetRigidbody))
            {
                targetRigidbodies.Add(targetRigidbody);
                if (!initialVelocities.ContainsKey(targetRigidbody))
                {
                    initialVelocities[targetRigidbody] = targetRigidbody.velocity;
                }
            }

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
                foreach (var rigidbody in targetRigidbodies)
                {
                    if (rigidbody != null)
                    {
                        rigidbody.AddForce(impulseIntensity, ForceMode.Impulse); // Apply impulse force on beat
                    }
                }

                lastBeatTime = Time.time; // Update the last beat time
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

            foreach (var rigidbody in targetRigidbodies)
            {
                if (rigidbody != null && initialVelocities.ContainsKey(rigidbody))
                {
                    rigidbody.velocity = initialVelocities[rigidbody];
                }
            }
        }
    }
}
