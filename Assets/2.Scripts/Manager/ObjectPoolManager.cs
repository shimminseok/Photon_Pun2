using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;


    public Dictionary<string, Queue<GameObject>> poolObjects = new Dictionary<string, Queue<GameObject>>();


    Dictionary<string, GameObject> registeredObj = new Dictionary<string, GameObject>();

    public Dictionary<string, Transform> parentCache = new Dictionary<string, Transform>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(TryCreatePool());
    }

    /// <summary>
    /// ??????? ? ????
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_prefab"></param>
    /// <param name="_poolSize"></param>
    public void CreatePool(GameObject _prefab, int _poolSize)
    {
        GameObject parentObj = CreatePoolRoot(_prefab.name);
        if (parentObj == null)
            return;

        Queue<GameObject> newPool = new Queue<GameObject>();
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject obj = Instantiate(_prefab, parentObj.transform);
            obj.name = _prefab.name;
            obj.SetActive(false);
            newPool.Enqueue(obj);
        }

        poolObjects[_prefab.name] = newPool;
        registeredObj[_prefab.name] = _prefab;
    }

    public GameObject CreatePoolRoot(string _name)
    {
        if (poolObjects.ContainsKey(_name))
        {
            Debug.LogWarning($"??? ??????? ? : {_name}");
            return null;
        }

        GameObject parentObj = new GameObject(_name) { transform = { parent = transform } };
        parentCache[_name] = parentObj.transform;
        poolObjects[_name] = new Queue<GameObject>();

        return parentObj;
    }

    /// <summary>
    /// ????? ????????? ???????? ???
    /// </summary>
    /// <param name="_name"></param>
    /// <returns></returns>
    public GameObject GetObject(string _name)
    {
        if (!poolObjects.ContainsKey(_name))
        {
            Debug.LogWarning($"???? ??? ??????? : {name}");
            return null;
        }

        Queue<GameObject> pool = poolObjects[_name];
        if (pool.Count > 0)
        {
            GameObject go = pool.Dequeue();
            go.SetActive(true);
            return go;
        }
        else
        {
            GameObject prefab = registeredObj[_name];
            GameObject newObj = Instantiate(prefab);
            newObj.name = _name;
            newObj.transform.SetParent(parentCache[_name]);
            newObj.SetActive(true);
            return newObj;
        }
    }

    /// <summary>
    /// ????? ????????? ??? ?????? ???
    /// </summary>
    /// <param name="_obj"></param>
    IEnumerator DelayedReturnObject(GameObject _obj, UnityAction _action, float _returnTime)
    {
        if (!poolObjects.ContainsKey(_obj.name))
        {
            Debug.LogWarning($"???? ??? ??????? : {_obj.name}");
            yield return null;
        }

        yield return new WaitForSeconds(_returnTime);
        _obj.SetActive(false);
        _action?.Invoke();
        poolObjects[_obj.name].Enqueue(_obj);
    }

    public void ReturnObject(GameObject _obj, float _returnTime = 0, UnityAction _action = null)
    {
        StartCoroutine(DelayedReturnObject(_obj, _action, _returnTime));
    }

    public void RemovePool(string _name)
    {
        Destroy(parentCache[_name].gameObject);
        parentCache.Remove(_name);
        poolObjects.Remove(_name);
        registeredObj.Remove(_name);
    }

    public IEnumerator TryCreatePool()
    {
        yield return new WaitForSeconds(0.5f); //??? ????..? 0.5?????? ??????

        if (!PhotonNetwork.IsMasterClient)
            yield break;

        List<int> allMonsterIds = new List<int>();
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (player.CustomProperties.TryGetValue("MONSTER_IDs", out object raw))
            {
                int[] ids = (int[])raw;
                foreach (int id in ids)
                {
                    allMonsterIds.Add(id);
                }
            }
        }

        MonsterTable monsterTb = TableManager.Instance.GetTable<MonsterTable>();
        foreach (int id in allMonsterIds)
        {
            string monsterName = monsterTb.GetDataByID(id).Prefabs.name;
            CreatePoolRoot(monsterName); // ??? ???? Hiearchy???? ???? ?????? ????? ?¬Ñ?...
            for (int i = 0; i < 10; i++)
            {
                GameObject go = PhotonNetwork.Instantiate(monsterName, Vector3.zero, Quaternion.identity, 0,
                    new object[] { id });
                //GameObject go = PhotonNetwork.InstantiateRoomObject(monsterName,Vector3.zero,Quaternion.identity,0,new object[] {id });
                SummonedMonsterController ctrl = Helper.GetComponetHelpper<SummonedMonsterController>(go);
                ctrl.NetworkReceiver.photonView.RPC(nameof(ctrl.RegisterToPool_RPC), RpcTarget.All,
                    ctrl.NetworkReceiver.photonView.ViewID);
                go.transform.SetParent(parentCache[go.name]); //????...
            }
        }
    }

    public void GetObjectSync(GameObject _go)
    {
        Queue<GameObject> pool = poolObjects[_go.name];
        if (pool.Count > 0)
        {
            GameObject go = pool.Dequeue();
        }
    }

    public void ReturnObjectSync(GameObject _go)
    {
    }
}