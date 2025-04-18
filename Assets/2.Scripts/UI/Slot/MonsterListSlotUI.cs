using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterListSlotUI : MonoBehaviour
{
    [SerializeField] Image monsterIcon;
    [SerializeField] TextMeshProUGUI monsterNameTxt;

    SummonObjectData monsterData;

    LobbyManager lobbyManager;

    void Start()
    {
        lobbyManager = FindAnyObjectByType<LobbyManager>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetMonsterSlot(SummonObjectData _data)
    {
        monsterIcon.sprite = _data.MonsterIcon;
        monsterNameTxt.text = _data.Name;
        monsterData = _data;
    }

    public void OnClickSlot()
    {
        //���� ����ϱ�
        lobbyManager.SelectedMonster(monsterData);
    }
}