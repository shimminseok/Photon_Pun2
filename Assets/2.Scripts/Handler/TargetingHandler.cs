using UnityEngine;

public class TargetingHandler : ITargetingHandler
{
    public SummonedMonsterController FindEnemy()
    {
        return SummonManager.Instance.FindEnemy();
    }
}
