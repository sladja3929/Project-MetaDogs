using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static readonly object lockObject = new();
    private static T instance;

    public static T Instance
    {
        get
        {
            lock (lockObject)
            {
                if (instance is null)
                {
                    instance = (T)FindObjectOfType(typeof(T));
                    if (instance is null)
                    {
                        instance = Instantiate(Resources.Load<T>("Singleton/" + typeof(T)));
                        DontDestroyOnLoad(instance.gameObject);
                    }
                }
                return instance;
            }
        }
    }
}
