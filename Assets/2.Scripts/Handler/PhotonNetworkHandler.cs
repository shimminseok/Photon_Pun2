using Photon.Pun;
using UnityEngine;

public class PhotonNetworkHandler : INetworkHandler
{
    private readonly PhotonView view;
    private readonly SummonedMonsterController owner;
    public PhotonNetworkHandler(PhotonView _view, SummonedMonsterController _owner)
    {
        view = _view;
        owner = _owner;
    }
    public bool IsMine => view.IsMine && view.Owner == PhotonNetwork.LocalPlayer;
}
