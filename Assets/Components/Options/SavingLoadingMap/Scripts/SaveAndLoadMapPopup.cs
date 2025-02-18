using Components.Grid.Generator;
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

		var fileNames = GridGenerator.GetAllMapFileNames();

		for (var i = 0; i < fileNames.Count; i++)
		{
			var fileName = fileNames[i];
			SavingAndLoadingMapPreview loadingMapPreview = Instantiate(_savingAndLoadingMapPreviewPrefab, _existingFilesTransform);
			loadingMapPreview.Init(fileName, _gridGenerator);
		}
	}
}