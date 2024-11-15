using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Template", menuName = "Consumable/Consumable Template")]

public class ConsumableTemplate : ScriptableObject
{
    [SerializeField] private string _consumableName;
    [SerializeField] private int _shopPrice;
	[SerializeField] private GameObject _gridView;
	[SerializeField] private Sprite _uiView;
    [SerializeField] private float _spawnProbability;


	public string ConsumableName => _consumableName;
    public int ShopPrice => _shopPrice;
	public GameObject GridView => _gridView;
	public Sprite UIView => _uiView;
    public float SpawnProbability => _spawnProbability;


}
