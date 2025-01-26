using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VComponent.CameraSystem;
using CameraType = VComponent.CameraSystem.CameraType;

namespace Components.Grid
{
    public class GridParcelUnlocker : MonoBehaviour
    {
        [SerializeField] private GameObject _unlockInterface;
        [SerializeField] private List<GridParcel> _parcelsToUnlock;

        public static Action<GridParcel> OnParcelUnlocked;
        
        public void ShowInterface()
        {
            _unlockInterface.gameObject.SetActive(true);
            CameraSelector.Instance.SwitchToCamera(CameraType.TOP_VIEW);
        }

        public void HideInterface()
        {
            _unlockInterface.gameObject.SetActive(false);
            CameraSelector.Instance.SwitchToCamera(CameraType.GAMEPLAY);
        }

        public void BuyParcel(int parcelIndex)
        {
            Debug.Log($"Parcel {parcelIndex} purchased !");
            OnParcelUnlocked?.Invoke(_parcelsToUnlock[parcelIndex]);
        }
        
        [Button(ButtonSizes.Medium)]
        private void UnlockParcelIndex(int index)
        {
            OnParcelUnlocked?.Invoke(_parcelsToUnlock[index]);
        }
    }
}