using Components.Grid.Tile;
using Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Components.Grid.Obstacle
{
	public class ObstacleManager : MonoBehaviour
	{

		[Header("Templates")]
		[SerializeField] private List<ObstacleTemplate> _obstacleTemplateList;
		[Header("Selector View")]
		[SerializeField] private ObstacleSelectorView _obstacleSelectorView;
		[SerializeField] private SerializableDictionary<ObstacleCategory, Transform> _obstacleSelectorViewHolder;


		public ObstacleTemplate SelectedObstacle { get; private set; }

		public static Action<ObstacleTemplate> OnChangeSelectedObstacle;
		// Start is called before the first frame update
		void Start()
		{
			_obstacleTemplateList = ScriptableObjectDatabase.GetAllScriptableObjectOfType<ObstacleTemplate>().ToList();
			if (_obstacleTemplateList.Count <= 0)
			{
				Debug.LogWarning("[Obstacle] No templates found.");
				return;
			}

			foreach (var obstacle in _obstacleTemplateList)
			{
				ObstacleSelectorView selectorView = Instantiate(_obstacleSelectorView, _obstacleSelectorViewHolder[obstacle.Category]);
				selectorView.Init(obstacle);
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

