using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _2.Scripts.UI
{
    public class SummonUI : MonoBehaviour
    {
        [SerializeField] List<Image> lifeFills = new List<Image>();

        private void OnEnable()
        {
            SummonManager.Instance.SummonLifeSystem.OnLifeChanged += UpdateLifeUI;
        }

        private void OnDisable()
        {
            SummonManager.Instance.SummonLifeSystem.OnLifeChanged -= UpdateLifeUI;
        }

        void UpdateLifeUI(float _life)
        {
            int fullIndex = (int)_life;
            float partial = _life - fullIndex;

            for (int i = 0; i < lifeFills.Count; i++)
            {
                if (i < fullIndex)
                    lifeFills[i].fillAmount = 1f;
                else if (i == fullIndex)
                    lifeFills[i].fillAmount = partial;
                else
                    lifeFills[i].fillAmount = 0f;
            }
        }
    }
}