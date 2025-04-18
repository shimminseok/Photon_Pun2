using Photon.Pun;
using System;
using UnityEngine;
public class AniEventListener : MonoBehaviour
{
    SummonedMonsterController m_MonsterController;
    void Awake()
    {
        m_MonsterController = Helper.GetComponetHelpper<SummonedMonsterController>(gameObject);
    }
    public void AttackEvent()
    {
        m_MonsterController.Attack();
    }

    public void PlayEffent()
    {

    }
}
