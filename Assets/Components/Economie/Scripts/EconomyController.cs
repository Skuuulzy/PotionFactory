using System;
using UnityEngine;
using VComponent.Tools.Singletons;

namespace Components.Economy
{
	public class EconomyController : Singleton<EconomyController>
	{
		[SerializeField] private int _playerMoney;
		[SerializeField] private RunConfiguration _runConfiguration;

		private int _stateMoneyObjecctive;

		public int PlayerMoney => _playerMoney;
		public static Action<int> OnPlayerMoneyUpdated;
		public static Action<int> OnMoneyStateObjectiveUpdated;

		private void Start()
		{
			PlanningFactoryState.OnPlanningFactoryStateStarted += SetMoneyStateObjective;
		}

		private void OnDestroy()
		{
			PlanningFactoryState.OnPlanningFactoryStateStarted -= SetMoneyStateObjective;
		}

		public void AddMoney(int amount)
		{
			_playerMoney += amount;
			
			OnPlayerMoneyUpdated?.Invoke(_playerMoney);
		}
		
		public void DecreaseMoney(int amount)
		{
			_playerMoney -= amount;
			
			OnPlayerMoneyUpdated?.Invoke(_playerMoney);
		}

		public void SetMoneyStateObjective(PlanningFactoryState state)
		{
			_stateMoneyObjecctive = _runConfiguration.RunStateList.Find(x => x.StateNumber == state.StateIndex).MoneyObjective;
			OnMoneyStateObjectiveUpdated?.Invoke(_stateMoneyObjecctive);
		}
	}
}