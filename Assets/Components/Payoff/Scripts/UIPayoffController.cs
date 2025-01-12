using Components.Economy;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPayoffController : MonoBehaviour
{
    [SerializeField] private GameObject _uiParent;
    [SerializeField] private TextMeshProUGUI _totalGoldText;
    [SerializeField] private TextMeshProUGUI _baseGoldAmountText;
    [SerializeField] private TextMeshProUGUI _goldInterestText;

    public static Action OnPayoffConfirm;

    void Start()
    {
        //EconomyController.OnEndRoundGoldValuesCalculated += Init;
    }


	private void OnDestroy()
	{
        //EconomyController.OnEndRoundGoldValuesCalculated -= Init;
    }


    private void Init(int totalGoldEarned, int baseGoldAmount, int goldInterest)
	{
        _uiParent.SetActive(true);
        _totalGoldText.text = $"{totalGoldEarned} $";
        _baseGoldAmountText.text = $"{baseGoldAmount} $";
        _goldInterestText.text = $"{goldInterest} $";
	}

    public void Confirm()
	{
        _uiParent.SetActive(false);
        OnPayoffConfirm?.Invoke();
    }
}
