using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatisticView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _statText;

	private string _machineName;
	private int _value;
	private int _tickRate;

	public string MachineName => _machineName;
	public void Init(string name, int value, int tickRate)
	{
		_machineName = name;
		_value = value;
		_tickRate = tickRate;
		_statText.text = name + " x" + value + " tick rate = " + tickRate;
	}

	public void UpdateStatisticView(int value)
	{
		_statText.text = _machineName + " x" + value + " tick rate = " + _tickRate;
	}
}
