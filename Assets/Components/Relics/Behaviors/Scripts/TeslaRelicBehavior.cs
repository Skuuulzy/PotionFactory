using Components.Grid;
using Components.Machines;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Relics.Behavior
{
	[CreateAssetMenu(fileName = "New Relic Behaviour", menuName = "Relics/Behavior/TeslaRelicBehavior")]
	public class TeslaRelicBehavior : RelicBehavior
	{
		[SerializeField] private float _percentageEffect;

		public float PercentageEffect => _percentageEffect;

		public override void ApplyEffect()
		{
			
		}

		public override void ApplyEffect(List<Cell> cells)
		{
			
		}

		public override void ApplyEffect(List<Machine> machines)
		{
			foreach(Machine machine in machines)
			{
				machine.Behavior.ChangeProcessTime(_percentageEffect);
			}
		}
	}
}

