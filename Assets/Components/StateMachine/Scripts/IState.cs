using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState 
{

    void SetName();
    void OnEnter();
    void Update();
    void FixedUpdate();
    void OnExit();
}
