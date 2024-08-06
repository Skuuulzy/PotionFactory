using Components.Grid.Generator;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveAndLoadMapPopup : MonoBehaviour
{
	[SerializeField] private Transform _existingFilesTransform;
	[SerializeField] private SavingAndLoadingMapPreview _savingAndLoadingMapPreviewPrefab;
	[SerializeField] private GridGenerator _gridGenerator;

	private void OnEnable()
	{
		foreach (Transform child in _existingFilesTransform)
		{
			Destroy(child.gameObject);
		}

		DirectoryInfo info = new DirectoryInfo(Application.persistentDataPath );
		FileInfo[] fileInfo = info.GetFiles();
		foreach(FileInfo file in fileInfo)
		{
			SavingAndLoadingMapPreview loadingMapPreview = Instantiate(_savingAndLoadingMapPreviewPrefab, _existingFilesTransform);
			loadingMapPreview.Init(file.Name, _gridGenerator);
		}
	}

}
