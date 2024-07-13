using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Components.Tick
{
    public class OptionTickView : MonoBehaviour, ITickable
    {
        [SerializeField] private TextMeshProUGUI _tickText;
        [SerializeField] private int _moduloNumber;

        private int _tickCount;
        
        // Start is called before the first frame update
        void Start()
        {
            _tickText.text = "Tick : " + _tickCount + " modulo " + _moduloNumber;
            TickSystem.AddTickable(this);
        }
        
        public void Tick()
		{
			_tickCount++;

            if(_tickCount % _moduloNumber == 0)
			{
                _tickText.text = "Tick : " + _tickCount / _moduloNumber + " modulo " + _moduloNumber;
            }
		}
    }
}
