using System;
using UnityEngine;
using UnityEngine.U2D;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] Transform monsterSlotRoot;
    [SerializeField] GameObject monsterSlotPrefabs;


    public SummonSlotUI[] selectedMonsterSlots = new SummonSlotUI[4];

    void Start()
    {
        MonsterTable monsterTb = TableManager.Instance.GetTable<MonsterTable>();
        foreach (var data in monsterTb.dataDic.Values)
        {
            GameObject go = Instantiate(monsterSlotPrefabs, monsterSlotRoot);
            MonsterListSlotUI slot = Helper.GetComponetHelpper<MonsterListSlotUI>(go);
            slot.SetMonsterSlot(data);
        }

        for (int i = 0; i < SelectedMonsterHolder.Instance.GetMonsterList().Count; i++)
        {
            var data = SelectedMonsterHolder.Instance.GetMonsterBySlotIndex(i);
            SelectedMonster(data);
        }
    }


    public void OnClickMatchBtn()
    {
        NetworkManager.Instance.JoinRandomRoomOrCreate();
    }


    public void SelectedMonster(SummonObjectData _monster)
    {
        var emptySlotIndex = Array.FindIndex(selectedMonsterSlots, x => x.MonsterData == null); //∫Û ΩΩ∑‘ √£±‚

        if (emptySlotIndex > -1)
        {
            if (!SelectedMonsterHolder.Instance.HasDuplicateId(_monster.ID))
            {
                selectedMonsterSlots[emptySlotIndex].SetSummonMonster(_monster);
                SelectedMonsterHolder.Instance.AddMonster(_monster);
            }
        }
        else
        {
            Debug.Log("∫Û ΩΩ∑‘¿Ã æ¯¿Ω");
        }
    }
}