using UnityEngine;

public class AnimatorHandler : IAnimationHandler
{
    readonly Animator animator;
    readonly MonsterStat stat;

    public AnimatorHandler(Animator _animator, MonsterStat _stat)
    {
        animator = _animator;
        stat = _stat;
    }

    public void ApplyStatToAnimator()
    {
        animator.SetFloat("AttackSpd", stat.AttackSpd.FinalValue);
        animator.SetFloat("MoveSpd", stat.MoveSpd.FinalValue);
    }

    public void SetMove(bool _isMove)
    {
        animator.SetBool("IsMove", _isMove);
    }

    public void TriggerAttack()
    {
        Debug.Log("Trigger Attack");
        animator.SetTrigger("Attack");
    }
}