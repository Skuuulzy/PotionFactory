using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodeLineController : MonoBehaviour
{
	[SerializeField] private Transform _nodeLineParent;
	[SerializeField] private Image _nodeLinePrefab;
    [SerializeField] private List<Image> _nodeLineImageList = new List<Image>();

    [SerializeField] private Color _lineConstructedColor;
    [SerializeField] private Color _lineUnconstructedColor;
	[SerializeField] private TextMeshProUGUI _distanceText;

	private int _normalizedDistance; 

	public int NormalizedDistance => _normalizedDistance;

	public void SetConstructedLineColor(bool value, bool isReversed)
	{
		_nodeLineImageList[0].color = value ? _lineConstructedColor : _lineUnconstructedColor;

		//if (isReversed)
		//{
		//	for (int i = _nodeLineImageList.Count - 1; i >= 0; i--)
		//	{
		//		//Return if we go further the road section number 
		//		if (roadSectionNumber <= 0)
		//		{
		//			_nodeLineImageList[i].color = _lineUnconstructedColor;
		//		}
		//		else
		//		{
		//			_nodeLineImageList[i].color = value ? _lineConstructedColor : _lineUnconstructedColor;
		//			roadSectionNumber--;
		//		}


		//	}
		//}
		//else
		//{
		//	for (int i = 0; i < _nodeLineImageList.Count; i++)
		//	{
		//		//Return if we go further the road section number 
		//		if (roadSectionNumber <= 0)
		//		{
		//			_nodeLineImageList[i].color = _lineUnconstructedColor;
		//		}
		//		else
		//		{
		//			_nodeLineImageList[i].color = value ? _lineConstructedColor : _lineUnconstructedColor;
		//			roadSectionNumber--;
		//		}
		//	}
		//}


	}

	public void SetNormalizedDistance(int value)
	{
		_normalizedDistance = value;
		_distanceText.text = value.ToString();
		for(int i = 0; i < _normalizedDistance; i++)
		{
			Image image = Instantiate(_nodeLinePrefab, _nodeLineParent);
			_nodeLineImageList.Add(image);
		}
	}
}
