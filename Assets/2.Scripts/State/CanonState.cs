using UnityEngine;

namespace CanonState
{
    public class IdleState : IState<CanonController>
    {
        private Stat attackRange;

        public IdleState(Stat _attackRangeStat)
        {
            attackRange = _attackRangeStat;
        }

        public void Enter(CanonController controller)
        {
        }

        public void Execute(CanonController controller)
        {
            controller.FindEnemy();
            if (controller.Target != null)
            {
                float distance = Vector3.Distance(controller.transform.position, controller.Target.Transform.localPosition);

                if (distance < attackRange.FinalValue)
                {
                    controller.ChangeState(ObjectState.Attack);
                }
            }
        }

        public void Exit(CanonController controller)
        {
        }
    }

    public class AttackState : IState<CanonController>
    {
        private readonly Stat attackRange;
        private readonly Stat attackSpd;

        float attackTimer;
        float attackDelay;

        public AttackState(Stat _attackRangStat, Stat _attackSpdStat)
        {
            attackRange = _attackRangStat;
            attackSpd = _attackSpdStat;
        }

        public void Enter(CanonController controller)
        {
            attackTimer = 0f;
            float attackSpd = this.attackSpd.FinalValue;
            attackDelay = attackSpd > 0 ? 1f / attackSpd : 1f;
        }

        public void Execute(CanonController controller)
        {
            if (controller.Target == null || controller.Target.IsDead)
            {
                controller.ChangeState(ObjectState.Idle);
                return;
            }

            float distance = Vector3.Distance(controller.transform.position, controller.Target.Transform.position);
            if (distance > attackRange.FinalValue)
            {
                controller.ChangeState(ObjectState.Idle);
                return;
            }

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackDelay)
            {
                attackTimer = 0f;
                controller.Attack();
            }
        }

        public void Exit(CanonController controller)
        {
            Debug.Log("Exit Attack");
        }
    }

    public class DestroyedState : IState<CanonController>
    {
        public void Enter(CanonController controller)
        {
            controller.Die();
        }

        public void Execute(CanonController controller)
        {
        }

        public void Exit(CanonController controller)
        {
            Debug.Log("Dead Exit");
        }
    }
}