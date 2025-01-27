using System;
using System.Collections.Generic;
using UnityEngine;
using VComponent.CameraSystem;
using CameraType = VComponent.CameraSystem.CameraType;

namespace Components.Grid.Parcel
{
    public class GridParcelUnlocker : MonoBehaviour
    {
        [SerializeField] private List<GridParcel> _parcelsToUnlock;
        
        [SerializeField] private GridParcelView _parcelViewPrefab;
        [SerializeField] private Transform _parcelViewHolder;
        
        public static Action<GridParcel> OnParcelUnlocked;

        private void Awake()
        {
            _parcelViewHolder.gameObject.SetActive(false);
            InstantiateParcelViews();
        }

        private void InstantiateParcelViews()
        {
            for (var i = 0; i < _parcelsToUnlock.Count; i++)
            {
                var parcel = _parcelsToUnlock[i];
                
                var parcelView = Instantiate(_parcelViewPrefab, _parcelViewHolder);
                parcelView.transform.name = $"Parcel ({parcel.OriginPosition.x}, {parcel.OriginPosition.y})";
                parcelView.Initialize(parcel, 1, true, i);
                
                parcelView.OnParcelBought += HandleParcelBought;
            }
        }

        private void HandleParcelBought(GridParcelView parcelView)
        {
            Debug.Log($"Parcel {parcelView.Parcel.OriginPosition} purchased !");
            OnParcelUnlocked?.Invoke(parcelView.Parcel);
            
            parcelView.OnParcelBought -= HandleParcelBought;
            Destroy(parcelView.gameObject);
        }

        public void ShowInterface()
        {
            _parcelViewHolder.gameObject.SetActive(true);
            CameraSelector.Instance.SwitchToCamera(CameraType.TOP_VIEW);
        }

        public void HideInterface()
        {
            _parcelViewHolder.gameObject.SetActive(false);
            CameraSelector.Instance.SwitchToCamera(CameraType.GAMEPLAY);
        }
    }
}