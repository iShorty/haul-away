using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ///<Summary>A manager class which will have its own static instance and will be called during Awake by the MasterGameManager in specific order determined by the basemanager's execution order index. Add your global event subscriptions here. If you inherit from this class, you accept the fact that the manager class you are going to write (especially if you are using update loops) will not be call OnEnable or OnDisable more than once throughout its entire lifecycle. To ensure that the manager classes are called in perfect execution order (that means that the execution order index displays the true order of execution relative to ALL scripts in Unity) make MasterGameManager the first script to be executed in ProjectSettings > Script Execution Order</Summary>
public abstract class BaseManager : MonoBehaviour,IGlobalEventManager
{
    [SerializeField]
    protected int _executionOrder;
    // protected T instance { get; set; }

    public int ExecutionOrder => _executionOrder;

    ///<Summary>The proxy method of OnGameAwake which will be called by the MasterGameManager on awake</Summary>
    public virtual void GameAwake()
    {
        OnGameAwake();
    }

    ///<Summary>Gets called in a specific order determined by the basemanager's execution order index. Subscribe to Global events here. Update loops subscribed here will be executed in that order as well. Set MasterGameManager's execution order in Unity's ProjectSettings > Script Execution Order to before default time for best effect</Summary>
    protected abstract void OnGameAwake();

    ///<Summary>Since we cannot control the order in which our managers gets destroyed, all manager classes must unsubscribe from Global events here</Summary>
    public abstract void OnDestroy();

}
