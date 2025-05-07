using Photon.Pun;
using UnityEngine;
public class CanonNetworkReceiver : BaseNetworkReceiver<CanonController>
{
    public bool IsMine => photonView.IsMine && photonView.Owner == PhotonNetwork.LocalPlayer;

    CanonController m_Controller;

    ObjectState receivedState;
    Quaternion receivedRot;

    protected virtual void Awake()
    {
        base.Awake();
    }

    protected virtual void Update()
    {
        base.Update();
    }
}
