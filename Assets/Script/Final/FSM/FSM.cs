using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM <T> where T : System.Enum
{
    State<T> currentState;
    Dictionary<T, State<T>> states = new Dictionary<T, State<T>>();

    public void AddState(T id, State<T> state)
    {
        states.Add(id, state);
        state.fsm = this;
    }

    public void OnUpdate()
    {
        currentState.OnUpdate();
    }

    public void ChangeState(T id)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }
        currentState = states[id];
        currentState.OnEnter();
    }



}
  