using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelSingleton<T> : MonoBehaviour
where T : LevelSingleton<T>
{
    protected static T instance { get; set; }

    protected virtual void Awake()
    {
#if UNITY_EDITOR
        if (instance != null)
        {
            if (instance == this)
                Debug.LogError("Instance for " + typeof(T) + " has already been established. Leak found.");
            else
                Debug.LogError("Instance for " + typeof(T) + " has already been established. Duplicate found.");
        }
#endif

        instance = (T)this;
        GameAwake();
    }

    protected abstract void GameAwake();

}
