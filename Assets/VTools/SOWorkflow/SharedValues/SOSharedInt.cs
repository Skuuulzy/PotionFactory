using UnityEngine;

namespace SoWorkflow.SharedValues
{
    [CreateAssetMenu(menuName = "Shared Value/Int")]
    public class SOSharedInt : SOSharedValue<int>
    {
        public void Increment(int increment)
        {
            Set(Value + increment);
        }
    }
}
