using UnityEngine;

namespace Components.Bundle
{
	public abstract class Bundle : ScriptableObject
	{
		[SerializeField] protected string _bundleName;
	}
}

