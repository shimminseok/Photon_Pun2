using System;
using _2.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterTable", menuName = "Scriptable Objects/MonsterTable")]
public class MonsterTable : BaseTable<SummonObjectData>
{
    public override void CreateTable()
    {
        base.CreateTable();
        foreach (var data in dataList)
        {
            dataDic[data.ID] = data;
        }
    }
}