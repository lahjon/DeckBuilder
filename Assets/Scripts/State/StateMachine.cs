using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    public State state;
    public bool actionInProgress;

    public void SetState(State _state)
    {
        state = _state;
        StartCoroutine(state.Start());
    }

    public void EndState()
    {
        StartCoroutine(state.End());
    }
    public void ActionState()
    {
        StartCoroutine(state.Action());
    }

}
