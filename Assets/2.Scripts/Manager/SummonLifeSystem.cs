using NUnit.Framework;
using System;
using UnityEngine;

public class SummonLifeSystem
{
    public event Action<float> OnLifeChanged;

    float fillDuration = 2f;
    int maxLife = 6;

    public float CurrentLife { get; private set; }

    public SummonLifeSystem(float _duration, int _maxLife)
    {
        fillDuration = _duration;
        maxLife = _maxLife;
    }

    public void Update(float delta)
    {
        CurrentLife += delta / fillDuration;
        CurrentLife = Mathf.Clamp(CurrentLife, 0f, maxLife);

        OnLifeChanged?.Invoke(CurrentLife);
    }

    public void ConsumeLife(float _cost)
    {
        CurrentLife -= _cost;
        CurrentLife = Mathf.Clamp(CurrentLife, 0f, maxLife);
        OnLifeChanged?.Invoke(CurrentLife);
    }

    public bool IsSummonable(float _cost) => CurrentLife >= _cost;
}