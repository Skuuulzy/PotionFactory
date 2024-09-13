using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Relic Template", menuName = "Relic/Relic Template")]

public class RelicTemplate : ScriptableObject
{
	[SerializeField] private string _relicName;
	[SerializeField] private int _shopPrice;
	[SerializeField] private Sprite _uiView;
	[SerializeField] private float _spawnProbability = 0.25f;

	public string RelicName => _relicName;
	public int ShopPrice => _shopPrice;	
	public Sprite UIView => _uiView;
	public float SpawnProbability => _spawnProbability;
}
