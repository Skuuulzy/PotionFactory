using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Grid.Decorations
{
    [CreateAssetMenu(fileName = "New Decoration Template", menuName = "Grid/Decoration Template")]
    public class DecorationTemplate : GridObjectTemplate
    {
		[SerializeField] private DecorationType _decorationType;

		public DecorationType DecorationType => _decorationType;
	}
}
