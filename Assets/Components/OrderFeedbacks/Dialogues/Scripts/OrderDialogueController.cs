using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Order
{
    public class OrderDialogueController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _characterImage;

        public void SetCharacterImage(Sprite sprite)
		{
            _characterImage.sprite = sprite;
		}

        public void SetText(string text)
		{
            _text.text = text;
		}
    }
}

