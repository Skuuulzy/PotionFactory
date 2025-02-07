using UnityEngine;

public class StartingGameLetterController : MonoBehaviour
{
	[SerializeField] private GameObject _parent;

	private void Awake()
	{
		MapState.OnMapStateStarted += Init;
	}

	private void OnDestroy()
	{
		MapState.OnMapStateStarted -= Init;
	}

	private void Init(MapState state)
	{
		
		if (state.StateIndex == 1)
		{
			_parent.SetActive(true);
		}
	}
}
