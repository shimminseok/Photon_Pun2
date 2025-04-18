using UnityEngine;
using UnityEngine.InputSystem.XR;


namespace MonsterStates
{
    public class IdleState : IState<SummonedMonsterController>
    {
        public void Enter(SummonedMonsterController _controller)
        {
            _controller.Stop(true);
        }

        public void Execute(SummonedMonsterController _controller)
        {

            _controller.FindEnemy();
            if (_controller.Target != null)
            {
                _controller.ChangeState(ObjectState.Move);
            }
        }
        public void Exit(SummonedMonsterController _controller)
        {
        }
    }
    public class MoveState : IState<SummonedMonsterController>
    {
        public void Enter(SummonedMonsterController _controller)
        {
            _controller.Stop(false);
            _controller.Move(_controller.Target.transform.position);
        }

        public void Execute(SummonedMonsterController _controller)
        {
            if(_controller.Target == null || _controller.Target.CurrentState == ObjectState.Dead)
            {
                _controller.ChangeState(ObjectState.Idle);
                return;
            }
            float distance = Vector3.Distance(_controller.transform.position, _controller.Target.transform.position);
            if (distance <= _controller.MonsterStat.AttackRange.FinalValue)
            {
                _controller.ChangeState(ObjectState.Attack);
            }
            else
            {
                _controller.Move(_controller.Target.transform.position);
                _controller.LookAtMoveDir();
            }
        }

        public void Exit(SummonedMonsterController _controller)
        {
            _controller.Stop(true);
        }
    }
    public class AttackState : IState<SummonedMonsterController>
    {
        float attackTimer;
        float attackDelay;
        public void Enter(SummonedMonsterController _controller)
        {
            attackTimer = 0f;
            float attackSpd = _controller.MonsterStat.AttackSpd.FinalValue;
            attackDelay = attackSpd > 0 ? 1f / attackSpd : 1f;
        }

        public void Execute(SummonedMonsterController _controller)
        {
            if (_controller.Target == null || _controller.Target.CurrentState == ObjectState.Dead)
            {
                _controller.ChangeState(ObjectState.Idle);
                return;
            }

            float distance = Vector3.Distance(_controller.transform.position, _controller.Target.transform.position);
            if (distance > _controller.MonsterStat.AttackRange.FinalValue)
            {
                _controller.ChangeState(ObjectState.Move);
                return;
            }
            _controller.LookAtTarget(_controller.Target.transform.position);
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackDelay)
            {
                attackTimer = 0f;
                _controller.HandleAttack();
            }
        }

        public void Exit(SummonedMonsterController _controller)
        {
            Debug.Log("Exit Attack");
        }
    }
    public class DeadState : IState<SummonedMonsterController>
    {
        public void Enter(SummonedMonsterController _controller)
        {
            _controller.Die();
        }

        public void Execute(SummonedMonsterController _controller)
        {
        }

        public void Exit(SummonedMonsterController _controller)
        {
            Debug.Log("Dead Exit");
        }
    }
}
