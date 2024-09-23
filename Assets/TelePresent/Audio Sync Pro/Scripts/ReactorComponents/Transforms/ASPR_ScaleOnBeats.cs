using System.Collections.Generic;
using UnityEngine;

namespace TelePresent.AudioSyncPro
{
    [ASP_ReactorCategory("Transforms")]
    public class ASPR_ScaleOnBeats : MonoBehaviour, ASP_IAudioReaction
    {
        public new string name = "Scale On Beats!";
        public string info = "Scale Transforms by a multiplier every Beat!";

        public bool useTargetTransform = true;
        [HideInInspector]
        public List<Transform> affectedTransforms;

        [ASP_ScaleVector3]
        public Vector3 scaleMultiplier = new Vector3(1.5f, 1.5f, 1.5f); // Adjust this value to control how much the transform scales on a beat

        [ASP_FloatSlider(0f, 10f)]
        public float sensitivity = 1f; // Adjust this value to control how sensitive the beat detection is

        [ASP_SpectrumDataSlider(0f, 1, 0f, .1f, "spectrumDataForSlider")]
        public Vector4 frequencyRangeAndThreshold = new Vector4(0f, .25f, 0, .04f); // Frequency range to analyze for beat detection

        private Dictionary<Transform, Vector3> initialScales = new Dictionary<Transform, Vector3>();
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
            affectedTransforms ??= new List<Transform>(); // Initialize if null
            if (useTargetTransform)
            {
                affectedTransforms.Clear();
            }
            initialScales.Clear();
            foreach (var transform in affectedTransforms)
            {
                if (transform != null && !initialScales.ContainsKey(transform))
                {
                    initialScales[transform] = transform.localScale;
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
                if (!initialScales.ContainsKey(targetTransform))
                {
                    initialScales[targetTransform] = targetTransform.localScale;
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

            if (averageSpectrumInWindow > frequencyRangeAndThreshold.w)
            {
                foreach (var transform in affectedTransforms)
                {
                    if (transform != null && initialScales.ContainsKey(transform))
                    {
                        Vector3 initialScale = initialScales[transform];
                        Vector3 beatScale = Vector3.Scale(initialScale, scaleMultiplier);

                        transform.localScale = Vector3.Lerp(transform.localScale, beatScale, 25f * Time.deltaTime);
                    }
                }

                lastBeatTime = Time.time; // Update the last beat time
            }
            else
            {
                foreach (var transform in affectedTransforms)
                {
                    if (transform != null && initialScales.ContainsKey(transform))
                    {
                        transform.localScale = Vector3.Lerp(transform.localScale, initialScales[transform], 10f * Time.deltaTime);
                    }
                }
            }
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
                if (transform != null && initialScales.ContainsKey(transform))
                {
                    transform.localScale = initialScales[transform];
                }
            }
        }
    }
}
