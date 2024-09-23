using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace TelePresent.AudioSyncPro
{
    [ASP_ReactorCategory("Animation")]
    public class ASPR_PlayAnimationOnBeat : MonoBehaviour, ASP_IAudioReaction
    {
        [HideInInspector]
        public new string name = "Play Animation On Beats!";
        public string info = "Plays an animation from an Animator component every time a beat is detected (Play Mode Only).";

        public Animator animator;
        [SerializeField]
        private string selectedAnimationState;
        [SerializeField]
        private bool mustFinishBeforeRetrigger = true;

        private List<string> animatorStateNames;

        [ASP_FloatSlider(0f, 10f)]
        public float sensitivity = 1f;

        [ASP_SpectrumDataSlider(0f, 1f, 0f, 0.1f, "spectrumDataForSlider")]
        public Vector4 frequencyRangeAndThreshold = new Vector4(0f, .25f, 0, .04f);
        [HideInInspector]
        [SerializeField] private bool isActive = true;

        public bool IsActive
        {
            get => isActive;
            set => isActive = value;
        }
        private float[] spectrumDataForSlider = new float[512];
        private float lastBeatTime = 0f;
        public float beatCooldown = 0.25f;

        private void OnValidate()
        {
            if (animator != null)
            {
#if UNITY_EDITOR
                UpdateAnimatorStateNames();
#endif
            }
        }

#if UNITY_EDITOR
        private void UpdateAnimatorStateNames()
        {
            animatorStateNames = new List<string>();

            if (animator == null || animator.runtimeAnimatorController == null)
            {
                return;
            }

            AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
            if (animatorController != null)
            {
                foreach (AnimatorControllerLayer layer in animatorController.layers)
                {
                    foreach (ChildAnimatorState state in layer.stateMachine.states)
                    {
                        animatorStateNames.Add(state.state.name);
                    }
                }
            }
        }
#endif

        public void Initialize(Vector3 initialPosition, Vector3 initialScale, Quaternion initialRotation) { }

        public void React(AudioSourcePlus audioSourcePlus, Transform targetTransform, float rmsValue, float[] spectrumData)
        {
            if (!IsActive || animator == null) return;

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
                PlayAnimationState(selectedAnimationState);
            }
        }

        private void PlayAnimationState(string stateName)
        {
            if (string.IsNullOrEmpty(stateName))
            {
                Debug.LogWarning("No animation state name provided.");
                return;
            }

            int layerIndex = GetLayerIndexContainingState(stateName);

            if (layerIndex == -1)
            {
                Debug.LogWarning($"Animator: State '{stateName}' could not be found in any layer.");
                return;
            }

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            if (mustFinishBeforeRetrigger)
            {
                if (!stateInfo.IsName(stateName) || stateInfo.normalizedTime >= 1f)
                {
                    animator.Play(stateName, layerIndex);
                    lastBeatTime = Time.time;
                }
            }
            else
            {
                animator.Play(stateName, layerIndex, 0f);
                lastBeatTime = Time.time;
            }
        }

        private int GetLayerIndexContainingState(string stateName)
        {
#if UNITY_EDITOR
            AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
            if (animatorController != null)
            {
                for (int i = 0; i < animatorController.layers.Length; i++)
                {
                    foreach (ChildAnimatorState state in animatorController.layers[i].stateMachine.states)
                    {
                        if (state.state.name == stateName)
                        {
                            return i;
                        }
                    }
                }
            }
#else
            for (int i = 0; i < animator.layerCount; i++)
            {
                if (animator.HasState(i, Animator.StringToHash(stateName)))
                {
                    return i;
                }
            }
#endif
            return -1;
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

        public void ResetToOriginalState(Transform targetTransform) { }

        public List<string> GetAnimatorStateNames()
        {
#if UNITY_EDITOR
            if (animatorStateNames == null)
            {
                UpdateAnimatorStateNames();
            }
            return animatorStateNames;
#else
            return new List<string>();
#endif
        }
    }
}
