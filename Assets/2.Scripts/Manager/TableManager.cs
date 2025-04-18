using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TableManager : Singleton<TableManager>
{
    [SerializeField] List<ScriptableObject> tableList = new List<ScriptableObject>();


    Dictionary<Type, ITable> tableDic = new Dictionary<Type, ITable>();

    void Awake()
    {
        foreach (var tableObj in tableList)
        {
            if (tableObj is ITable table)
            {
                table.CreateTable();
                tableDic[table.Type] = table;
            }
        }
    }

    public T GetTable<T>() where T : class
    {
        return tableDic[typeof(T)] as T;
    }

#if UNITY_EDITOR
    public void AutoAssignTables()
    {
        tableList.Clear();

        string[] guids =
            UnityEditor.AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets/5.Tables/ScriptableObj" });

        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

            if (asset is ITable)
            {
                if (!tableList.Contains(asset))
                {
                    tableList.Add(asset);
                }
            }
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}