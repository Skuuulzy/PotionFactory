using Components.Relics.Behavior;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Relic Template", menuName = "Relic/Relic Template")]

public class RelicTemplate : ScriptableObject
{
	[SerializeField] private string _relicName;
	[SerializeField] private int _shopPrice;
	[SerializeField] private GameObject _gridView;
	[SerializeField] private Sprite _uiView;
	[SerializeField] private float _spawnProbability = 0.25f;
	[SerializeField] private int _effectZoneRadius = 5;
	[SerializeField] private RelicBehavior _relicBehaviors;

	public string RelicName => _relicName;
	public int ShopPrice => _shopPrice;	
	public GameObject GridView => _gridView;
	public Sprite UIView => _uiView;
	public float SpawnProbability => _spawnProbability;
	public int EffectZoneRadius => _effectZoneRadius;
	public RelicBehavior RelicBehavior => _relicBehaviors;
}
