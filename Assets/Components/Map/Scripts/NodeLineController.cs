using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeLineController : MonoBehaviour
{
    [SerializeField] private Image _nodeLineImage;

    [SerializeField] private Color _lineConstructedColor;
    [SerializeField] private Color _lineUnconstructedColor;

	private void Start()
	{
        _nodeLineImage.color = _lineUnconstructedColor;
	}


	public void SetConstructedLineColor(bool value)
	{
        _nodeLineImage.color = value ? _lineConstructedColor : _lineUnconstructedColor;
	}
}
