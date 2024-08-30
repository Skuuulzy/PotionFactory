using System;
using UnityEngine;

namespace Components.Machines.UIView
{
    public class MachineUIViewController : MonoBehaviour
    {
        [SerializeField] private SerializableDictionary<MachineType, MachineUIViewBase> _machineViewPrefabs;
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
            
            if (!_currentView || _currentView.AssociatedType != machineType)
            {
                _currentView = Instantiate(_machineViewPrefabs[machineType], _holder);
            }

            switch (machineType)
            {
                case MachineType.EXTRACTOR:
                    if (_currentView is ExtractorUIView extractorUIView)
                    {
                        extractorUIView.Initialize(machine);
                        break;
                    }
                    Debug.LogError($"The clicked machine is type: {machineType}, yet the instanced view is not of type: {nameof(ExtractorUIView)}");
                    break;
                case MachineType.CAULDRON:
                case MachineType.CONVEYOR:
                case MachineType.DESTRUCTOR:
                case MachineType.DISPANCER:
                case MachineType.DISTILLER:
                case MachineType.MIXER:
                case MachineType.PRESS:
                    _currentView.Initialize(machine);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}