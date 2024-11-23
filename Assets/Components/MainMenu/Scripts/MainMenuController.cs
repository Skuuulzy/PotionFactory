using UnityEngine;
using VComponent.Tools.SceneLoader;

public class MainMenuController : MonoBehaviour
{
	public void LaunchSandBox()
	{
		SceneLoader.Instance.LoadSandbox();
	}

	public void LaunchLevel()
	{
		SceneLoader.Instance.LoadLevel();
	}

	public void LaunchGridGenerator()
	{
		SceneLoader.Instance.LoadGridGenerator();
	}
}