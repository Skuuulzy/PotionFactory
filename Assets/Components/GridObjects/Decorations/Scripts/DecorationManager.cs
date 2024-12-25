using Components.Grid.Obstacle;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Components.Grid.Decorations
{
	public class DecorationManager : MonoBehaviour
	{

		[Header("Templates")]
		[SerializeField] private List<DecorationTemplate> _decorationsTemplateList;
		[Header("Selector View")]
		[SerializeField] private DecorationSelectorView _decorationSelectorView;
		[SerializeField] private Transform  _decorationSelectorViewHolder;


		public DecorationTemplate SelectedDecoration { get; private set; }

		public static Action<DecorationTemplate> OnChangeSelectedDecoration;


		void Start()
		{
			_decorationsTemplateList = ScriptableObjectDatabase.GetAllScriptableObjectOfType<DecorationTemplate>().ToList();

			if (_decorationsTemplateList.Count <= 0)
			{
				Debug.LogWarning("[Obstacle] No templates found.");
				return;
			}

			ChangeDecorationCategory(0);
			// Init selected machine has the first 
			SelectedDecoration = _decorationsTemplateList[0];
		}


		private void HandleDecorationSelected(DecorationTemplate decoration)
		{
			SelectedDecoration = decoration;
			OnChangeSelectedDecoration?.Invoke(decoration);
		}

		public void ChangeDecorationCategory(int category)
		{
			int childCount = _decorationSelectorViewHolder.childCount;

			for (int i = childCount - 1; i >= 0; i--)
			{
				Destroy(_decorationSelectorViewHolder.GetChild(i).gameObject);
			}

			List<DecorationTemplate> filtredDecorationTemplate = _decorationsTemplateList.Where(decoration => decoration.DecorationCategory == (DecorationCategory)category).ToList();
			foreach (var decoration in filtredDecorationTemplate)
			{
				DecorationSelectorView selectorView = Instantiate(_decorationSelectorView, _decorationSelectorViewHolder);
				selectorView.Init(decoration);
				selectorView.OnSelected += HandleDecorationSelected;
			}
		}
	}
}
