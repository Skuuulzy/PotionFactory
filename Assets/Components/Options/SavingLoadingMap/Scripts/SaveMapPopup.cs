using Components.Grid.Generator;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveMapPopup : MonoBehaviour
{
	[SerializeField] private Transform _existingFilesTransform;
	[SerializeField] private SavingAndLoadingMapPreview _loadingMapPreviewPrefab;
	[SerializeField] private GridGenerator _gridGenerator;

	private void OnEnable()
	{
		foreach (Transform child in _existingFilesTransform)
		{
			Destroy(child.gameObject);
		}

		//System.IO.File.ReadAllText(Application.persistentDataPath + $"/{_fileName}.json");
		DirectoryInfo info = new DirectoryInfo(Application.persistentDataPath);
		FileInfo[] fileInfo = info.GetFiles();
		foreach (FileInfo file in fileInfo)
		{
			SavingAndLoadingMapPreview loadingMapPreview = Instantiate(_loadingMapPreviewPrefab, _existingFilesTransform);
			loadingMapPreview.Init(file.Name, _gridGenerator);
		}
	}
}
