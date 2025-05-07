using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

#region[Enum]

public enum StatType
{
    None,
    AttackPow,
    Defense,
    MaxHP,
    AttackSpd,
    AttackRange,
    MoveSpd,
    CriticalChance,
    CurrentHP,
    CurrentMP
}

public enum ObjectState
{
    Idle,
    Move,
    Attack,
    Dead
}

public enum ChannelType
{
    Lobby,
    Room
}

#endregion


#region[Interface]

public interface IMovementHandler
{
    void Move(Vector3 _dir);
    void Stop(bool _isStop);
    void LookAtMovementDir();
    void LookAtTarget(Vector3 _target);
    void MirrorPosition();
    void ApplyStats();
}

public interface ICombatHandler
{
    void Attack(ITargetable target);
}

public interface IAnimationHandler
{
    void ApplyStatToAnimator();
    void SetMove(bool _isMove);
    void TriggerAttack();
}

public interface INetworkHandler
{
    bool IsMine { get; }
}

public interface ITargetingHandler
{
    ITargetable FindEnemy();
}

public interface ITable
{
    public          Type Type { get; }
    public abstract void CreateTable();
}

public interface ITargetable
{
    ITargetable Target       { get; }
    Transform   Transform    { get; }
    int         PhotonViewID { get; }
    bool        IsDead       { get; }
    void        TakeDamage(int _damage);
}

public interface INetworkPoolable
{
    PhotonView PhotonView { get; }

    [PunRPC]
    void RegisterToPool_RPC(int _viewID, string _name);

    [PunRPC]
    void RPC_SpawnSync(int _actorNum, Vector3 _spawnPos);
}

#endregion[Interface]

[Serializable]
public class SummonObjectData
{
    public int ID;
    public string Name;
    public int MaxHP;
    public int AttackPow;
    public float AttackRange;
    public float AttackSpd;
    public int Defense;
    public float MoveSpd;
    public GameObject Prefabs;
    public Sprite MonsterIcon;

    public float SummonCost;
}

public interface IState<T> where T : class
{
    void Enter(T _owenr);
    void Execute(T _owenr);
    void Exit(T _owenr);
}


public class Stat
{
    public StatType Type;
    public float BaseValue    { get; private set; }
    public float BuffValue    { get; private set; }
    public float PercentValue { get; private set; }
    public float FinalValue   => (BaseValue + BuffValue /*+ EquipmentValue*/) * (1 + PercentValue);


    public event Action<float> OnStatChanged;


    public Stat(StatType _type)
    {
        Type = _type;
    }

    public void ResetModifiers()
    {
        BaseValue = 0;
        BuffValue = 0;
    }

    public void ModifyBaseValue(float _value, float _min = 0, float _max = float.MaxValue)
    {
        BaseValue = Mathf.Clamp(BaseValue + _value, _min, _max);
        OnStatChanged?.Invoke(FinalValue);
    }

    public void ModifyBuffValue(float _flat, float _percent)
    {
        BuffValue += _flat;
        PercentValue += _percent;
        OnStatChanged?.Invoke(FinalValue);
    }

    public void ModifyAllValue(float _value, float _percent = 0)
    {
        float remainingDam = _value;
        if (BuffValue > 0)
        {
            float damToBuff = MathF.Min(remainingDam, BuffValue);
            ModifyBuffValue(damToBuff, _percent);
            remainingDam -= damToBuff;
        }

        if (remainingDam > 0)
        {
            ModifyBaseValue(-remainingDam, 0, FinalValue);
        }
    }
}