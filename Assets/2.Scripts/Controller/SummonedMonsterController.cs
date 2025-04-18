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
[RequireComponent(typeof(ObjectNetworkReceiver))]
[RequireComponent(typeof(MonsterStat))]
[RequireComponent(typeof(Animator))]
public class SummonedMonsterController : MonoBehaviour, IPunInstantiateMagicCallback
{
    public SummonObjectData MonsterData { get; private set; }
    public AniEventListener AniEventListener { get; private set; }
    public ObjectNetworkReceiver NetworkReceiver { get; private set; }
    public MonsterStat MonsterStat { get; private set; }
    public ObjectState CurrentState { get; private set; }
    public SummonedMonsterController Target { get; private set; }
    public int ActorNum { get; private set; }


    private Animator animator;
    private NavMeshAgent agent;
    private StateMachine<SummonedMonsterController> stateMachine;
    private IState<SummonedMonsterController>[] states;
    private HPBarUI healthBar;

    private ICombatHandler combatHandler;
    private IMovementHandler movementHandler;
    private IAnimationHandler animationHandler;
    private INetworkHandler networkHandler;
    private ITargetingHandler targetingHandler;


    private void Awake()
    {
        AniEventListener = Helper.GetComponetHelpper<AniEventListener>(gameObject);
        agent = Helper.GetComponetHelpper<NavMeshAgent>(gameObject);
        NetworkReceiver = Helper.GetComponetHelpper<ObjectNetworkReceiver>(gameObject);
        MonsterStat = Helper.GetComponetHelpper<MonsterStat>(gameObject);
        animator = Helper.GetComponetHelpper<Animator>(gameObject);


        animationHandler = new AnimatorHandler(animator, MonsterStat);
        movementHandler = new NavMeshMovementHandler(agent, transform, MonsterStat, animationHandler);
        combatHandler = new CombatHandler(this, MonsterStat, animationHandler);
        networkHandler = new PhotonNetworkHandler(NetworkReceiver.photonView, this);
        targetingHandler = new TargetingHandler();

        SetupState();
    }

    private void Start()
    {
        //리스폰
        MonsterStat.InitializeFromMonster(MonsterData);
        movementHandler.ApplyStats();
        healthBar = HealthBarManager.Instance.SpawnHealthBar(transform);
        UpdateHealtBar();
        //
    }

    private void Update()
    {
        if (ActorNum != PhotonNetwork.LocalPlayer.ActorNumber)
            return;


        Updated();
    }

    private void OnDisable()
    {
        if (healthBar != null)
            healthBar.UnLink();
    }

    private void SetupState()
    {
        states = new IState<SummonedMonsterController>[Enum.GetValues(typeof(ObjectState)).Length];
        for (int i = 0; i < states.Length; i++)
        {
            states[i] = GetState((ObjectState)i);
        }

        stateMachine = new StateMachine<SummonedMonsterController>();
        stateMachine.Setup(this, states[(int)ObjectState.Idle]);
    }

    IState<SummonedMonsterController> GetState(ObjectState _state)
    {
        return _state switch
        {
            ObjectState.Idle => new MonsterStates.IdleState(),
            ObjectState.Move => new MonsterStates.MoveState(),
            ObjectState.Attack => new MonsterStates.AttackState(),
            ObjectState.Dead => new MonsterStates.DeadState(),
            _ => null
        };
    }

    public void Updated()
    {
        stateMachine.Excute();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void ChangeState(ObjectState _newState)
    {
        if (CurrentState == ObjectState.Dead)
            return;

        stateMachine.ChangeState(states[(int)_newState]);
        CurrentState = _newState;
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

        if (newTarget == Target) // 타겟이 같으면 안보냄.
            return;

        Target = newTarget;
        int targetViewID = -1;

        if (Target != null)
            targetViewID = Target.NetworkReceiver.photonView.ViewID;


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

    public void TakeDamage(int damage)
    {
        if (CurrentState == ObjectState.Dead) return;
        if (ActorNum != PhotonNetwork.LocalPlayer.ActorNumber)
            return;

        NetworkReceiver.photonView.RPC(nameof(RPC_TakeDamage), RpcTarget.All, damage);
    }

    public void Die()
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

    void UpdateHealtBar()
    {
        healthBar.UpdateFill(MonsterStat.CurrentHP.FinalValue, MonsterStat.MaxHP.FinalValue);
    }

    /// <summary>
    /// 나의 타겟을 RPC로 다른 오브젝트들에게 보내기
    /// 이유 : 각 오브젝트마다 Target이 다를것이고, 다른 Target으로 이동해야함.
    /// </summary>
    /// <param name="_viewID"></param>
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
                Target = Helper.GetComponetHelpper<SummonedMonsterController>(targetView.gameObject);
            }
        }
    }

    /// <summary>
    /// RPC 로 해당 오브젝트의 State를 보내고 해당 RPC를 받은 상대 클라이언트는 해당 State로
    /// 오브젝트의 상태를 변경함
    /// 사용하지 않는 이유 :
    /// RPC로 하면 지연이 발생. 네트워크에 트래픽을 증가시킬수가있음
    /// </summary>
    /// <param name="_state"></param>

    //[PunRPC]
    //public void RPC_ChangeState(int _state)
    //{
    //    ChangeState((ObjectState)_state, true);
    //}
    [PunRPC]
    public void RPC_TakeDamage(int _damage)
    {
        if (CurrentState == ObjectState.Dead) return;


        MonsterStat.CurrentHP.ModifyAllValue(_damage);
        UpdateHealtBar();
        if (MonsterStat.CurrentHP.FinalValue <= 0)
        {
            ChangeState(ObjectState.Dead);
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
        //내꺼 아니면 반대로 보내버림
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

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instData = NetworkReceiver.photonView.InstantiationData;
        MonsterData = TableManager.Instance.GetTable<MonsterTable>().GetDataByID((int)instData[0]);
        gameObject.name = MonsterData.Prefabs.name;
    }
}