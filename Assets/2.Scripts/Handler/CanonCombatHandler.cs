using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;

public class CanonCombatHandler : ICombatHandler
{
    private CanonController controller;
    private CanonStat stat;

    public CanonCombatHandler(CanonController controller, CanonStat _stats)
    {
        this.controller = controller;
        stat = _stats;
    }

    public void Attack(ITargetable target)
    {
        if (target == null || target.IsDead)
            return;


        GameObject muzzle = ObjectPoolManager.Instance.GetObject("Muzzle");
        if (muzzle == null)
            return;

        Muzzle muzzleComponent = muzzle.GetComponent<Muzzle>();
        if (muzzleComponent == null)
            return;

        int damage = Mathf.RoundToInt(stat.Attack.FinalValue);

        muzzleComponent.SetTarget(controller, target.Transform.gameObject, target.TakeDamage, damage);
    }
}