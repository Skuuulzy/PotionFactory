using System;
using Components.Economy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Grid.Parcel
{
    public class GridParcelView: MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Button _buyButton;
        [SerializeField] private TMP_Text _parcelNameTxt;

        public Action<GridParcelView> OnParcelBought;

        private int _index;
        private GridParcel _parcel;
        private int _price;
        private bool _unlocked;

        public GridParcel Parcel => _parcel;

        public void Initialize(GridParcel parcel, int cellSize, bool unlocked, int index)
        {
            _rectTransform.anchoredPosition = new Vector2(parcel.OriginPosition.x * cellSize, parcel.OriginPosition.y * cellSize);
            _rectTransform.sizeDelta = new Vector2(parcel.Width * cellSize, parcel.Lenght * cellSize);

            _parcelNameTxt.text = $"Parcel {index}";
            
            _price = parcel.Price;
            _unlocked = unlocked;
            _parcel = parcel;

            EconomyController.OnPlayerMoneyUpdated += CheckBuyingEligibility;
            CheckBuyingEligibility(EconomyController.Instance.PlayerMoney);
        }

        private void OnDestroy()
        {
            EconomyController.OnPlayerMoneyUpdated -= CheckBuyingEligibility;
        }

        public void Buy()
        {
            if (!_unlocked)
            {
                Debug.LogError("Parcel cannot be bought because it is not unlocked yet.");
                return;
            }

            OnParcelBought?.Invoke(this);
        }
        
        private void CheckBuyingEligibility(int playerMoney)
        {
            _buyButton.interactable = _price <= playerMoney;
        }
    }
}