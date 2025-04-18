using Photon.Pun;
using UnityEngine;

public class CombatHandler : ICombatHandler
{
    readonly SummonedMonsterController m_Controller;
    readonly MonsterStat stat;
    readonly IAnimationHandler animation;

    public CombatHandler(SummonedMonsterController _controller, MonsterStat _stat, IAnimationHandler _ani)
    {
        m_Controller = _controller;
        stat = _stat;
        animation = _ani;
    }

    public void Attack(SummonedMonsterController target)
    {
        if (target == null || target.CurrentState == ObjectState.Dead)
            return;

        int damage = Mathf.RoundToInt(stat.Attack.FinalValue);

        target.TakeDamage(damage);
    }
}