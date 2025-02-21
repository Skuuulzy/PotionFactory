using SoWorkflow.SharedValues;
using System;
using UnityEngine;
using VComponent.Tools.Singletons;
using VTools.SoWorkflow.EventSystem;

namespace Components.Economy
{
	public class EconomyController : Singleton<EconomyController>
	{
		[Header("Economy")]
		[SerializeField] private SOSharedInt _playerGuildToken;
        [SerializeField] private SOSharedInt _statePlayerScore; //Score that the player earn during resolution state, reset every round
        [SerializeField] private SOSharedInt _stateScoreObjective;
        [SerializeField] private SOSharedInt _totalGuildToken;
        [SerializeField] private SOSharedInt _guildTokenTimeInterest;
		[Header("Time")]
        [SerializeField] private SOSharedFloat _stateCurrentTime;
        [SerializeField] private SOSharedFloat _stateInitialTime;
		[SerializeField] private GameEvent _onPlayerScoreSuccess;

		[SerializeField] private RunConfiguration _runConfiguration;




		private void Start()
		{
			ResolutionFactoryState.OnResolutionFactoryStateStarted += HandleResolutionFactoryStateStarted;
			EndOfDayState.OnEndOfDayStateStarted += HandleEndOfDayStateStarted;
		}
		
		private void OnDestroy()
		{
			ResolutionFactoryState.OnResolutionFactoryStateStarted -= HandleResolutionFactoryStateStarted;
			EndOfDayState.OnEndOfDayStateStarted -= HandleEndOfDayStateStarted;
            _stateCurrentTime.OnValueUpdated -= CheckInterest;
        }

		public void AddMoney(int amount)
		{
			_playerGuildToken.Increment(amount);

		}

		public void AddScore(int amount)
		{
			_statePlayerScore.Increment(amount);
			if(_statePlayerScore.Value >= _stateScoreObjective.Value)
			{
				_onPlayerScoreSuccess?.Raise();
				_stateCurrentTime.OnValueUpdated += CheckInterest;
            }
		}

        private void CheckInterest(float value)
        {
			_guildTokenTimeInterest.Set(Mathf.RoundToInt((value / _stateInitialTime.Value) * _runConfiguration.GuildTicketTimeInterestMaxValue));
        }

        public void DecreaseMoney(int amount)
		{
			_playerGuildToken.Increment(-amount);			

		}

		private void HandleResolutionFactoryStateStarted(ResolutionFactoryState state)
		{
			if(state.StateIndex == 1)
			{
				ResetValues();
			}
			_statePlayerScore.Set(0);
			_stateScoreObjective.Set(_runConfiguration.RunStateList.Find(x => x.StateNumber == state.StateIndex).MoneyObjective);
		}

		private void ResetValues()
		{
			_statePlayerScore.Set(0);
			_playerGuildToken.Set(0);
			_totalGuildToken.Set(0);
        }

        public bool CheckGameOver(int stateIndex)
		{
			if (_statePlayerScore.Value < _stateScoreObjective.Value)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Calcultate the gold amount to give to the player at the start of the payoff State
		/// </summary>
		private void HandleEndOfDayStateStarted(EndOfDayState shopState)
		{
            _stateCurrentTime.OnValueUpdated -= CheckInterest;

            int interest = (_playerGuildToken.Value / _runConfiguration.GuildTicketInterestValue) * _runConfiguration.GuildTicketInterestAmountPerRound;
			interest += _guildTokenTimeInterest.Value;

            _totalGuildToken.Set(_runConfiguration.GuildTicketAmountPerRound + interest);
			AddMoney(_totalGuildToken.Value);
		}

	}
}