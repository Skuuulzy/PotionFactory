using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TacticaleViewController : MonoBehaviour
{
    [SerializeField] Image _2DView;
    
    public void IsAvailable(bool value)
	{
        _2DView.color = value ? Color.green : Color.red;
	}
}
