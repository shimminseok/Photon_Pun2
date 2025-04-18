using UnityEngine;
using UnityEngine.UI;


public class SummonSlotUI : MonoBehaviour
{
    [SerializeField] Image monsterIcon;
    public SummonObjectData MonsterData { get; private set; }
    public int Index { get; private set; }

    private void Start()
    {
        //ResetSlot();
        Index = transform.GetSiblingIndex();
    }

    private void ResetSlot()
    {
        SelectedMonsterHolder.Instance.RemoveMonster(MonsterData);
        monsterIcon.sprite = null;
        MonsterData = null;
        monsterIcon.enabled = false;
    }

    public void SetSummonMonster(SummonObjectData _data)
    {
        if (_data == null)
            ResetSlot();

        monsterIcon.enabled = true;
        MonsterData = _data;
        monsterIcon.sprite = _data.MonsterIcon;
    }

    public void OnClickSlot()
    {
        ResetSlot();
    }
}