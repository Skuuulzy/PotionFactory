using UnityEngine;

namespace Components.Interactions.Clickable
{
    [RequireComponent(typeof(Collider))]
    public class ClickableComponent : MonoBehaviour
    {
        private IClickable _clickable;

        private void Start()
        {
            if (TryGetComponent(out IClickable clickableComponent))
            {
                _clickable = clickableComponent;
            }
            else
            {
                Debug.LogError($"No mono behaviour implementing IClickable found on {gameObject.name}. Add one or remove the Clickable component.");
                Destroy(this);
                return;
            }
            
            if (!TryGetComponent(out Collider _))
            {
                Debug.LogError($"No collider component found on {gameObject.name}. Add one or remove the Clickable component.");
                Destroy(this);
            }
        }
        
        private void OnMouseDown()
        {
            _clickable.Clicked();
        }
    }
}