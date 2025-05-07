using Photon.Pun;
using UnityEngine;

public class SummonObjectCombatHandler : ICombatHandler
{
    readonly SummonedMonsterController m_Controller;
    readonly MonsterStat stat;
    readonly IAnimationHandler animation;

    public SummonObjectCombatHandler(SummonedMonsterController _controller, MonsterStat _stat, IAnimationHandler _ani)
    {
        m_Controller = _controller;
        stat = _stat;
        animation = _ani;
    }

    public void Attack(ITargetable target)
    {
        if (target == null || target.IsDead)
            return;

        int damage = Mathf.RoundToInt(stat.Attack.FinalValue);

        target.TakeDamage(damage);
    }
}