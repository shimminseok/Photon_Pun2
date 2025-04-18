using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    public static HealthBarManager Instance;

    public Canvas hpBarCanvas;
    [SerializeField] GameObject healthBarPrefab;
    [SerializeField] int _initialPoolSize = 20;

    List<HPBarUI> activeBars = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        ObjectPoolManager.Instance.CreatePool(healthBarPrefab, _initialPoolSize);
    }

    private void LateUpdate()
    {
        foreach (var bar in activeBars)
        {
            bar.UpdatePosion();
        }
    }

    public HPBarUI SpawnHealthBar(Transform _targetTransform)
    {
        HPBarUI bar = ObjectPoolManager.Instance.GetObject(healthBarPrefab.name).GetComponent<HPBarUI>();
        bar.Initialize(_targetTransform);
        bar.gameObject.SetActive(true);
        activeBars.Add(bar);
        return bar;
    }

    public void DespawnHealthBar(HPBarUI _bar)
    {
        _bar.gameObject.SetActive(false);
        ObjectPoolManager.Instance.ReturnObject(_bar.gameObject);
        activeBars.Remove(_bar);
    }
}