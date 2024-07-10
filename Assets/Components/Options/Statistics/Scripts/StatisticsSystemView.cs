using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsSystemView : MonoBehaviour
{

    [SerializeField] private StatisticsSystem _statisticsSystem;
    [SerializeField] private StatisticView _statisticViewPrefab;
    [SerializeField] private Transform _parent;

    private List<StatisticView> _statisticsViews;
    // Start is called before the first frame update
    void Start()
    {
        _statisticsViews = new List<StatisticView>();
        _statisticsSystem.OnMachineAddedToStat += AddMachine;
        _statisticsSystem.OnMachineRemovedToStat += RemoveMachine;
    }

	private void OnDestroy()
	{
        _statisticsSystem.OnMachineAddedToStat -= AddMachine;
        _statisticsSystem.OnMachineRemovedToStat -= RemoveMachine;
    }

	private void AddMachine(string name, int value, int tickRate)
	{
        for(int i = 0; i < _statisticsViews.Count; i++)
		{
            //Already contains a statistic view associated to this machine
            if(_statisticsViews[i].MachineName == name)
			{
                _statisticsViews[i].UpdateStatisticView(value);
                return;
			}
		}

        //Create a statistic view
        StatisticView stat = Instantiate(_statisticViewPrefab, _parent);
        stat.Init(name, value, tickRate);
        _statisticsViews.Add(stat);
	}

    private void RemoveMachine(string name, int value, int tickRate)
	{
        for (int i = 0; i < _statisticsViews.Count; i++)
        {
            //Already contains a statistic view associated to this machine
            if (_statisticsViews[i].MachineName == name)
            {
                _statisticsViews[i].UpdateStatisticView(value);
                return;
            }
        }
    }
}
