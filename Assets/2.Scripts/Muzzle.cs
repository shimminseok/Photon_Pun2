using System;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class Muzzle : MonoBehaviour, INetworkPoolable
{
    public PhotonView PhotonView { get; private set; }
    private CanonController m_Canon;
    private System.Action attackAction;
    private GameObject target;
    private float moveSpeed = 5f;
    private int actorNum;

    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        MoveToTarget();
    }

    public void SetTarget(CanonController _owner, GameObject _target, Action<int> _action, int _damage)
    {
        m_Canon = _owner;
        this.target = _target;
        this.attackAction = () => _action(_damage);
        transform.localPosition = _owner.transform.localPosition;
        PhotonView.RPC(nameof(RPC_SpawnSync), RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber, _owner.transform.localPosition);
    }

    void MoveToTarget()
    {
        if (target == null) return;

        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            target.transform.localPosition,
            moveSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ITargetable>(out var iTarget) && iTarget.Transform.gameObject == target)
        {
            attackAction?.Invoke();
            ObjectPoolManager.Instance.ReturnObject(gameObject, 0.1f);
        }
    }


    [PunRPC]
    public void RegisterToPool_RPC(int _viewID, string _name)
    {
        PhotonView view = PhotonView.Find(_viewID);
        if (view != null)
        {
            GameObject go = view.gameObject;
            go.name = _name;
            ObjectPoolManager.Instance.RegisterRuntimeObject(_name, go);
        }
    }

    [PunRPC]
    public void RPC_SpawnSync(int _actorNum, Vector3 _spawnPos)
    {
        ObjectPoolManager.Instance.GetObjectSync(PhotonView.ViewID);
        gameObject.SetActive(true);
    }
}