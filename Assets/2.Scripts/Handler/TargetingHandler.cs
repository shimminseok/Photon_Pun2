using UnityEngine;

public class TargetingHandler : ITargetingHandler
{
    public ITargetable FindEnemy()
    {
        return SummonManager.Instance.FindEnemy();
    }
}
