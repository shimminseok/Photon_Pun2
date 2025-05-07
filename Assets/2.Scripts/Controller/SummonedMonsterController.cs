using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;


[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(AniEventListener))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MonsterNetworkReceiver))]
[RequireComponent(typeof(MonsterStat))]
[RequireComponent(typeof(Animator))]
public class SummonedMonsterController : BaseController<SummonedMonsterController>, IPunInstantiateMagicCallback
{
    public SummonObjectData MonsterData      { get; private set; }
    public AniEventListener AniEventListener { get; private set; }
    public MonsterStat      MonsterStat      { get; private set; }


    private Animator animator;
    private NavMeshAgent agent;

    private ICombatHandler combatHandler;
    private IMovementHandler movementHandler;
    private IAnimationHandler animationHandler;
    private INetworkHandler networkHandler;
    private ITargetingHandler targetingHandler;


    private void Awake()
    {
        Transform = transform;
        agent = Helper.GetComponetHelpper<NavMeshAgent>(gameObject);
        AniEventListener = Helper.GetComponetHelpper<AniEventListener>(gameObject);
        MonsterStat = Helper.GetComponetHelpper<MonsterStat>(gameObject);
        animator = Helper.GetComponetHelpper<Animator>(gameObject);
        animationHandler = new AnimatorHandler(animator, MonsterStat);
        movementHandler = new NavMeshMovementHandler(agent, transform, MonsterStat, animationHandler);
        base.Awake();

        combatHandler = new SummonObjectCombatHandler(this, MonsterStat, animationHandler);
        networkHandler = new PhotonNetworkHandler(NetworkReceiver.photonView, this);
        targetingHandler = new TargetingHandler();
    }

    private void Start()
    {
        MonsterStat.InitializeFromMonster(MonsterData);
        movementHandler.ApplyStats();
        healthBar = HealthBarManager.Instance.SpawnHealthBar(transform);
        UpdateHealtBar();
    }

    protected virtual void Update()
    {
        base.Update();
    }

    private void OnDisable()
    {
        if (healthBar != null)
            healthBar.UnLink();
    }


    protected override IState<SummonedMonsterController> GetState(ObjectState _state)
    {
        return _state switch
        {
            ObjectState.Idle   => new MonsterStates.IdleState(),
            ObjectState.Move   => new MonsterStates.MoveState(),
            ObjectState.Attack => new MonsterStates.AttackState(),
            ObjectState.Dead   => new MonsterStates.DeadState(),
            _                  => null
        };
    }


    public void Move(Vector3 _dis)
    {
        movementHandler.Move(_dis);
    }

    public void Stop(bool _isStop)
    {
        movementHandler.Stop(_isStop);
    }

    public void LookAtMoveDir()
    {
        movementHandler.LookAtMovementDir();
    }

    public void LookAtTarget(Vector3 _target)
    {
        movementHandler.LookAtTarget(_target);
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
            targetViewID = Target.PhotonViewID;


        NetworkReceiver.photonView.RPC(nameof(RPC_SetTarget), RpcTarget.Others, targetViewID);
    }

    public void HandleAttack()
    {
        animationHandler.TriggerAttack();
    }

    public void Attack()
    {
        combatHandler.Attack(Target);
    }

    public override void TakeDamage(int damage)
    {
        if (CurrentState == ObjectState.Dead) return;
        if (ActorNum != PhotonNetwork.LocalPlayer.ActorNumber)
            return;

        NetworkReceiver.photonView.RPC(nameof(RPC_TakeDamage), RpcTarget.All, damage);
    }

    public override void Die()
    {
        if (ActorNum != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            SummonManager.Instance.RemoveEnemy(this);
        }

        agent.enabled = false;
        animator.SetTrigger("Dead");
        healthBar.UnLink();
        HealthBarManager.Instance.DespawnHealthBar(healthBar);
        NetworkReceiver.photonView.RPC(nameof(RPC_DestroySync), RpcTarget.Others);
        ObjectPoolManager.Instance.ReturnObject(gameObject, 3);
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
                Target = Helper.GetComponetHelpper<ITargetable>(targetView.gameObject);
            }
        }
    }


    [PunRPC]
    public void RegisterToPool_RPC(int viewID)
    {
        PhotonView view = PhotonView.Find(viewID);
        if (view != null)
        {
            GameObject go = view.gameObject;
            go.name = view.name;
            go.SetActive(false);
            if (!ObjectPoolManager.Instance.poolObjects.TryGetValue(go.name, out var queue))
            {
                queue = new Queue<GameObject>();
                ObjectPoolManager.Instance.poolObjects[go.name] = queue;
            }

            queue.Enqueue(go);
        }
    }

    [PunRPC]
    public void RPC_SpawnSync(Vector3 _pos, int _actorNum)
    {
        agent.Warp(_pos);
        ActorNum = _actorNum;
        if (ActorNum != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            movementHandler.MirrorPosition();
            transform.rotation = NetworkReceiver.MirrorRotation(transform.rotation);
            SummonManager.Instance.EnemyList.Add(this);
            ObjectPoolManager.Instance.GetObjectSync(gameObject);
        }

        agent.avoidancePriority = UnityEngine.Random.Range(0, 50);
        gameObject.SetActive(true);
    }

    [PunRPC]
    public void RPC_DestroySync()
    {
        ObjectPoolManager.Instance.ReturnObject(gameObject, 3);
    }

    [PunRPC]
    public override void RPC_TakeDamage(int _damage)
    {
        if (CurrentState == ObjectState.Dead) return;


        MonsterStat.CurrentHP.ModifyAllValue(_damage);
        UpdateHealtBar();
        if (MonsterStat.CurrentHP.FinalValue <= 0)
        {
            ChangeState(ObjectState.Dead);
        }
    }

    protected override void UpdateHealtBar()
    {
        healthBar.UpdateFill(MonsterStat.CurrentHP.FinalValue, MonsterStat.MaxHP.FinalValue);
    }


    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instData = NetworkReceiver.photonView.InstantiationData;
        MonsterData = TableManager.Instance.GetTable<MonsterTable>().GetDataByID((int)instData[0]);
        gameObject.name = MonsterData.Prefabs.name;
    }
}