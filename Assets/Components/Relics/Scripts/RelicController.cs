using Components.Ingredients;
using Components.Interactions.Clickable;
using Components.Machines;
using Components.Shop.ShopItems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Relics
{
	public class RelicController : MonoBehaviour, IClickable
	{
		[SerializeField] private Transform _3dViewHolder;
		[SerializeField] private Relic _relic;

		[SerializeField] private GameObject _inPreview;
		[SerializeField] private GameObject _outPreview;

		public static Action<Relic> OnRelicClicked;
		public Relic Relic => _relic;

		private bool _initialized;
		private GameObject _view;


		// ------------------------------------------------------------------------- INIT -------------------------------------------------------------------------
		public void InstantiatePreview(RelicTemplate relicTemplate, float scale)
		{
			_view = Instantiate(relicTemplate.GridView, _3dViewHolder);
			_relic = new Relic(relicTemplate, this);
			_view.transform.localScale = new Vector3(scale, scale, scale);
		}

		public void RotatePreview(int angle)
		{
			_view.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));

		}

		public void ConfirmPlacement()
		{

			_initialized = true;

			_relic.OnTick += Tick;

		}

		private void OnDestroy()
		{
			if (!_initialized)
			{
				return;
			}

			_relic.OnTick -= Tick;

		}

		public void Clicked()
		{
			if (!_initialized)
			{
				return;
			}

			OnRelicClicked?.Invoke(_relic);
		}

		private void Tick()
		{
			
		}
	}

}
