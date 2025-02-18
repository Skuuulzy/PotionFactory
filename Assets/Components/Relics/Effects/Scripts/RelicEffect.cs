using Components.Machines.Behaviors;
using UnityEngine;


public class RelicEffect : ScriptableObject
{
	public RelicEffectType Type;
	public virtual void ApplyEffect(MachineBehavior behavior)
	{
		behavior.AddRelicEffect(this);
	}
}

[CreateAssetMenu(fileName = "New Increase Machine Process Time Template", menuName = "Component/Relic/Relic Effect")]
public class IncreaseMachineProcessTimeEffect : RelicEffect
{
	[SerializeField] private float _bonusProcessTime;
	public override void ApplyEffect(MachineBehavior behavior)
	{
		base.ApplyEffect(behavior);
	}
}

public enum RelicEffectType
{
	MACHINE_RELIC_TYPE,
	SHOP_STATE_RELIC_TYPE
}