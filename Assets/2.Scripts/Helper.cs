using UnityEngine;

public class Helper : MonoBehaviour
{
    public static T GetComponetHelpper<T>(GameObject _obj)
    {
        T _component = _obj.GetComponent<T>();
        if(_component == null)
        {
            Debug.LogError($"component Is null {typeof(T)}");
        }
        return _component;
    }
}


