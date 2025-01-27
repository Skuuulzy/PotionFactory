using UnityEngine;
using UnityEngine.Events;

namespace SOWorkflow.EventSystem
{
    /// <summary>
    /// A MonoBehaviour component that listens to a specific `SOChannel` and invokes Unity events in response.
    /// </summary>
    public class SOChannelListener : MonoBehaviour
    {
        [SerializeField] protected SOChannel _soChannel;
        [Space] 
        [SerializeField] protected UnityEvent _callback;

        public SOChannel SOChannel => _soChannel;

        private void Awake()
        {
            if (!_soChannel)
            {
                Debug.LogError($"[SOWorkflow] Channel listener on {name} has no channel to observe, please assign one or remove the component.");
                return;
            }

            _soChannel.Register(this);
        }

        private void OnDestroy()
        {
            if (!_soChannel)
            {
                return;
            }

            _soChannel.Deregister(this);
        }

        public virtual void Raise()
        {
            _callback?.Invoke();
        }
    }
}