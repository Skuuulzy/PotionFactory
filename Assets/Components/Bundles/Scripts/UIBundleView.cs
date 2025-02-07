using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Bundle
{
    public class UIBundleView : MonoBehaviour
    {

        [SerializeField] private Image _itemImage;
        [SerializeField] private TextMeshProUGUI _itemNameTxt;


        public void SetInfos(Sprite itemSprite, string itemName)
        {
            _itemImage.sprite = itemSprite;
            _itemNameTxt.text = itemName;
        }
    }
}

