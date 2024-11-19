using Components.Grid.Tile;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Grid.Obstacle
{
	public class ObstacleManager : MonoBehaviour
	{

		[Header("Templates")]
		[SerializeField] private List<ObstacleTemplate> _obstacleTemplateList;
		[Header("Selector View")]
		[SerializeField] private ObstacleSelectorView _obstacleSelectorView;
		[SerializeField] private Transform _obstacleSelectorViewHolder;


		public ObstacleTemplate SelectedObstacle { get; private set; }

		public static Action<ObstacleTemplate> OnChangeSelectedObstacle;
		// Start is called before the first frame update
		void Start()
		{
			if (_obstacleTemplateList.Count <= 0)
			{
				Debug.LogWarning("[Obstacle] No templates found.");
				return;
			}

			foreach (var tile in _obstacleTemplateList)
			{
				ObstacleSelectorView selectorView = Instantiate(_obstacleSelectorView, _obstacleSelectorViewHolder);
				selectorView.Init(tile);
				selectorView.OnSelected += HandleObstacleSelected;
			}

			// Init selected machine has the first 
			SelectedObstacle = _obstacleTemplateList[0];
		}


		private void HandleObstacleSelected(ObstacleTemplate obstacle)
		{
			SelectedObstacle = obstacle;
			OnChangeSelectedObstacle?.Invoke(obstacle);
		}

	}
}

