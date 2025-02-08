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
		public static Action<int, int, int, int ,int> OnEndRoundGoldValuesCalculated;

		private int _totalGoldAmountPerRound;
		
		private void Start()
		{
			ResolutionFactoryState.OnResolutionFactoryStateStarted += HandleResolutionFactoryStateStarted;
			EndOfDayState.OnEndOfDayStateStarted += HandleEndOfDayState;
			ResolutionFactoryState.OnResolutionFactoryStateEnded += HandleResolutionFactoryStateEnded;
		}
		
		private void OnDestroy()
		{
			ResolutionFactoryState.OnResolutionFactoryStateStarted -= HandleResolutionFactoryStateStarted;
			EndOfDayState.OnEndOfDayStateStarted -= HandleEndOfDayState;
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

		private void HandleResolutionFactoryStateStarted(ResolutionFactoryState state)
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
		private void HandleEndOfDayState(EndOfDayState shopState)
		{
			int interest = (_playerMoney / _runConfiguration.GuildTicketInterestValue) * _runConfiguration.GuildTicketInterestAmountPerRound;
			_totalGoldAmountPerRound = _runConfiguration.GuildTicketAmountPerRound + interest;
			OnEndRoundGoldValuesCalculated?.Invoke(_totalGoldAmountPerRound, _runConfiguration.GuildTicketAmountPerRound, interest, _stateScoreObjective, _statePlayerScore);
			AddMoney(_totalGoldAmountPerRound);
		}

	}
}