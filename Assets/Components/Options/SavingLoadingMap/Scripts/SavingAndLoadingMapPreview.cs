using Components.Grid.Generator;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SavingAndLoadingMapPreview : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _fileNameText;
	[SerializeField] private Button _button;
	private GridGenerator _gridGenerator;
	public Button Button => _button;
	public void Init(string name, GridGenerator gridGenerator)
	{
		_fileNameText.text = name;
		_gridGenerator = gridGenerator;
	}

	public void SelectFileName()
	{
		if(_gridGenerator == null)
		{
			Debug.LogError("GridGenerator should not be null");
			return; 
		}

		_gridGenerator.SetFileName(_fileNameText.text);
	}
}
