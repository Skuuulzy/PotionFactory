using Components.Machines.Behaviors;
using UnityEngine;

public class RelicEffect : ScriptableObject
{
	public RelicEffectType Type;
	public virtual void ApplyEffect(MachineBehavior behavior)
	{
	}
}

public class IncreaseMachineSpeedEffect : RelicEffect
{
	public override void ApplyEffect(MachineBehavior behavior)
	{
		base.ApplyEffect(behavior);
		behavior.BonusProcessTime *= Mathf.RoundToInt(behavior.BonusProcessTime * 1.1f);
	}
}

public enum RelicEffectType
{
	MACHINE_RELIC_TYPE,
	SHOP_STATE_RELIC_TYPE
}