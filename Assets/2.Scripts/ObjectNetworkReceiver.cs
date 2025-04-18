using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectNetworkReceiver : MonoBehaviourPun, IPunObservable
{
    public bool IsMine => photonView.IsMine && photonView.Owner == PhotonNetwork.LocalPlayer;

    SummonedMonsterController m_Controller;

    ObjectState receivedState;
    Quaternion receivedRot;

    private void Awake()
    {
        m_Controller = Helper.GetComponetHelpper<SummonedMonsterController>(gameObject);

    }
    void Update()
    {
        if (m_Controller.ActorNum == PhotonNetwork.LocalPlayer.ActorNumber)
            return;

        
        m_Controller.Updated();
        //transform.rotation = MirrorRotation(receivedRot);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(receivedRot);
            stream.SendNext((int)m_Controller.CurrentState);
        }
        else
        {
            receivedRot = (Quaternion)stream.ReceiveNext();
            receivedState = (ObjectState)(int)stream.ReceiveNext();
            if (m_Controller.CurrentState == ObjectState.Dead)
                return;

            if (receivedState != m_Controller.CurrentState)
            {
                m_Controller.ChangeState(receivedState);
            }
        }
    }

    public Quaternion MirrorRotation(Quaternion _rot)
    {
        Vector3 euler = _rot.eulerAngles;
        euler.y = 180f - euler.y;

        return Quaternion.Euler(euler);
    }



}
