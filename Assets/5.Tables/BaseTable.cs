using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using _2.Scripts;

public class BaseTable<T> : ScriptableObject, ITable where T : class
{
    [SerializeField] protected List<T> dataList = new List<T>();

    public Dictionary<int, T> dataDic { get; private set; } = new Dictionary<int, T>();

    protected Type type;
    public Type Type => type;

    public virtual void CreateTable()
    {
        type = GetType();
    }

    public virtual T GetDataByID(int _id)
    {
        if (dataDic.TryGetValue(_id, out T value))
            return value;

        Debug.LogError($"ID {_id}�� �ش��ϴ� �����Ͱ� �������� �ʽ��ϴ�. (Type: {type})");
        return null;
    }
}