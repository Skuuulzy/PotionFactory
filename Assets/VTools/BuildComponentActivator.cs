using UnityEngine;

public class BuildComponentActivator : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _componentToDisable;

    private void Awake()
    {
#if !UNITY_EDITOR
        if (!Debug.isDebugBuild)
        {
            _componentToDisable.enabled = false;
        }
#endif
    }
}