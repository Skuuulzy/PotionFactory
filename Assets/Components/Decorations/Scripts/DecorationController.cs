using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Grid.Decorations
{
    public class DecorationController : MonoBehaviour
    {
		[SerializeField] private Transform _3dViewHolder;
		[SerializeField] private DecorationType _decorationType;

		private GameObject _view;

		public Transform View => _3dViewHolder;
		public DecorationType DecorationType => _decorationType;

		// ------------------------------------------------------------------------- INIT -------------------------------------------------------------------------
		public void InstantiatePreview(DecorationTemplate decorationTemplate, float scale)
		{
			_view = Instantiate(decorationTemplate.GridView, _3dViewHolder);

			transform.localScale = new Vector3(scale, scale, scale);
		}


		public void SetDecorationType(DecorationType obstacleType)
		{
			_decorationType = obstacleType;
		}
	}

	public enum DecorationType
	{
		NONE,
		FLOWER1
	}
}
