using UnityEngine;
using VComponent.Tools.SceneLoader;

public class Boot : MonoBehaviour
{
    private void Start()
    {
        SceneLoader.LoadMainMenu();
    }
}