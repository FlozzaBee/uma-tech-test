using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError($"Service is null {typeof(T)}");
            }
            return _instance;
        }
    }

    public static bool IsValid()
    {
        return _instance != null;
    }
    protected virtual void Awake()
    {
        _instance = this as T;
    }

    protected void OnDestroy()
    {
        _instance = null;
    }
}