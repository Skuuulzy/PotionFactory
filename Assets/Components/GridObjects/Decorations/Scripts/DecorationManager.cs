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
		[SerializeField] private SerializableDictionary<DecorationCategory, Transform>  _decorationSelectorViewHolder;


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

			foreach (var decoration in _decorationsTemplateList)
			{
				DecorationSelectorView selectorView = Instantiate(_decorationSelectorView, _decorationSelectorViewHolder[decoration	.DecorationCategory]);
				selectorView.Init(decoration);
				selectorView.OnSelected += HandleObstacleSelected;
			}

			// Init selected machine has the first 
			SelectedDecoration = _decorationsTemplateList[0];
		}


		private void HandleObstacleSelected(DecorationTemplate decoration)
		{
			SelectedDecoration = decoration;
			OnChangeSelectedDecoration?.Invoke(decoration);
		}
	}
}
