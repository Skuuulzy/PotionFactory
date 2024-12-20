using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIIngredientSlotController : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _countTxt;
    [SerializeField] private Sprite _emptySprite;

    public void SetEmpty(int maxIngredientInSlotCount)
    {
        _icon.sprite = _emptySprite;
        _countTxt.text = $"0/{maxIngredientInSlotCount}";
    }

    public void SetIngredientSlot(Sprite ingredientSprite, int ingredientInSlotCount, int maxIngredientInSlotCount)
    {
        _icon.sprite = ingredientSprite;
        _countTxt.text = $"{ingredientInSlotCount}/{maxIngredientInSlotCount}";
    }
}
