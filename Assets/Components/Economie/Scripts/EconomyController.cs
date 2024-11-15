using System;
using UnityEngine;
using VComponent.Tools.Singletons;

namespace Components.Economy
{
	public class EconomyController : Singleton<EconomyController>
	{
		[SerializeField] private int _playerMoney;
		[SerializeField] private RunConfiguration _runConfiguration;

		private int _stateScoreObjective;
		private int _statePlayerScore; //Money that the player earn during a current phase

		public int PlayerMoney => _playerMoney;
		public static Action<int> OnPlayerMoneyUpdated;
		public static Action<int> OnStatePlayerScoreUpdated;
		public static Action<int> OnScoreStateObjectiveUpdated;
		public static Action OnGameOver;

		private void Start()
		{
			PlanningFactoryState.OnPlanningFactoryStateStarted += HandlePlanningFactoryState;
			ShopState.OnShopStateStarted += HandleShopState;
			ResolutionFactoryState.OnResolutionFactoryStateEnded += HandleResolutionFactoryStateEnded;
		}


		private void OnDestroy()
		{
			PlanningFactoryState.OnPlanningFactoryStateStarted -= HandlePlanningFactoryState;
			ShopState.OnShopStateStarted -= HandleShopState;
			ResolutionFactoryState.OnResolutionFactoryStateEnded -= HandleResolutionFactoryStateEnded;

		}

		public void AddMoney(int amount)
		{
			_playerMoney += amount;
			OnPlayerMoneyUpdated?.Invoke(_playerMoney);
		}

		public void AddScore(int amount)
		{
			_statePlayerScore += amount;
			OnStatePlayerScoreUpdated?.Invoke(_statePlayerScore);
		}
		
		public void DecreaseMoney(int amount)
		{
			_playerMoney -= amount;			
			OnPlayerMoneyUpdated?.Invoke(_playerMoney);
		}

		private void HandlePlanningFactoryState(PlanningFactoryState state)
		{
			_statePlayerScore = 0;
			_stateScoreObjective = _runConfiguration.RunStateList.Find(x => x.StateNumber == state.StateIndex).MoneyObjective;
			OnScoreStateObjectiveUpdated?.Invoke(_stateScoreObjective);
			OnStatePlayerScoreUpdated?.Invoke(_statePlayerScore);

		}

		private void HandleResolutionFactoryStateEnded(ResolutionFactoryState state)
		{
			if(_statePlayerScore < _stateScoreObjective)
			{
				OnGameOver?.Invoke();
			}
		}

		private void HandleShopState(ShopState state)
		{
			int amount = 5 + (_playerMoney / 5);
			AddMoney(amount);
		}

	}
}