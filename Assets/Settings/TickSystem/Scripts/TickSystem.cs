using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TickSystem : MonoBehaviour
{
	public class OnTickEventArgs : EventArgs
	{
		public int _tick;
	}
	private float _tickTimer = 0f;
	private int _tick;
	[SerializeField] private  float _tickTimerMax = 0.2f;

	[SerializeField] private SerializableDictionary<int ,TextMeshProUGUI> _timerTextList;


	public static EventHandler<OnTickEventArgs> OnTick;
	private void Update()
	{
		_tickTimer += Time.deltaTime;
		while (_tickTimer >= _tickTimerMax)
		{
			_tickTimer -= _tickTimerMax;
			_tick++;
			DisplayTime();
			OnTick?.Invoke(this, new OnTickEventArgs { _tick = _tick });
		}
	}

	private void DisplayTime()
	{
		_timerTextList[1].text = "1 Tick = " + _tick;

		if (_tick % 5 == 0)
		{
			if (_timerTextList.ContainsKey(5))
			{
				_timerTextList[5].text = "5 Tick = " + _tick / 5;
			}
		}

		if (_tick % 10 == 0)
		{
			if (_timerTextList.ContainsKey(10))
			{
				_timerTextList[10].text = "10 Tick = " + _tick / 10;
			}
		}

		if (_tick % 20 == 0)
		{
			if (_timerTextList.ContainsKey(20))
			{
				_timerTextList[20].text = "20 Tick = " + _tick / 20;
			}
			
		}
	}

	public void ChangeTimeSpeed(int value)
	{
		Time.timeScale =  value;
	}
}
