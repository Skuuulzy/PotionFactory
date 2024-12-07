using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Grid
{
	public abstract class GridObjectController : MonoBehaviour
	{
		[SerializeField] protected Transform _3dViewHolder;

		protected GameObject _view;

		public Transform View => _3dViewHolder;

		// ------------------------------------------------------------------------- INIT -------------------------------------------------------------------------
		public virtual void InstantiatePreview(GridObjectTemplate template, float scale)
		{
			_view = Instantiate(template.GridView, _3dViewHolder);

			transform.localScale = new Vector3(scale, scale, scale);
		}
	}
}
