using System;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class SummonManager : MonoBehaviour
{
    public static SummonManager Instance { get; private set; }
    [SerializeField] float lifeDuration;
    [SerializeField] int maxLife;


    string monsterPath = string.Empty;

    public List<ITargetable> EnemyList = new List<ITargetable>();
    public SummonLifeSystem SummonLifeSystem { get; private set; }
    public ITargetable      EnemyCanon       { get; set; }

    bool isSummonable;

    int summonedMonsterID;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SummonLifeSystem = new SummonLifeSystem(lifeDuration, maxLife);
    }

    void Start()
    {
        GameObject      go        = PhotonNetwork.Instantiate("Canon", Vector3.zero, Quaternion.identity, 0);
        CanonController canonCtrl = Helper.GetComponetHelpper<CanonController>(go);
        canonCtrl.NetworkReceiver.photonView.RPC(nameof(canonCtrl.RPC_SpawnSync), RpcTarget.All, new Vector3(0, 0.5f, -7), PhotonNetwork.LocalPlayer.ActorNumber);
    }

    void Update()
    {
        SummonLifeSystem.Update(Time.deltaTime);

        if (Input.GetMouseButtonDown(0) && isSummonable && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                GameObject go = ObjectPoolManager.Instance.GetObject(monsterPath);

                SummonedMonsterController ctrl = Helper.GetComponetHelpper<SummonedMonsterController>(go);
                ctrl.NetworkReceiver.photonView.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
                ctrl.NetworkReceiver.photonView.RPC(nameof(ctrl.RPC_SpawnSync), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, hit.point);

                SummonLifeSystem.ConsumeLife(ctrl.MonsterData.SummonCost);
                isSummonable = false;
            }
        }
        else if (Input.GetMouseButtonDown(1) && isSummonable && !EventSystem.current.IsPointerOverGameObject())
        {
            isSummonable = false;
            monsterPath = string.Empty;
        }
    }

    public void OnClickSummonBtn(int _id)
    {
        var data = TableManager.Instance.GetTable<MonsterTable>().GetDataByID(_id);
        if (data == null || !SummonLifeSystem.IsSummonable(data.SummonCost))
            return;

        isSummonable = true;
        monsterPath = data.Prefabs.name;
        summonedMonsterID = _id;
    }

    public ITargetable FindEnemy()
    {
        if (EnemyList.Count > 0)
        {
            return EnemyList.Find(x => !x.IsDead);
        }

        return EnemyCanon;
    }


    public void RemoveEnemy(SummonedMonsterController _enemy)
    {
        EnemyList.Remove(_enemy);
    }

    private void OnGUI()
    {
        GUI.TextArea(new Rect(30, 0, 200, 50), PhotonNetwork.GetPing().ToString());

        GUI.TextArea(new Rect(30, 60, 200, 50), PhotonNetwork.NickName);
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}