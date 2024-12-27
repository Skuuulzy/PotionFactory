using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VComponent.Tools.EventSystem;

public class ShortcutController : MonoBehaviour
{
	[SerializeField] private FloatEventChannel _valueEventChannel;
	[SerializeField] private TextMeshProUGUI _valueText;
	[SerializeField] private Slider _slider;
    public void ChangeValue(float value)
	{
		_valueText.text = $"{value.ToString("F2")}";
		_slider.value = value;
	}

	private void Start()
	{
		_slider.minValue = _valueEventChannel.MinValue;
		_slider.maxValue = _valueEventChannel.MaxValue;
	}

}
