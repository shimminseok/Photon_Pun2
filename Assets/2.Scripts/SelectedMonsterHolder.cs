using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectedMonsterHolder : Singleton<SelectedMonsterHolder>
{
    [SerializeField] List<SummonObjectData> SelectedMonsters = new();


    public void AddMonster(SummonObjectData _data)
    {
        if (!SelectedMonsters.Exists(x => x.ID == _data.ID))
            SelectedMonsters.Add(_data);
    }
    public void RemoveMonster(SummonObjectData _data)
    {
        SelectedMonsters.Remove(_data);
    }

    public SummonObjectData GetMonsterByMonsterID(int _id)
    {
        return SelectedMonsters.Find(x => x.ID == _id);
    }
    public SummonObjectData GetMonsterBySlotIndex(int _index)
    {
        return SelectedMonsters[_index];
    }

    public List<SummonObjectData> GetMonsterList()
    {
        return SelectedMonsters;
    }

    public int[] GetMonsterIds()
    {
        return SelectedMonsters.Select(mon => mon.ID).ToArray();
    }

    public bool HasDuplicateId(int _id)
    {
        return SelectedMonsters.Exists(x => x.ID == _id);
    }
}
