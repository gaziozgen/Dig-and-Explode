using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames.Core;
public class Singleton<T> : FateMonoBehaviour where T : Component
{
    [SerializeField] bool dontDestroyOnLoad = true;
    private static T instance;
    public bool duplicated { get; private set; }
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));
                if (instance == null)
                {
                    SetupInstance();
                }
            }
            return instance;
        }
    }
    protected virtual void Awake()
    {
        transform.SetParent(null);
        RemoveDuplicates();
    }
    private static void SetupInstance()
    {
        instance = (T)FindObjectOfType(typeof(T));
        if (instance == null)
        {
            GameObject gameObj = new GameObject();
            gameObj.name = typeof(T).Name;
            instance = gameObj.AddComponent<T>();
            DontDestroyOnLoad(gameObj);
        }
    }
    private void RemoveDuplicates()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            duplicated = true;
            Destroy(gameObject);
        }
    }
}