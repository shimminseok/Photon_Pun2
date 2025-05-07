using System.Collections;
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

        GameObject muzzle = ObjectPoolManager.Instance.GetObject("Muzzle");
        if (muzzle == null)
            return;

        Muzzle muzzleComponent = muzzle.GetComponent<Muzzle>();
        if (muzzleComponent == null)
            return;

        int damage = Mathf.RoundToInt(stat.Attack.FinalValue);
        // target.TakeDamage(damage);
        muzzleComponent.SetTarget(target.Transform.gameObject, target.TakeDamage, damage);
    }

    IEnumerator GetMuzzle()
    {
        yield return null;
    }
}