using Components.Ingredients;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Machines.Behaviors
{
    [CreateAssetMenu(fileName = "New Machine Behaviour", menuName = "Machines/Behavior/Destructor")]
    public class DestructorMachineBehaviour : MachineBehavior
    {
        public static Action<List<IngredientTemplate>> OnItemSold;
        public override void Process(Machine machine)
        {
            OnItemSold?.Invoke(machine.Ingredients);
            machine.RemoveAllItems();
        }
    }
}