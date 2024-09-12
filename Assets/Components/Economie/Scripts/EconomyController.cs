using System;
using UnityEngine;
using VComponent.Tools.Singletons;

namespace Components.Economy
{
	public class EconomyController : Singleton<EconomyController>
	{
		[SerializeField] private int _playerMoney;

		public int PlayerMoney => _playerMoney;
		public static Action<int> OnPlayerMoneyUpdated;

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
	}
}