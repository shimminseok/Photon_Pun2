using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class NavMeshMovementHandler : IMovementHandler
{
    private readonly NavMeshAgent agent;
    private readonly Transform transform;
    private readonly MonsterStat stat;
    private readonly IAnimationHandler animator;
    public NavMeshMovementHandler(NavMeshAgent _agent, Transform _trans,MonsterStat _stat, IAnimationHandler _animator)
    {
        agent = _agent;
        transform = _trans;
        stat = _stat;
        animator = _animator;
        agent.updateRotation = false;
    }
    public void Move(Vector3 _pos)
    {
        agent.SetDestination(_pos);
    }

    public void MirrorPosition()
    {
        agent.Warp(new Vector3(transform.position.x, transform.position.y, -transform.position.z));
    }
    public void ApplyStatToAgent()
    {
        agent.speed = stat.MoveSpd.FinalValue;
        agent.stoppingDistance = stat.AttackRange.FinalValue;
    }

    public void Stop(bool _isStop)
    {
        animator.SetMove(!_isStop);
        agent.isStopped = _isStop;
    }

    public void LookAt(Vector3 targetPos)
    {
        if(agent.hasPath && agent.velocity.sqrMagnitude > 0.01f)
        {
            Vector3 moveDir = agent.velocity.normalized;
            moveDir.y = 0;
            transform.forward = moveDir;
        }
    }
    public void ApplyStats()
    {
        agent.speed = stat.MoveSpd.FinalValue;
        agent.stoppingDistance = stat.AttackRange.FinalValue;
    }

    public void LookAtMovementDir()
    {
        if (agent.hasPath && agent.velocity.sqrMagnitude > 0.01f)
        {
            Vector3 moveDir = agent.velocity.normalized;
            moveDir.y = 0;
            transform.forward = moveDir;
        }
    }
    public void LookAtTarget(Vector3 _target)
    {
        Vector3 dir = _target - transform.position;
        dir.y = 0;
        transform.forward = dir.normalized;
    }
}
