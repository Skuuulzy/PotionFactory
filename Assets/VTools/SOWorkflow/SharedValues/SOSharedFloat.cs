using UnityEngine;

namespace SOWorkflow.SharedValues
{
    [CreateAssetMenu(menuName = "Shared Value/Float")]
    public class SOSharedFloat : SOSharedValue<float>
    {
        public void Increment(float increment)
        {
            Set(Value + increment);
        }
    }
}