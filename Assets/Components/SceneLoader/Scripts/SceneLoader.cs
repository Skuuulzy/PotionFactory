using MyGameDevTools.SceneLoading;
using UnityEngine;


namespace VComponent.Tools.SceneLoader
{
    public static class SceneLoader
    {
        private static readonly ISceneManager SCENE_MANAGER = new AdvancedSceneManager(true);
        private static readonly ISceneLoaderUniTask SCENE_LOADER = new SceneLoaderUniTask(SCENE_MANAGER);

        private static readonly ILoadSceneInfo BOOT_SCENE_INFO = new LoadSceneInfoName("Boot");
        private static readonly ILoadSceneInfo LOADING_SCREEN_SCENE_INFO = new LoadSceneInfoName("LoadingScreen");
        
        private static readonly ILoadSceneInfo MAIN_MENU_SCENE_INFO = new LoadSceneInfoName("Main_Menu");
        private static readonly ILoadSceneInfo LEVEL_SCENE_INFO = new LoadSceneInfoName("Level");
        private static readonly ILoadSceneInfo LEVEL_UI_SCENE_INFO = new LoadSceneInfoName("LevelUI");
        private static readonly ILoadSceneInfo GRID_GENERATOR_SCENE_INFO = new LoadSceneInfoName("Grid_Generator");
        private static readonly ILoadSceneInfo GRID_SANDBOX_SCENE_INFO = new LoadSceneInfoName("Grid_Sandbox");

        public static async void ReloadGame()
        {
            Debug.Log("[SCENE_LOADER] Reloading game ...");
            await SCENE_LOADER.TransitionToSceneFromAllAsync(BOOT_SCENE_INFO, LOADING_SCREEN_SCENE_INFO);
            Debug.Log("[SCENE_LOADER] Boot loaded !");
        }
        
        public static async void LoadMainMenu()
        {
            Debug.Log("[SCENE_LOADER] Loading main menu ...");
            await SCENE_LOADER.TransitionToSceneFromAllAsync(MAIN_MENU_SCENE_INFO, LOADING_SCREEN_SCENE_INFO);
            Debug.Log("[SCENE_LOADER] Main menu loaded !");
        }
        
        public static async void LoadLevel()
        {
            ILoadSceneInfo[] LEVEL_GROUP =
            {
                LEVEL_SCENE_INFO,
                LEVEL_UI_SCENE_INFO
            };
            
            Debug.Log("[SCENE_LOADER] Loading level ...");
            await SCENE_LOADER.TransitionToScenesFromAllAsync(LEVEL_GROUP,0 ,LOADING_SCREEN_SCENE_INFO);
            Debug.Log("[SCENE_LOADER] Level loaded !");
        }

        public static async void LoadGridGenerator()
        {
            Debug.Log("[SCENE_LOADER] Loading grid generator ...");
            await SCENE_LOADER.TransitionToSceneFromAllAsync(GRID_GENERATOR_SCENE_INFO, LOADING_SCREEN_SCENE_INFO);
            Debug.Log("[SCENE_LOADER] Grid generator loaded !");
        }

        public static async void LoadSandbox()
        {
            Debug.Log("[SCENE_LOADER] Loading sandbox ...");
            await SCENE_LOADER.TransitionToSceneFromAllAsync(GRID_SANDBOX_SCENE_INFO, LOADING_SCREEN_SCENE_INFO);
            Debug.Log("[SCENE_LOADER] Sandbox loaded !");
        }
    }
}