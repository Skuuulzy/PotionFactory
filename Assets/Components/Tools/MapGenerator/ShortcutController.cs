using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShortcutController : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _valueText;
    public void ChangeValue(float value)
	{
		_valueText.text = $"{value.ToString("F2")}";
	}
	
}
