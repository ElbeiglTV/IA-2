using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State <T> where T : System.Enum
{
    public FSM<T> fsm;

    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void OnUpdate();

}
