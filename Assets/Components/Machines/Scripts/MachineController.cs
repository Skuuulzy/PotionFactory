using System.Collections.Generic;
using Components.Ingredients;
using Components.Tick;
using UnityEngine;
using Components.Machines.Behaviors;

namespace Components.Machines
{
    public class MachineController : MonoBehaviour
    {
        [SerializeField] private Transform _3dViewHolder;
        [SerializeField] private Machine _machine;
        [SerializeField] private IngredientController _ingredientController;

        [SerializeField] private GameObject _inPreview;
        [SerializeField] private GameObject _outPreview;
        
        public Machine Machine => _machine;

        private List<GameObject> _previewObjects;
        
        private bool _initialized;
        private GameObject _view;

        private int _outMachineTickCount;
        
        // ------------------------------------------------------------------------- INIT -------------------------------------------------------------------------
        public void InstantiatePreview(MachineTemplate machineTemplate, float scale)
        {
            _view = Instantiate(machineTemplate.GridView, _3dViewHolder);
            _machine = new Machine(machineTemplate, this);
            _view.transform.localScale = new Vector3(scale, scale, scale);

            SetupDirectionalArrows(machineTemplate);
        }

        private void SetupDirectionalArrows(MachineTemplate machineTemplate)
        {
            _previewObjects = new List<GameObject>();
            
            foreach (var node in machineTemplate.Nodes)
            {
                foreach (var port in node.Ports)
                {
                    var previewArrow = Instantiate(port.Way == Way.IN ? _inPreview : _outPreview, _view.transform);
                    
                    previewArrow.transform.localPosition = new Vector3(node.LocalPosition.x, previewArrow.transform.position.y, node.LocalPosition.y);
                    
                    // TODO: Find why we need to invert the angle when the machine is only 1x1 (especially with curved conveyor).
                    previewArrow.transform.Rotate(Vector3.up, port.Side.AngleFromSide(machineTemplate.Nodes.Count == 1));
                    
                    _previewObjects.Add(previewArrow);
                }
            }
        }
        
        public void RotatePreview(int angle)
        {
            _view.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
            _machine.UpdateNodesRotation(angle);
        }
        
        public void ConfirmPlacement()
        {
            foreach (var previewObject in _previewObjects)
            {
                Destroy(previewObject);
            }
            
            _previewObjects.Clear();
            
            _initialized = true;
            
            _machine.OnTick += Tick;
            _machine.OnPropagateTick += PropagateTick;
            _machine.OnItemAdded += ShowItem;
            _machine.Behavior.SetInitialProcessTime(_machine.Template.ProcessTime);
            _machine.LinkNodeData();


            if(_machine.Behavior is DestructorMachineBehaviour destructor)
            {
                destructor.OnSpecialIngredientChanged += ShowItem;
			}

            AddMachineToChain();
        }

        // ------------------------------------------------------------------------- DESTROY -------------------------------------------------------------------------
        private void OnDestroy()
        {
            if (!_initialized)
            {
                return;
            }
            
            RemoveMachineFromChain();
            
            _machine.OnTick -= Tick;
            _machine.OnPropagateTick -= PropagateTick;
            _machine.OnItemAdded -= ShowItem;
        }

        // ------------------------------------------------------------------------- TICK -------------------------------------------------------------------------
        // Base tick called by the tick system.
        private void Tick()
        {
            _machine.Behavior.Process(_machine);
            _machine.Behavior.TryGiveOutIngredient(_machine);
            
            // Propagate tick
            if (_machine.TryGetInMachine(out List<Machine> previousMachines))
            {
                foreach (var previousMachine in previousMachines)
                {
                    previousMachine.PropagateTick();
                }
            }
        }

        // Tick propagation called by the next machine.
        private void PropagateTick()
        {
            _outMachineTickCount++;

            if (!_machine.TryGetOutMachines(out var connectedMachines))
            {
                return;            
            }

            // The machine has not received the propagation of all his next machine.
            if (_outMachineTickCount < connectedMachines.Count)
            {
                return;
            }
            
            _machine.Behavior.Process(_machine);
            _machine.Behavior.TryGiveOutIngredient(_machine);

            // Propagate tick
            if (!_machine.TryGetInMachine(out List<Machine> previousMachines))
            {
                return;
            }
            
            foreach (var previousMachine in previousMachines)
            {
                previousMachine.PropagateTick();
            }

            _outMachineTickCount = 0;
        }

        // ------------------------------------------------------------------------- CHAIN -------------------------------------------------------------------------
        private void AddMachineToChain()
        {
            bool hasInMachine = _machine.TryGetInMachine(out List<Machine> inMachines);
            bool hasOutMachine = _machine.TryGetOutMachines(out _);

            // The machine is not connected to any chain, create a new one.
            if (!hasInMachine && !hasOutMachine)
            {
                TickSystem.AddTickable(_machine);
            }
            // The machine only has an IN, it is now the end of the chain.
            if (hasInMachine && !hasOutMachine)
            {
                foreach (var inMachine in inMachines)
                {
                    TickSystem.ReplaceTickable(inMachine, _machine);
                }
            }
            // The machine has an IN and an OUT, it makes a link between two existing chains,
            // remove the IN tickable since the out chain already has a tickable.
            if (hasInMachine && hasOutMachine)
            {
                foreach (var inMachine in inMachines)
                {
                    TickSystem.RemoveTickable(inMachine);
                }
            }
        }

        private void RemoveMachineFromChain()
        {
            bool hasInMachine = _machine.TryGetInMachine(out List<Machine> inMachines);
            bool hasOutMachine = _machine.TryGetOutMachines(out _);
            
            // The machine is not connected to any chain, create a new one.
            if (!hasInMachine && !hasOutMachine)
            {
                TickSystem.RemoveTickable(_machine);
            }
            // The machine only has an IN, it is now the end of the chain.
            if (hasInMachine && !hasOutMachine)
            {
                foreach (var inMachine in inMachines)
                {
                    TickSystem.ReplaceTickable(_machine, inMachine);
                }
            }
            // The machine has an IN and an OUT, it makes a link between two existing chains,
            // remove the IN tickable since the out chain already has a tickable.
            if (hasInMachine && hasOutMachine)
            {
                foreach (var inMachine in inMachines)
                {
                    TickSystem.AddTickable(inMachine);
                }
            }
        }

        // ------------------------------------------------------------------------- ITEM -------------------------------------------------------------------------
        // TODO: The ingredient should not be controlled by the machine but need to be independent and linked to a machine.
        private void ShowItem(bool show)
        {
            if (show)
            {
                _ingredientController.CreateRepresentationFromTemplate(_machine.InIngredients);
            }
            else
            {
                _ingredientController.DestroyRepresentation();
            }
        }

        //TO Change : special for destructor behavior
        private void ShowItem(IngredientTemplate ingredient)
        {
			_ingredientController.CreateFavoriteSellerItemRepresentationFromTemplate(ingredient);
		}
    }
}