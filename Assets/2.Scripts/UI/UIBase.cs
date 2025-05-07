using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UIBase : MonoBehaviour
{
    [Header("Base")] [SerializeField] private GameObject content;

    protected virtual void Awake()
    {
        content.SetActive(false);
    }

    public virtual void Open()
    {
        content.SetActive(true);
        UIManager.Instance.OpenPanel(this);
    }

    public virtual void Close()
    {
        content.SetActive(false);
        UIManager.Instance.ClosePanel(this);
    }
    
    
}