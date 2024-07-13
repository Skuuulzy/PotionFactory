using System;
using Components.Machines;
using UnityEngine;

namespace Components.Grid
{
    [Serializable]
    public class Cell
    {
        public int X { get; }
        public int Y { get; }
        public float Size { get; }
        public bool ContainsObject { get; private set; }
        public bool ContainsObstacle { get; private set; }

        [SerializeField] private MachineController _machineController;
		[SerializeField] private GameObject _obstacle; 
       
        public MachineController MachineController => _machineController;
        public GameObject Obstacle => _obstacle;  

        public Cell(int x, int y, float size, bool containsObject)
        {
            X = x;
            Y = y;
            Size = size;
            ContainsObject = containsObject;
        }

        public void AddMachineToCell(MachineController machineController)
        {
            _machineController = machineController;
            ContainsObject = true;
        }

        public void RemoveMachineFromCell()
        {
            _machineController = null;
            ContainsObject = false;
        }

        public void AddObstacleToCell(GameObject obstacle)
        {
            _obstacle = obstacle;
            ContainsObject = true;
			ContainsObstacle = true;
        }

        public void RemoveObstacleFromCell()
        {
            _obstacle = null;
            ContainsObject = false;
			ContainsObstacle = false;
        }

    }
}