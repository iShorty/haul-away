using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>A manager class which will have its own static instance and will be called during Awake by the MasterGameManager in specific order determined by the basemanager's execution order index. Add your global event subscriptions here. If you inherit from this class, you accept the fact that the manager class you are going to write (especially if you are using update loops) will not be call OnEnable or OnDisable more than once throughout its entire lifecycle. To ensure that the manager classes are called in perfect execution order (that means that the execution order index displays the true order of execution relative to ALL scripts in Unity) make MasterGameManager the first script to be executed in ProjectSettings > Script Execution Order</Summary>
public abstract class GenericManager<T> : BaseManager
where T : GenericManager<T>
{
    protected static T instance { get; set; }

    public override void GameAwake()
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
        base.GameAwake();
    }


}
