using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
   
	public void LaunchGame()
	{
		SceneManager.LoadScene("Grid_Sandbox", LoadSceneMode.Single);
	}

	public void LaunchGridGenerator()
	{
		SceneManager.LoadScene("Grid_Generator", LoadSceneMode.Single);
	}
}
