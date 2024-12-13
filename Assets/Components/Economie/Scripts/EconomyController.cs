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
		public static Action<int, int, int> OnEndRoundGoldValuesCalculated;

		private int _totalGoldAmountPerRound;
		private void Start()
		{
			PlanningFactoryState.OnPlanningFactoryStateStarted += HandlePlanningFactoryState;
			PayoffState.OnPayoffStateStarted += HandleStartPayoffState;
			PayoffState.OnPayoffStateEnded += HandleEndPayoffState;
			ResolutionFactoryState.OnResolutionFactoryStateEnded += HandleResolutionFactoryStateEnded;
		}


		private void OnDestroy()
		{
			PlanningFactoryState.OnPlanningFactoryStateStarted -= HandlePlanningFactoryState;
			PayoffState.OnPayoffStateStarted -= HandleStartPayoffState;
			PayoffState.OnPayoffStateEnded -= HandleEndPayoffState;
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

		/// <summary>
		/// Calcultate the gold amount to give to the player at the start of the payoff State
		/// </summary>
		private void HandleStartPayoffState(PayoffState payoffState)
		{
			int interest = (_playerMoney / _runConfiguration.GoldInterestValue) * _runConfiguration.GoldInterestAmountPerRound;
			_totalGoldAmountPerRound = _runConfiguration.GoldAmountPerRound + interest;
			OnEndRoundGoldValuesCalculated?.Invoke(_totalGoldAmountPerRound, _runConfiguration.GoldAmountPerRound, interest);
		}

		/// <summary>
		/// Give the gold amount previously calculated to player
		/// </summary>
		private void HandleEndPayoffState(PayoffState payoffState)
		{
			AddMoney(_totalGoldAmountPerRound);
		}


	}
}