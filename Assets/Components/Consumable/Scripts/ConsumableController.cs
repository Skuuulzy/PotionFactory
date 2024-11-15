using Components.Grid;

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Consumable
{
	public class ConsumableController : MonoBehaviour
	{
		[SerializeField] private Transform _3dViewHolder;
		[SerializeField] private Consumable _consumable;


		public Consumable Consumable => _consumable;

		private bool _initialized;
		private GameObject _view;
		private ConsumableTemplate _template;

		public ConsumableTemplate Template => _template;

		private Cell _chosenCell;
		private int _radius;



		// ------------------------------------------------------------------------- INIT -------------------------------------------------------------------------
		public void InstantiatePreview(ConsumableTemplate consumableTemplate, float scale)
		{
			_view = Instantiate(consumableTemplate.GridView, _3dViewHolder);
			_consumable = new Consumable(consumableTemplate, this);
			_view.transform.localScale = new Vector3(scale, scale, scale);
			_template = consumableTemplate;
		}


		public void ConfirmPlacement(Cell chosenCell)
		{
			_initialized = true;
			_chosenCell = chosenCell;
			LaunchConsumableEffect();
		}

		private void LaunchConsumableEffect()
		{
			//Launch effect of the consumable
			Debug.Log($"Launch effect of {Template}");
		}

		private void OnDestroy()
		{
			if (!_initialized)
			{
				return;
			}

		}
	}

}
