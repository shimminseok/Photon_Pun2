using System;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CanonStat))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(CanonNetworkReceiver))]
public class CanonController : BaseController<CanonController>
{
    private ITargetingHandler targetingHandler;
    private ICombatHandler combatHandler;
    private CanonStat stat;


    private IState<CanonController>[] states;


    protected virtual void Awake()
    {
        Transform = transform;
        stat = Helper.GetComponetHelpper<CanonStat>(gameObject);
        base.Awake();

        targetingHandler = new TargetingHandler();
        combatHandler = new CanonCombatHandler(this, stat);
    }

    private void Start()
    {
        healthBar = HealthBarManager.Instance.SpawnHealthBar(transform);
        UpdateHealtBar();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        base.Update();
    }

    protected override void UpdateHealtBar()
    {
        healthBar.UpdateFill(stat.CurrentHp.FinalValue, stat.MaxHp.FinalValue);
    }

    protected override IState<CanonController> GetState(ObjectState _state)
    {
        return _state switch
        {
            ObjectState.Idle   => new CanonState.IdleState(),
            ObjectState.Attack => new CanonState.AttackState(stat.AttackRange, stat.AttackSpd),
            ObjectState.Dead   => new CanonState.DestroyedState(),
            _                  => null
        };
    }

    public void FindEnemy()
    {
        if (ActorNum != PhotonNetwork.LocalPlayer.ActorNumber)
            return;


        var newTarget = targetingHandler.FindEnemy();

        if (newTarget == Target)
            return;

        Target = newTarget;
        int targetViewID = -1;

        if (Target != null)
        {
            targetViewID = Target.PhotonViewID;
        }


        NetworkReceiver.photonView.RPC(nameof(RPC_SetTarget), RpcTarget.Others, targetViewID);
    }

    public void Attack()
    {
        //TODO : 포탄 생성

        Debug.Log("포탑 공격중!!!");
        combatHandler.Attack(Target);
    }

    public override void Die()
    {
        //TODO: 파괴 로직
        //게임 패배
        Debug.Log("파괴 패배!!");
    }


    [PunRPC]
    public void RPC_SetTarget(int _viewID)
    {
        Debug.Log($"Receive Set TargetRPC : {_viewID}");
        if (_viewID == -1)
        {
            Target = null;
        }
        else
        {
            PhotonView targetView = PhotonView.Find(_viewID);
            if (targetView != null)
            {
                Target = targetView.GetComponent<ITargetable>();
            }
        }
    }

    [PunRPC]
    public void RPC_SpawnSync(Vector3 _pos, int _actorNum)
    {
        ActorNum = _actorNum;
        if (ActorNum != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            transform.rotation = NetworkReceiver.MirrorRotation(transform.rotation);
            _pos.z *= -1;
            SummonManager.Instance.EnemyCanon = this;
        }

        transform.position = _pos;
    }

    public override void TakeDamage(int damage)
    {
        if (CurrentState == ObjectState.Dead) return;
        if (ActorNum != PhotonNetwork.LocalPlayer.ActorNumber)
            return;

        NetworkReceiver.photonView.RPC(nameof(RPC_TakeDamage), RpcTarget.All, damage);
    }

    [PunRPC]
    public override void RPC_TakeDamage(int _damage)
    {
        if (CurrentState == ObjectState.Dead) return;


        stat.CurrentHp.ModifyAllValue(_damage);
        UpdateHealtBar();
        if (stat.CurrentHp.FinalValue <= 0)
        {
            ChangeState(ObjectState.Dead);
        }
    }
}