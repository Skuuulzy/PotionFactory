using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
	[SerializeField] private GameObject _loadingSplashScreen;

	private void Start()
	{
		_loadingSplashScreen.SetActive(false);
	}

	public void LaunchSandBox()
	{
		SceneManager.LoadScene("Grid_Sandbox", LoadSceneMode.Single);
	}

	public void LaunchLevel()
	{
		_loadingSplashScreen.SetActive(true);
		
		SceneManager.LoadSceneAsync("Level")!.completed
			+= _ =>
			{
				SceneManager.LoadSceneAsync("LevelUI", LoadSceneMode.Additive);
			};
	}

	public void LaunchGridGenerator()
	{
		SceneManager.LoadScene("Grid_Generator", LoadSceneMode.Single);
	}
}
