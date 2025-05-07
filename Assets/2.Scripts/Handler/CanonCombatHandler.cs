using System.Collections.Generic;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;

public class CanonCombatHandler : MonoBehaviour, ICombatHandler
{
    private CanonController controller;
    private CanonStat stat;
    public CanonCombatHandler(CanonController _controller, CanonStat _stats)
    {
        controller = _controller;
        stat = _stats;
    }
    public void Attack(ITargetable target)
    {
        if (target == null || target.IsDead)
            return;
        
        int damage = Mathf.RoundToInt(stat.Attack.FinalValue);


        target.TakeDamage(damage);
    }
}
