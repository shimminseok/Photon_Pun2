using UnityEngine;

namespace CanonState
{
public class IdleState : IState<CanonController>
    {
        public void Enter(CanonController _controller)
        {
            // _controller.Stop(true);
        }

        public void Execute(CanonController _controller)
        {

            _controller.FindEnemy();
            if (_controller.Target != null)
            {
                _controller.ChangeState(ObjectState.Move);
            }
        }
        public void Exit(CanonController _controller)
        {
        }
    }
    public class AttackState : IState<CanonController>
    {
        private readonly Stat attackStat;
        private readonly Stat attackRange;
        private readonly Stat attackSpd;

        float attackTimer;
        float attackDelay;
        
        public AttackState(Stat _attackStat, Stat _attackRangStat, Stat _attackSpdStat)
        {
            attackStat = _attackStat;
            attackRange = _attackRangStat;
            attackSpd = _attackSpdStat;
        }
        public void Enter(CanonController _controller)
        {
            attackTimer = 0f;
            float attackSpd = this.attackSpd.FinalValue;
            attackDelay = attackSpd > 0 ? 1f / attackSpd : 1f;
        }

        public void Execute(CanonController _controller)
        {
            if (_controller.Target == null || _controller.Target.IsDead)
            {
                _controller.ChangeState(ObjectState.Idle);
                return;
            }

            float distance = Vector3.Distance(_controller.transform.position, _controller.Target.Transform.position);
            if (distance > attackRange.FinalValue)
            {
                _controller.ChangeState(ObjectState.Move);
                return;
            }
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackDelay)
            {
                attackTimer = 0f;
                _controller.Attack();
            }
        }

        public void Exit(CanonController _controller)
        {
            Debug.Log("Exit Attack");
        }
    }
    public class DestroyedState : IState<CanonController>
    {
        public void Enter(CanonController _controller)
        {
            _controller.Die();
        }

        public void Execute(CanonController _controller)
        {
        }

        public void Exit(CanonController _controller)
        {
            Debug.Log("Dead Exit");
        }
    }
}
