using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType(typeof(T)) as T;
                if (instance == null)
                {
                    SteupInstance();
                }
                DontDestroyOnLoad(instance.gameObject);

            }
            return instance;
        }
    }

    private void Awake()
    {
        RemoveDuplicates();
    }
    void RemoveDuplicates()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    static void SteupInstance()
    {
        GameObject gameObj = new GameObject();
        gameObj.name = typeof(T).Name;
        instance = gameObj.AddComponent<T>();
    }

}

