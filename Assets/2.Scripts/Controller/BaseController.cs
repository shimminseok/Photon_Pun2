using System;
using MonsterStates;
using UnityEngine;
using Photon.Pun;

public abstract class BaseController<TController> : MonoBehaviour, ITargetable
    where TController : BaseController<TController>
{
    public BaseNetworkReceiver<TController> NetworkReceiver { get; private set; }
    public ITargetable                      Target          { get; protected set; }
    public Transform                        Transform       { get; protected set; }
    public int                              PhotonViewID    { get; private set; }
    public bool                             IsDead          { get; private set; }


    public int         ActorNum     { get; protected set; }
    public ObjectState CurrentState { get; private set; }

    public StateMachine<TController> StateMachine { get; private set; }
    private IState<TController>[] states;
    protected HPBarUI healthBar;


    protected virtual void Awake()
    {
        NetworkReceiver = Helper.GetComponetHelpper<BaseNetworkReceiver<TController>>(gameObject);
        PhotonViewID = NetworkReceiver.photonView.ViewID;
        SetupState();
    }

    protected virtual void Update()
    {
        if (ActorNum != PhotonNetwork.LocalPlayer.ActorNumber)
            return;

        StateMachine.Excute();
    }

    private void SetupState()
    {
        states = new IState<TController>[Enum.GetValues(typeof(ObjectState)).Length];
        for (int i = 0; i < states.Length; i++)
        {
            var state = GetState((ObjectState)i);
            if (state == null)
                continue;

            states[i] = state;
        }

        StateMachine = new StateMachine<TController>();
        StateMachine.Setup((TController)this, states[(int)ObjectState.Idle]);
    }

    protected abstract void                UpdateHealtBar();
    protected abstract IState<TController> GetState(ObjectState _state);

    public abstract void Die();


    public void ChangeState(ObjectState _state)
    {
        IsDead = _state == ObjectState.Dead;
        if (CurrentState == ObjectState.Dead)
        {
            return;
        }

        StateMachine.ChangeState(states[(int)_state]);
        CurrentState = _state;
    }

    public abstract void TakeDamage(int _damage);

    public abstract void RPC_TakeDamage(int _damage);
}