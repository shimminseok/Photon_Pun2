using MonsterStates;
using UnityEngine;

public class StateMachine<T> where T : class
{
    T ownerEntity;
    IState<T> currentState;

    public void Setup(T owner, IState<T> entryState)
    {
        ownerEntity = owner;
        ChangeState(entryState);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void Excute()
    {
        if (currentState != null)
        {
            currentState.Execute(ownerEntity);
        }
    }

    public void ChangeState(IState<T> _newState)
    {
        if (_newState == null)
            return;


        if (currentState != null)
        {
            currentState.Exit(ownerEntity);
        }

        currentState = _newState;
        currentState.Enter(ownerEntity);
    }
}