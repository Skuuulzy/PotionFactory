using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayoffController : MonoBehaviour
{

    public static Action OnPayoffConfirm;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Confirm()
	{
        OnPayoffConfirm?.Invoke();
    }
}
