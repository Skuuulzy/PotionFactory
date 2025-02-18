using Components.Machines;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBuyedItemController : MonoBehaviour
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemName;
    
    public void SetInfos(MachineTemplate template)
	{
        _itemImage.sprite = template.UIView;
        _itemName.text = template.Name;
	}
}
