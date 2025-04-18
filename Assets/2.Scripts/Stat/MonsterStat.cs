using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class MonsterStat : MonoBehaviour
{
    public Stat MaxHP { get; private set; } = new Stat(StatType.MaxHP);
    public Stat Attack { get; private set; } = new Stat(StatType.AttackPow);
    public Stat Defense { get; private set; } = new Stat(StatType.Defense);
    public Stat MoveSpd { get; private set; } = new Stat(StatType.MoveSpd);
    public Stat AttackSpd { get; private set; } = new Stat(StatType.AttackSpd);
    public Stat CurrentHP { get; private set; } = new Stat(StatType.CurrentHP);
    public Stat AttackRange { get; private set; } = new Stat(StatType.AttackRange);

    //public Dictionary<StatType, Stat> Stats = new Dictionary<StatType, Stat>();
    protected virtual void Start()
    {

    }

    public void InitializeFromMonster(SummonObjectData _monsterData)
    {
        MaxHP.ModifyBaseValue(_monsterData.MaxHP);
        CurrentHP.ModifyBaseValue(_monsterData.MaxHP,0, _monsterData.MaxHP);
        Attack.ModifyBaseValue(_monsterData.AttackPow);
        Defense.ModifyBaseValue(_monsterData.Defense);
        AttackSpd.ModifyBaseValue(_monsterData.AttackSpd);
        AttackRange.ModifyBaseValue(_monsterData.AttackRange);
        //CriticalChance.ModifyBaseValue(_monsterData.CriticalChance);
        MoveSpd.ModifyBaseValue(_monsterData.MoveSpd);
    }
    public void ResetStat()
    {
    }

    public void ApplyBuff(StatType _type, float _flat, float _percent)
    {
        Stat targetStat = GetStat(_type);
        targetStat.ModifyBuffValue(_flat, _percent);

    }
    public void RemoveBuff(StatType _type, float _flat, float _percent)
    {
        Stat targetStat = GetStat(_type);
        targetStat.ModifyBuffValue(-_flat, -_percent);
    }
    public void RecoverHP(int _value)
    {
        CurrentHP.ModifyBaseValue(_value, 0, MaxHP.FinalValue);
    }
    public virtual Stat GetStat(StatType _type)
    {
        return _type switch
        {
            StatType.AttackPow => Attack,
            StatType.Defense => Defense,
            StatType.MaxHP => MaxHP,
            StatType.AttackSpd => AttackSpd,
            StatType.MoveSpd => MoveSpd,
            StatType.AttackRange => AttackRange,
            _ => null
        };
    }
}
