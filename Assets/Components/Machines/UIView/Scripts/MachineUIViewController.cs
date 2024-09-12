using UnityEngine;

namespace Components.Machines.UIView
{
    public class MachineUIViewController : MonoBehaviour
    {
        [SerializeField] private SerializableDictionary<MachineType, MachineContextualUIView> _specialMachineViewPrefabs;
        [SerializeField] private MachineContextualUIView _machineContextualViewPrefab;
        [SerializeField] private Transform _holder;
        
        private MachineContextualUIView _currentView;
        
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
            InstantiateAndInitializeView(machine);
        }
        
        private void InstantiateAndInitializeView(Machine machine)
        {
            if (_currentView)
            {
                Destroy(_currentView.gameObject);
            }
            
            // Potential special views
            switch (machine.Template.Type)
            {
                case MachineType.EXTRACTOR:
                    _currentView = Instantiate(_specialMachineViewPrefabs[MachineType.EXTRACTOR], _holder);
                    break;
                default:
                    _currentView = Instantiate(_machineContextualViewPrefab, _holder);
                    break;
            }
            
            // Contextual base data
            _currentView.Initialize(machine);

            // Potential special views
            switch (machine.Template.Type)
            {
                case MachineType.EXTRACTOR:
                    if (_currentView.TryGetComponent(out ExtractorContextualUIView extractorView))
                    {
                        extractorView.Initialize(machine);
                    }
                    break;
            }

            _currentView.gameObject.SetActive(true);
        }
    }
}