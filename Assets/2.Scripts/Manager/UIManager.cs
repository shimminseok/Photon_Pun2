using System.Collections.Generic;
using UnityEngine;
public class UIManager : Singleton<UIManager>
{
    public List<UIBase> OpenedPopup { get; private set; } = new();

    public void CheckOpenPopup(UIBase _panel)
    {
        if (OpenedPopup.Contains(_panel))
        {
            _panel.Close();
        }
        else
        {
            _panel.Open();
        }
    }

    public void OpenPanel(UIBase _panel)
    {
        OpenedPopup.Add(_panel);
        Debug.Log($"{_panel.name} Open");
    }

    public void ClosePanel(UIBase _panel)
    {
        OpenedPopup.Remove(_panel);
        Debug.Log($"{_panel.name} Close");
    }

    public void AllClosePanel()
    {
        for (int i = OpenedPopup.Count - 1; i >= 0; i--)
        {
            OpenedPopup[i].Close();
        }
    }
}