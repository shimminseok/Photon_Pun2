using Photon.Pun;
using UnityEngine;

public class CanonNetworkReceiver : BaseNetworkReceiver<CanonController>
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