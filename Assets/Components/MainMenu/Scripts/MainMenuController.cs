using UnityEditor;
using UnityEngine;
using VComponent.Tools.SceneLoader;

public class MainMenuController : MonoBehaviour
{
	[SerializeField] private GameObject[] _devFeatures;

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
			_devFeatures[i].SetActive(false);
		}
#endif
	}

	public void LaunchSandBox()
	{
		SceneLoader.LoadSandbox();
	}

	public void LaunchLevel()
	{
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