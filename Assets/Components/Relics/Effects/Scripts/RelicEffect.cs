using Components.Machines.Behaviors;
using UnityEngine;

public class RelicEffect : ScriptableObject
{
	protected virtual void ApplyEffect(MachineBehavior behavior)
	{
	}
}

public class IncreaseMachineSpeedEffect : RelicEffect
{
	protected override void ApplyEffect(MachineBehavior behavior)
	{
		base.ApplyEffect(behavior);
		behavior.BonusProcessTime *= Mathf.RoundToInt(behavior.BonusProcessTime * 1.1f);
	}
}
