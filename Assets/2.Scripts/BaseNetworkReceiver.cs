using System;
using Photon.Pun;
using UnityEngine;

public abstract class BaseNetworkReceiver<TController> : MonoBehaviourPun, IPunObservable
    where TController : BaseController<TController>
{
    protected TController Controller { get; private set; }
    private ObjectState receivedState;
    private Quaternion receivedRot;

    protected virtual void Awake()
    {
        Controller = Helper.GetComponetHelpper<TController>(gameObject);
    }

    protected void Update()
    {
        if (Controller.ActorNum == PhotonNetwork.LocalPlayer.ActorNumber)
            return;
        Controller.StateMachine.Excute();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.rotation);
            stream.SendNext((int)Controller.CurrentState);
        }
        else
        {
            receivedRot = (Quaternion)stream.ReceiveNext();
            receivedState = (ObjectState)(int)stream.ReceiveNext();
            if (Controller.CurrentState == ObjectState.Dead)
                return;
            if (receivedState != Controller.CurrentState)
                Controller.ChangeState(receivedState);
        }
    }

    public Quaternion MirrorRotation(Quaternion rot)
    {
        Vector3 euler = rot.eulerAngles;
        euler.y = 180f - euler.y;
        return Quaternion.Euler(euler);
    }
}