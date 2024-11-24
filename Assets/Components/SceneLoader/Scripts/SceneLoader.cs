using Eflatun.SceneReference;
using MyGameDevTools.SceneLoading;
using UnityEngine;
using UnityEngine.SceneManagement;
using VComponent.Tools.Singletons;


namespace VComponent.Tools.SceneLoader
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        [SerializeField] private SceneReference _loadingScene;
        [SerializeField] private SceneReference _levelScene;
        [SerializeField] private SceneReference _levelUIScene;
        [SerializeField] private SceneReference _mainMenuScene;
        [SerializeField] private SceneReference _sandBoxScene;
        [SerializeField] private SceneReference _gridGenerator;

        private ISceneManager _sceneManager;
        private ISceneLoaderUniTask _sceneLoader;
        private ILoadSceneInfo _loadingSceneInfo;

        private async void Start()
        {
            DontDestroyOnLoad(this);

            _sceneManager = new AdvancedSceneManager();
            _sceneLoader = new SceneLoaderUniTask(_sceneManager);
            var sceneLoader = new SceneLoaderCoroutine(_sceneManager);

            _loadingSceneInfo = new LoadSceneInfoName(_loadingScene.Name);

            await _sceneLoader.TransitionToSceneAsync(new LoadSceneInfoName(_mainMenuScene.Name), _loadingSceneInfo);

            // Unload the boot
            SceneManager.UnloadSceneAsync(0);
        }

        public async void LoadLevel()
        {
            ILoadSceneInfo[] levelGroup =
            {
                new LoadSceneInfoName(_levelScene.Name),
                new LoadSceneInfoName(_levelUIScene.Name),
            };

            await _sceneLoader.TransitionToScenesAsync(levelGroup, 0, _loadingSceneInfo);
        }

        public async void LoadSandbox()
        {
            await _sceneLoader.TransitionToSceneAsync(new LoadSceneInfoName(_sandBoxScene.Name), _loadingSceneInfo);
        }

        public async void LoadMainMenu()
        {
            await _sceneLoader.TransitionToSceneFromAllAsync(new LoadSceneInfoName(_mainMenuScene.Name), _loadingSceneInfo);
        }

        public async void LoadGridGenerator()
        {
            await _sceneLoader.TransitionToSceneAsync(new LoadSceneInfoName(_gridGenerator.Name), _loadingSceneInfo);
        }
    }
}