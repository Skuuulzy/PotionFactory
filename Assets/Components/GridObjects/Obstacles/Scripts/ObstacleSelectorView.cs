using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Components.Grid.Obstacle
{
    public class ObstacleSelectorView : MonoBehaviour
    {
		[SerializeField] private TMP_Text _name;
		[SerializeField] private Image _background;
		public Action<ObstacleTemplate> OnSelected;

		private ObstacleTemplate _obstacle;

		public void Init(ObstacleTemplate obstacle)
		{
			_obstacle = obstacle;

			_name.text = obstacle.Name;
			_background.sprite = obstacle.UIView;
		}

		public void Select()
		{
			OnSelected?.Invoke(_obstacle);
		}
	}
}
