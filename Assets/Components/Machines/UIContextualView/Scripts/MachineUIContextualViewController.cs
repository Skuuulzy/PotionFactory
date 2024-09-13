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
            InstantiateAndInitializeBaseView();
            MachineController.OnMachineClicked += HandleMachineClicked;
        }

        private void OnDestroy()
        {
            MachineController.OnMachineClicked -= HandleMachineClicked;
        }

        private void InstantiateAndInitializeBaseView()
        {
            if (_currentView)
            {
                Destroy(_currentView.gameObject);
            }

            _currentView = Instantiate(_machineContextualViewPrefab, _holder);
            _currentView.gameObject.SetActive(false);
        }
        
        private void HandleMachineClicked(Machine machine)
        {
            _currentView.Initialize(machine);
            _currentView.AddComponents(machine.Template.ContextualComponents);
            _currentView.gameObject.SetActive(true);
        }
    }
}