using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2.Scripts.UI
{
    public class InGameSummonBtn : MonoBehaviour
    {
        [SerializeField] Image monsterIcon;
        [SerializeField] int index;
        [SerializeField] Image summonCostImgFill;
        [SerializeField] TextMeshProUGUI costTxt;
        public SummonObjectData MonsterData { get; private set; }

        void Start()
        {
            SetMonsterData();
        }


        public void SetMonsterData()
        {
            MonsterData = SelectedMonsterHolder.Instance.GetMonsterBySlotIndex(index);
            if (MonsterData == null)
                gameObject.SetActive(false);
            else
            {
                gameObject.SetActive(true);
                monsterIcon.sprite = MonsterData.MonsterIcon;
                costTxt.text = MonsterData.SummonCost.ToString();
                summonCostImgFill.fillAmount = MonsterData.SummonCost;
            }
        }

        public void OnClickBtn()
        {
            SummonManager.Instance.OnClickSummonBtn(MonsterData.ID);
        }
    }
}