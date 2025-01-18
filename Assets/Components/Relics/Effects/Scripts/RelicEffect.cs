using Components.Machines.Behaviors;
using UnityEngine;


public class RelicEffect : ScriptableObject
{
	public RelicEffectType Type;
	public virtual void ApplyEffect(MachineBehavior behavior)
	{
	}
}

[CreateAssetMenu(fileName = "New Increase Machine Process Time Template", menuName = "Component/Relic/Relic Effect")]
public class IncreaseMachineProcessTimeEffect : RelicEffect
{
	[SerializeField] private float _bonusProcessTime;
	public override void ApplyEffect(MachineBehavior behavior)
	{
		base.ApplyEffect(behavior);
		if (!behavior.RelicEffects.Contains(this))
		{
			behavior.RelicEffectBonusProcessTime = Mathf.Clamp(behavior.RelicEffectBonusProcessTime + behavior.RelicEffectBonusProcessTime * _bonusProcessTime, 0, 1);
			behavior.RelicEffects.Add(this);
		}
	}
}

public enum RelicEffectType
{
	MACHINE_RELIC_TYPE,
	SHOP_STATE_RELIC_TYPE
}