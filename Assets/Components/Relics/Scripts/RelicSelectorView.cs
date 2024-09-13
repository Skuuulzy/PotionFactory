using Components.Inventory;
using Components.Machines;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicSelectorView : MonoBehaviour
{
	[SerializeField] private TMP_Text _name;
	[SerializeField] private Image _background;

	public static Action<RelicTemplate> OnSelected;

	private RelicTemplate _relic;

	public RelicTemplate Relic => _relic;

	public void Init(RelicTemplate relic, int value = 1)
	{
		_relic = relic;
		_name.text = relic.RelicName;
		_background.sprite = relic.UIView;
	}

	public void Select()
	{
		OnSelected?.Invoke(_relic);
	}
}
