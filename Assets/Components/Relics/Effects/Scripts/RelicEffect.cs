using Components.Machines.Behaviors;
using UnityEngine;


public class RelicEffect : ScriptableObject
{
	public RelicEffectType Type;
	public virtual void ApplyEffect(MachineBehavior behavior)
	{
	}
}

[CreateAssetMenu(fileName = "New Increase Machine Process Time Template", menuName = "Relic/Relic Effect")]
public class IncreaseMachineProcessTimeEffect : RelicEffect
{
	[SerializeField] private float _bonusProcessTime;
	public override void ApplyEffect(MachineBehavior behavior)
	{
		base.ApplyEffect(behavior);
		behavior.BonusProcessTime *= Mathf.RoundToInt(behavior.BonusProcessTime * _bonusProcessTime);
	}
}

public enum RelicEffectType
{
	MACHINE_RELIC_TYPE,
	SHOP_STATE_RELIC_TYPE
}