using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeIngredientView : MonoBehaviour
{
    [SerializeField] private Image _ingredientImage; 
    [SerializeField] private TextMeshProUGUI _ingredientNumber;
    [SerializeField] private TextMeshProUGUI _ingredientName;


    public void Init(Sprite sprite, int number, string ingredientName)
    {
        _ingredientImage.sprite = sprite;
        _ingredientNumber.gameObject.SetActive(!(number == -1));
        _ingredientNumber.text = $"x{number}";
        _ingredientName.text = ingredientName;
    }
}
