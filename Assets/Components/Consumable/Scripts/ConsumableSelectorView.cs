using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsumableSelectorView : MonoBehaviour
{
	[SerializeField] private TMP_Text _name;
	[SerializeField] private Image _uiView;

	public static Action<ConsumableTemplate> OnSelected;

	private ConsumableTemplate _consumable;

	public ConsumableTemplate Consumable => _consumable;

	public void Init(ConsumableTemplate consumable, int value = 1)
	{
		_consumable = consumable;
		_name.text = consumable.ConsumableName;
		_uiView.sprite = consumable.UIView;
	}

	public void Select()
	{
		OnSelected?.Invoke(_consumable);
	}
}
