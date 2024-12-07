using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Grid.Decorations
{
	public class DecorationSelectorView : MonoBehaviour
	{
		[SerializeField] private TMP_Text _name;
		[SerializeField] private Image _background;
		public Action<DecorationTemplate> OnSelected;

		private DecorationTemplate _decoration;

		public void Init(DecorationTemplate obstacle)
		{
			_decoration = obstacle;

			_name.text = obstacle.Name;
			_background.sprite = obstacle.UIView;
		}

		public void Select()
		{
			OnSelected?.Invoke(_decoration);
		}
	}
}
