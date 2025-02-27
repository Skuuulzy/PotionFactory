using Components.GameParameters;
using TMPro;
using UnityEditor;
using UnityEngine;
using VComponent.Tools.SceneLoader;

public class MainMenuController : MonoBehaviour
{
	[SerializeField] private GameObject[] _devFeatures;
	[SerializeField] private TMP_Text _bestScoreText;

	private void Start()
	{
#if UNITY_EDITOR
		for (int i = 0; i < _devFeatures.Length; i++)
		{
			_devFeatures[i].SetActive(true);
		}
#else
		for (int i = 0; i < _devFeatures.Length; i++)
		{
			_devFeatures[i].SetActive(Debug.isDebugBuild);
		}
#endif

		_bestScoreText.text = $"{GameParameters.CurrentBestScore}";
	}

	public void LaunchSandBox()
	{
		GameParameters.CurrentGameMode = GameParameters.GameMode.SANDBOX;
		SceneLoader.LoadLevel();
	}

	public void LaunchLevel()
	{
		GameParameters.CurrentGameMode = GameParameters.GameMode.STANDARD;
		SceneLoader.LoadLevel();
	}

	public void LaunchGridGenerator()
	{
		SceneLoader.LoadGridGenerator();
	}

	public void QuitGame()
	{
#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}
}