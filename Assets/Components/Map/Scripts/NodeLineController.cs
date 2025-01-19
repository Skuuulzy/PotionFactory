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



	public void SetConstructedLineColor(bool value, int roadSectionNumber = 1)
	{
		for(int i = 0; i < _nodeLineImageList.Count; i++)
		{
			//Return if we go further the road section number 
			if(i >= roadSectionNumber)
			{
				_nodeLineImageList[i].color = _lineUnconstructedColor;
			}
			else
			{
				_nodeLineImageList[i].color = value ? _lineConstructedColor : _lineUnconstructedColor;
			}
			
		}
   
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
