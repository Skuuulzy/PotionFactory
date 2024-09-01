using UnityEngine;

namespace Components.Machines.UIView
{
    public class MachineUIViewController : MonoBehaviour
    {
        [SerializeField] private SerializableDictionary<MachineType, MachineUIViewBase> _machineViewPrefabs;
        [SerializeField] private MachineUIViewBase _machineViewBasePrefab;
        [SerializeField] private Transform _holder;
        
        private MachineUIViewBase _currentView;
        
        private void Awake()
        {
            MachineController.OnMachineClicked += HandleMachineClicked;
        }

        private void OnDestroy()
        {
            MachineController.OnMachineClicked -= HandleMachineClicked;
        }

        private void HandleMachineClicked(Machine machine)
        {
            var machineType = machine.Template.Type;

            switch (machineType)
            {
                case MachineType.EXTRACTOR:
                    InstantiateAndInitializeView<ExtractorUIView>(machine);
                    break;
                case MachineType.CAULDRON:
                case MachineType.CONVEYOR:
                case MachineType.DESTRUCTOR:
                case MachineType.DISPANCER:
                case MachineType.DISTILLER:
                case MachineType.MIXER:
                case MachineType.PRESS:
                    InstantiateAndInitializeView(machine);
                    break;
            }
        }
        
        private void InstantiateAndInitializeView(Machine machine)
        {
            if (_currentView)
            {
                Destroy(_currentView.gameObject);
            }
            
            _currentView = Instantiate(_machineViewBasePrefab, _holder);
            _currentView.Initialize(machine);
        }

        private void InstantiateAndInitializeView<T>(Machine machine) where T : MachineUIViewBase
        {
            if (_currentView && _currentView is T desiredTypeView)
            {
                desiredTypeView.Initialize(machine);
                return;
            }
            
            _currentView = Instantiate(_machineViewPrefabs[machine.Template.Type], _holder);
            ((T)_currentView).Initialize(machine);
        }
    }
}