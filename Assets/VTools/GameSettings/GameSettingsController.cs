using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VComponent.Tools.SceneLoader;

namespace VTools.GameSettings
{
    public class GameSettingsController : MonoBehaviour
    {
        // ----------------------------------------- SERIALIZED FIELDS -------------------------------------------------

        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private TMP_Dropdown _resolutionDropdown;
        [SerializeField] private Vector2Int[] _availableResolutions;

        private Resolution[] _resolutions;
        
        // ----------------------------------------- MONO -------------------------------------------------

        private void Start()
        {
            InitializeResolutions();
            LoadSettings();
        }

        // ----------------------------------------- PUBLIC METHODS -------------------------------------------------
        
        /// Sets the music volume and saves it to PlayerPrefs.
        public void ToggleMusic(bool toggle)
        {
            PlayerPrefs.SetInt("ToggleMusic", toggle ? 1 : 0);
            PlayerPrefs.Save();
        }

        /// Sets the screen resolution based on the dropdown selection.
        public void SetResolution(int resolutionIndex)
        {
            if (resolutionIndex < 0 || resolutionIndex >= _availableResolutions.Length)
            {
                Debug.LogError("[SettingsManager] Invalid resolution index.");
                return;
            }

            Resolution resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            
            PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
            PlayerPrefs.Save();
        }

        public void LoadMainMenu()
        {
            SceneLoader.LoadMainMenu();
        }

        // ----------------------------------------- PRIVATE METHODS -------------------------------------------------
        
        private void InitializeResolutions()
        {
            _resolutions = new Resolution[_availableResolutions.Length];
            
            for (int i = 0; i < _availableResolutions.Length; i++)
            {
                _resolutions[i] = new Resolution
                {
                    width = _availableResolutions[i].x,
                    height = _availableResolutions[i].y
                    //TODO: HandleRefresh rate
                };
            }
        }

        
        /// Loads saved settings and applies them.
        private void LoadSettings()
        {
            // Load and apply music volume

            // Load and apply full-screen setting
            bool isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
            Screen.fullScreen = isFullScreen;

            // Load available resolutions and set the dropdown
            _resolutionDropdown.ClearOptions();

            int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
            savedResolutionIndex = Mathf.Clamp(savedResolutionIndex, 0, _resolutions.Length - 1);

            for (int i = 0; i < _resolutions.Length; i++)
            {
                _resolutionDropdown.options.Add(new TMP_Dropdown.OptionData($"{_resolutions[i].width} x {_resolutions[i].height}"));
            }

            _resolutionDropdown.value = savedResolutionIndex;
            _resolutionDropdown.RefreshShownValue();
            SetResolution(savedResolutionIndex);
        }
    }
}