using System;
using Components.Economy;
using SoWorkflow.SharedValues;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Grid.Parcel
{
    public class GridParcelView: MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Button _buyButton;
        [SerializeField] private TMP_Text _parcelPriceTxt;
        [SerializeField] private Image _background;

        public Action<GridParcelView> OnParcelBought;

        [Header("SharedValues")]
        [SerializeField] private SOSharedInt _playerGuildToken;

        private int _index;
        private GridParcel _parcel;
        private int _price;
        private bool _unlocked;

        public GridParcel Parcel => _parcel;

        public void Initialize(GridParcel parcel, int cellSize, bool unlocked, int index)
        {
            _rectTransform.anchoredPosition = new Vector2(parcel.OriginPosition.x * cellSize, parcel.OriginPosition.y * cellSize);
            _rectTransform.sizeDelta = new Vector2(parcel.Width * cellSize, parcel.Lenght * cellSize);
            
            _price = parcel.Price;
            _unlocked = unlocked;
            _parcel = parcel;

            _parcelPriceTxt.text = _price.ToString();
            _background.color = ExtensionMethods.GenerateRandomColorWithAlpha(0.1f);
            
        }

        private void OnEnable()
        {
            CheckBuyingEligibility(_playerGuildToken.Value);
            _playerGuildToken.OnValueUpdated += CheckBuyingEligibility;
        }

        private void OnDisable()
        {
            _playerGuildToken.OnValueUpdated -= CheckBuyingEligibility;
        }

        public void Buy()
        {
            if (!_unlocked)
            {
                Debug.LogError("Parcel cannot be bought because it is not unlocked yet.");
                return;
            }

            OnParcelBought?.Invoke(this);
            EconomyController.Instance.DecreaseMoney(_price);
        }
        
        private void CheckBuyingEligibility(int playerMoney)
        {
            _buyButton.interactable = _price <= playerMoney;
            _parcelPriceTxt.color = _buyButton.interactable ? Color.green : Color.red;
        }
    }
}