using UnityEngine;

namespace Components.Machines.UIView
{
    public class MachineUIContextualViewController : MonoBehaviour
    {
        [SerializeField] private MachineContextualUIView _machineContextualViewPrefab;
        [SerializeField] private Transform _holder;
        
        private MachineContextualUIView _currentView;
        
        private void Awake()
        {
            Machine.OnSelected += HandleMachineClicked;
        }

        private void OnDestroy()
        {
            Machine.OnSelected -= HandleMachineClicked;
        }
        
        private void HandleMachineClicked(Machine machine)
        {
            if (_currentView)
            {
                Destroy(_currentView.gameObject);
            }

            _currentView = Instantiate(_machineContextualViewPrefab, _holder);
            
            _currentView.Initialize(machine);
            _currentView.AddComponents(machine.Template.ContextualComponents);
            
            _currentView.gameObject.SetActive(true);
        }
    }
}