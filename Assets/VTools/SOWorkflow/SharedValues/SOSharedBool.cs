using UnityEngine;

namespace SoWorkflow.SharedValues
{
    [CreateAssetMenu(menuName = "Shared Value/Bool")]
    public class SOSharedBool : SOSharedValue<bool>
    {
        public bool IsTrue => Value;
        
        public void Inverse()
        {
            Set(!Value);
        }
    }
}