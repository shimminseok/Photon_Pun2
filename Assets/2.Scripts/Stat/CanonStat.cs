using UnityEngine;

public class CanonStat : MonoBehaviour
{
    public Stat MaxHp       { get; } = new Stat(StatType.MaxHP);
    public Stat Attack      { get; } = new Stat(StatType.AttackPow);
    public Stat AttackRange { get; } = new Stat(StatType.AttackRange);
    public Stat AttackSpd   { get; } = new Stat(StatType.AttackSpd);
    public Stat Defense     { get; } = new Stat(StatType.Defense);
    public Stat CurrentHp   { get; } = new Stat(StatType.CurrentHP);

    void Start()
    {
        InitializeStat();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void InitializeStat()
    {
        MaxHp.ModifyBaseValue(1500);
        CurrentHp.ModifyBaseValue(MaxHp.BaseValue,0, MaxHp.BaseValue);
        Attack.ModifyBaseValue(50);
        AttackRange.ModifyBaseValue(5);
        AttackSpd.ModifyBaseValue(0.5f);
        Defense.ModifyBaseValue(50);
    }
}
