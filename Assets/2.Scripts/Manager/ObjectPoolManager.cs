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
        CreateMonsterPool();
        CreateNetworkObject("Muzzle");
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
            Debug.LogWarning($"등록된 풀이 없습니다. : {_name}");
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

    IEnumerator DelayedReturnObject(GameObject _obj, UnityAction _action, float _returnTime)
    {
        if (!poolObjects.ContainsKey(_obj.name))
        {
            Debug.LogWarning($"생성된 풀이 존재하지 않습니다. : {_obj.name}");
            yield return null;
        }

        yield return new WaitForSeconds(_returnTime);
        _obj.transform.localPosition = Vector3.zero;
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

    private void CreateMonsterPool()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

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
            for (int i = 0; i < 10; i++)
            {
                // GameObject go = PhotonNetwork.Instantiate(monsterName, Vector3.zero, Quaternion.identity, 0,
                //     new object[] { id });
                // if (!go.TryGetComponent<INetworkPoolable>(out var poolable))
                // {
                //     Debug.LogError($"INetworkPoolable is Null {go.name}");
                //     yield break;
                // }

                CreateNetworkObject(monsterName, new object[] { id });
                // SummonedMonsterController ctrl = Helper.GetComponetHelpper<SummonedMonsterController>(go);
                // ctrl.NetworkReceiver.photonView.RPC(nameof(ctrl.RegisterToPool_RPC), RpcTarget.All, ctrl.NetworkReceiver.photonView.ViewID);
            }
        }
    }

    private void CreateNetworkObject(string _name, object[] _initData = null)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        for (int i = 0; i < 10; i++)
        {
            GameObject go = PhotonNetwork.InstantiateRoomObject(_name, Vector3.zero, Quaternion.identity, 0, _initData);
            if (!go.TryGetComponent<INetworkPoolable>(out var poolable))
            {
                Debug.LogError($"INetworkPoolable is Null {go.name}");
                return;
            }

            poolable.PhotonView.RPC(nameof(poolable.RegisterToPool_RPC), RpcTarget.All, poolable.PhotonView.ViewID, _name);
        }
    }

    public void GetObjectSync(int _viewID)
    {
        GameObject        go   = PhotonView.Find(_viewID).gameObject;
        Queue<GameObject> pool = poolObjects[go.name];
        if (pool.Count > 0)
        {
            pool.Dequeue();
        }
    }

    public void RegisterRuntimeObject(string name, GameObject obj)
    {
        if (!parentCache.ContainsKey(name))
            CreatePoolRoot(name);

        obj.transform.SetParent(parentCache[name]);
        obj.SetActive(false);

        if (!poolObjects.ContainsKey(name))
            poolObjects[name] = new Queue<GameObject>();

        poolObjects[name].Enqueue(obj);
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}