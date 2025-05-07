using System;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class Muzzle : MonoBehaviour
{
    private System.Action attackAction;
    private GameObject target;
    private float moveSpeed;

    void Update()
    {
        MoveToTarget();
    }

    public void SetTarget(GameObject target, Action<int> _action, int _damage)
    {
        this.target = target;
        this.attackAction = () => _action(_damage);
    }

    void MoveToTarget()
    {
        if (target == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.transform.position,
            moveSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ITargetable>(out var iTarget) && iTarget.Transform.gameObject == target)
        {
            attackAction?.Invoke();
            ObjectPoolManager.Instance.ReturnObject(gameObject);
            print($"포탑이 공격했음!");
        }
    }
}