using System;
using UnityEngine;
using VComponent.Tools.Singletons;

namespace Components.Economy
{
	public class EconomyController : Singleton<EconomyController>
	{
		[SerializeField] private int _playerMoney;
		[SerializeField] private RunConfiguration _runConfiguration;

		private int _stateMoneyObjective;
		private int _statePlayerMoney; //Money that the player earn during a current phase

		public int PlayerMoney => _playerMoney;
		public static Action<int> OnPlayerMoneyUpdated;
		public static Action<int> OnStatePlayerMoneyUpdated;
		public static Action<int> OnMoneyStateObjectiveUpdated;
		public static Action OnGameOver;

		private void Start()
		{
			PlanningFactoryState.OnPlanningFactoryStateStarted += HandlePlanningFactoryState;
			ResolutionFactoryState.OnResolutionFactoryStateEnded += HandleResolutionFactoryStateEnded;
		}

		private void OnDestroy()
		{
			PlanningFactoryState.OnPlanningFactoryStateStarted -= HandlePlanningFactoryState;
			ResolutionFactoryState.OnResolutionFactoryStateEnded -= HandleResolutionFactoryStateEnded;

		}

		public void AddMoney(int amount)
		{
			_playerMoney += amount;
			_statePlayerMoney += amount;
			OnPlayerMoneyUpdated?.Invoke(_playerMoney);
			OnStatePlayerMoneyUpdated?.Invoke(_statePlayerMoney);
		}
		
		public void DecreaseMoney(int amount)
		{
			_playerMoney -= amount;			
			OnPlayerMoneyUpdated?.Invoke(_playerMoney);
		}

		private void HandlePlanningFactoryState(PlanningFactoryState state)
		{
			_statePlayerMoney = 0;
			_stateMoneyObjective = _runConfiguration.RunStateList.Find(x => x.StateNumber == state.StateIndex).MoneyObjective;
			OnMoneyStateObjectiveUpdated?.Invoke(_stateMoneyObjective);
			OnStatePlayerMoneyUpdated?.Invoke(_statePlayerMoney);

		}

		private void HandleResolutionFactoryStateEnded(ResolutionFactoryState state)
		{
			if(_statePlayerMoney < _stateMoneyObjective)
			{
				OnGameOver?.Invoke();
			}
		}
	}
}