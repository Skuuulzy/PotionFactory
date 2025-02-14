using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VComponent.CameraSystem;
using CameraType = VComponent.CameraSystem.CameraType;

namespace Components.Grid.Parcel
{
    public class GridParcelUnlocker : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private List<GridParcel> _parcelsToUnlock;
        
        [Header("Components")]
        [SerializeField] private GridParcelView _parcelViewPrefab;
        [SerializeField] private Transform _parcelViewHolder;

        [Header("Parcels generator")] 
        [SerializeField] private Vector2Int _gridSize;
        [SerializeField] private int _parcelSize;
        [SerializeField] private int _parcelPrice;
         
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
        
        // ----------------------------------------- Generate Parcels -------------------------------------------------

        [Button]
        private void GenerateParcels()
        {
            var parcels = GenerateParcel(_gridSize, _parcelSize, _parcelPrice);
            _parcelsToUnlock = parcels;
        }
        
        /// Generates a list of parcels by splitting the grid into equally sized parcels
        private List<GridParcel> GenerateParcel(Vector2Int gridSize, int parcelSize, int parcelPrice)
        {
            // Validate inputs
            if (gridSize.x <= 0 || gridSize.y <= 0 || parcelSize <= 0)
            {
                Debug.LogError("[GridParcelUnlocker] Invalid gridSize or parcelSize. All must be greater than 0.");
                return null;
            }

            // Ensure the grid dimensions are divisible by the parcel size
            if (gridSize.x % parcelSize != 0 || gridSize.y % parcelSize != 0)
            {
                Debug.LogError("[GridParcelUnlocker] Grid dimensions must be divisible by the parcel size for equal parcels.");
                return null;
            }

            List<GridParcel> parcels = new List<GridParcel>();

            // Calculate the number of parcels in each dimension
            int parcelCountX = gridSize.x / parcelSize;
            int parcelCountY = gridSize.y / parcelSize;

            // Loop through and create parcels
            for (int y = 0; y < parcelCountY; y++)
            {
                for (int x = 0; x < parcelCountX; x++)
                {
                    // Define the parcel origin
                    Vector2Int origin = new Vector2Int(x * parcelSize, y * parcelSize);

                    // Create and store the parcel
                    GridParcel parcel = new GridParcel
                    {
                        OriginPosition = origin,
                        Lenght = parcelSize,
                        Width = parcelSize,
                        Price = parcelPrice
                    };

                    parcels.Add(parcel);
                }
            }

            return parcels;
        }
    }
}