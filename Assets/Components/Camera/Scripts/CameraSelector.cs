using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using VComponent.Tools.Singletons;

namespace VComponent.CameraSystem
{
    public class CameraSelector : Singleton<CameraSelector>
    {
        [SerializeField] private List<CameraConfiguration> _configurations;
        [SerializeField] private Camera _camera;
        
        private CinemachineVirtualCamera _activeCamera;

        private const int LOW_PRIORITY = 0;
        private const int HIGH_PRIORITY = 10;
        
        public void SwitchToCamera(CameraType type)
        {
            var configuration = _configurations.First(conf => conf.Type == type);

            if (!configuration.IsAssign)
            {
                Debug.LogError($"No camera configuration found for type: {type}.");
                return;
            }
            
            ApplyCameraConfiguration(configuration);
        }

        private void ApplyCameraConfiguration(CameraConfiguration configuration)
        {
            if (_activeCamera)
            {
                _activeCamera.Priority = LOW_PRIORITY;
            }

            _activeCamera = configuration.Camera;

            _activeCamera.Priority = HIGH_PRIORITY;
            RenderSettings.fog = !configuration.DisableFog;
            //_camera.orthographic = configuration.Orthographic;
        }
    }
    
    [Serializable]
    public struct CameraConfiguration
    {
        public CameraType Type;
        public CinemachineVirtualCamera Camera;
        public bool DisableFog;
        public bool Orthographic;

        public bool IsAssign => Camera !=null;
    }
    
    public enum CameraType
    {
        TOP_VIEW,
        GAMEPLAY
    }
}