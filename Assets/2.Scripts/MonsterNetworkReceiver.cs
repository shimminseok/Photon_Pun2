using Photon.Pun;
using UnityEngine;

public class MonsterNetworkReceiver : BaseNetworkReceiver<SummonedMonsterController>
{
    public bool IsMine => photonView.IsMine && photonView.Owner == PhotonNetwork.LocalPlayer;


    protected virtual void Awake()
    {
        base.Awake();
    }

    protected virtual void Update()
    {
        base.Update();
    }
}