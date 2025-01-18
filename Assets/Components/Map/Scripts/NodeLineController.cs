using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodeLineController : MonoBehaviour
{
    [SerializeField] private Image _nodeLineImage;

    [SerializeField] private Color _lineConstructedColor;
    [SerializeField] private Color _lineUnconstructedColor;
	[SerializeField] private TextMeshProUGUI _distanceText;

	private int _normalizedDistance; 

	public int NormalizedDistance => _normalizedDistance;
	private void Start()
	{
        _nodeLineImage.color = _lineUnconstructedColor;
	}


	public void SetConstructedLineColor(bool value)
	{
        _nodeLineImage.color = value ? _lineConstructedColor : _lineUnconstructedColor;
		//_distanceText.gameObject.SetActive(value);
	}

	public void SetNormalizedDistance(int value)
	{
		_normalizedDistance = value;
		_distanceText.text = value.ToString();
	}
}
