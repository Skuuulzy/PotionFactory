using UnityEngine;

public class StartingGameLetterController : MonoBehaviour
{
	[SerializeField] private GameObject _parent;

	private void Awake()
	{
		BundleChoiceState.OnBundleStateStarted += Init;
	}

	private void OnDestroy()
	{
		BundleChoiceState.OnBundleStateStarted -= Init;
	}

	private void Init(BundleChoiceState state)
	{
		
		if (state.StateIndex == 1)
		{
			_parent.SetActive(true);
		}
	}
}
