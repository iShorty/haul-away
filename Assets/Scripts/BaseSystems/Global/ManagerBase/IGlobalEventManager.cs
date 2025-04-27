using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>Used to allow managers which do not inherit from GenericManagers<T> the ability to be updated by the MasterGameManager</Summary>
public interface IGlobalEventManager
{
    void GameAwake();
    void OnDestroy();

    int ExecutionOrder {get;}
}
