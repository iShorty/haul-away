using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ManagerUtility
{
    #region Find Manager Method

    // #region Non-Updateable Manager
    // public static IGlobalEventManager[] FindAllManagers(this IGlobalEventManager[] managers, GameObject managerContainer)
    // {
    //     List<IGlobalEventManager> tempList = new List<IGlobalEventManager>();
    //     tempList.AddRange(managerContainer.GetComponentsInChildren<IGlobalEventManager>());
    //     return tempList.ToArray();
    // }

    // public static List<IGlobalEventManager> FindAllManagers(this List<IGlobalEventManager> allManagers, GameObject managerContainer)
    // {
    //     allManagers.AddRange(managerContainer.GetComponentsInChildren<IGlobalEventManager>());
    //     return allManagers;
    // }

    // public static IGlobalEventManager[] FindAllManagers(this IGlobalEventManager[] allManagers, GameObject[] managersOutsideOfContainer)
    // {
    //     List<IGlobalEventManager> tempList = new List<IGlobalEventManager>();

    //     for (int i = 0; i < managersOutsideOfContainer.Length; i++)
    //     {
    //         //Add range + getcomponents is used here incase the manager transform holds more than 1 manager on the same gameobject
    //         tempList.AddRange(managersOutsideOfContainer[i].GetComponents<IGlobalEventManager>());
    //     }

    //     return tempList.ToArray();
    // }

    // public static List<IGlobalEventManager> FindAllManagers(this List<IGlobalEventManager> allManagers, GameObject[] managersOutsideOfContainer)
    // {
    //     for (int i = 0; i < managersOutsideOfContainer.Length; i++)
    //     {
    //         //Add range + getcomponents is used here incase the manager transform holds more than 1 manager on the same gameobject
    //         allManagers.AddRange(managersOutsideOfContainer[i].GetComponents<IGlobalEventManager>());
    //     }
    //     return allManagers;
    // }

    // public static IGlobalEventManager[] FindAllManagers(this IGlobalEventManager[] allManagers, GameObject managerContainer, GameObject[] managersOutsideOfContainer)
    // {
    //     List<IGlobalEventManager> tempList = new List<IGlobalEventManager>();

    //     tempList.AddRange(managerContainer.GetComponentsInChildren<IGlobalEventManager>());

    //     for (int i = 0; i < managersOutsideOfContainer.Length; i++)
    //     {
    //         //Add range + getcomponents is used here incase the manager transform holds more than 1 manager on the same gameobject
    //         tempList.AddRange(managersOutsideOfContainer[i].GetComponents<IGlobalEventManager>());
    //     }

    //     return tempList.ToArray();
    // }

    // public static List<IGlobalEventManager> FindAllManagers(this List<IGlobalEventManager> allManagers, GameObject managerContainer, GameObject[] managersOutsideOfContainer)
    // {
    //     allManagers.AddRange(managerContainer.GetComponentsInChildren<IGlobalEventManager>());

    //     for (int i = 0; i < managersOutsideOfContainer.Length; i++)
    //     {
    //         //Add range + getcomponents is used here incase the manager transform holds more than 1 manager on the same gameobject
    //         allManagers.AddRange(managersOutsideOfContainer[i].GetComponents<IGlobalEventManager>());
    //     }
    //     return allManagers;
    // }


    // #endregion

    // #region Find Updateable Managers

    // public static IUpdateGlobalEventManager[] FindAllUpdateableManagers(this IUpdateGlobalEventManager[] allUpdateableManagers, GameObject managerContainer)
    // {
    //     List<IUpdateGlobalEventManager> tempUpdateableList = new List<IUpdateGlobalEventManager>();

    //     tempUpdateableList.AddRange(managerContainer.GetComponentsInChildren<IUpdateGlobalEventManager>());

    //     return tempUpdateableList.ToArray();
    // }

    // public static List<IUpdateGlobalEventManager> FindAllUpdateableManagers(this List<IUpdateGlobalEventManager> allUpdateableManagers, GameObject managerContainer)
    // {
    //     allUpdateableManagers.AddRange(managerContainer.GetComponentsInChildren<IUpdateGlobalEventManager>());
    //     return allUpdateableManagers;
    // }


    // public static IUpdateGlobalEventManager[] FindAllUpdateableManagers(this IUpdateGlobalEventManager[] allUpdateableManagers, GameObject[] managersOutsideOfContainer)
    // {
    //     List<IUpdateGlobalEventManager> tempUpdateableList = new List<IUpdateGlobalEventManager>();

    //     for (int i = 0; i < managersOutsideOfContainer.Length; i++)
    //     {
    //         //Add range + getcomponents is used here incase the manager transform holds more than 1 manager on the same gameobject
    //         tempUpdateableList.AddRange(managersOutsideOfContainer[i].GetComponents<IUpdateGlobalEventManager>());
    //     }

    //     return tempUpdateableList.ToArray();
    // }

    // public static List<IUpdateGlobalEventManager> FindAllUpdateableManagers(this List<IUpdateGlobalEventManager> allUpdateableManagers, GameObject[] managersOutsideOfContainer)
    // {
    //     for (int i = 0; i < managersOutsideOfContainer.Length; i++)
    //     {
    //         //Add range + getcomponents is used here incase the manager transform holds more than 1 manager on the same gameobject
    //         allUpdateableManagers.AddRange(managersOutsideOfContainer[i].GetComponents<IUpdateGlobalEventManager>());
    //     }

    //     return allUpdateableManagers;
    // }

    // public static IUpdateGlobalEventManager[] FindAllUpdateableManagers(this IUpdateGlobalEventManager[] allUpdateableManagers, GameObject managerContainer, GameObject[] managersOutsideOfContainer)
    // {
    //     List<IUpdateGlobalEventManager> tempUpdateableList = new List<IUpdateGlobalEventManager>();

    //     tempUpdateableList.AddRange(managerContainer.GetComponentsInChildren<IUpdateGlobalEventManager>());

    //     for (int i = 0; i < managersOutsideOfContainer.Length; i++)
    //     {
    //         //Add range + getcomponents is used here incase the manager transform holds more than 1 manager on the same gameobject
    //         tempUpdateableList.AddRange(managersOutsideOfContainer[i].GetComponents<IUpdateGlobalEventManager>());
    //     }

    //     return tempUpdateableList.ToArray();
    // }

    // public static List<IUpdateGlobalEventManager> FindAllUpdateableManagers(this List<IUpdateGlobalEventManager> allUpdateableManagers, GameObject managerContainer, GameObject[] managersOutsideOfContainer)
    // {
    //     allUpdateableManagers.AddRange(managerContainer.GetComponentsInChildren<IUpdateGlobalEventManager>());

    //     for (int i = 0; i < managersOutsideOfContainer.Length; i++)
    //     {
    //         //Add range + getcomponents is used here incase the manager transform holds more than 1 manager on the same gameobject
    //         allUpdateableManagers.AddRange(managersOutsideOfContainer[i].GetComponents<IUpdateGlobalEventManager>());
    //     }
    //     return allUpdateableManagers;
    // }
    // #endregion
    
    public static int CompareManagerExecutionPriority(IGlobalEventManager a, IGlobalEventManager b)
    {
        if (a.ExecutionOrder > b.ExecutionOrder)
            return 1;
        else if (a.ExecutionOrder == b.ExecutionOrder)
            return 0;
        else
            return -1;
    }
    #endregion

    // #region Call Methods

    // public static void GameAwake(this IGlobalEventManager[] managers)
    // {
    //     for (int i = 0; i < managers.Length; i++)
    //     {
    //         managers[i].GameAwake();
    //     }
    // }

    // public static void GameAwake(this List<IGlobalEventManager> managers)
    // {
    //     for (int i = 0; i < managers.Count; i++)
    //     {
    //         managers[i].GameAwake();
    //     }
    // }

    // public static void GameSceneEnter(this IGlobalEventManager[] managers)
    // {
    //     for (int i = 0; i < managers.Length; i++)
    //     {
    //         managers[i].GameSceneEnter();
    //     }
    // }

    // public static void GameSceneEnter(this List<IGlobalEventManager> managers)
    // {
    //     for (int i = 0; i < managers.Count; i++)
    //     {
    //         managers[i].GameSceneEnter();
    //     }
    // }

    // public static void GameStart(this IGlobalEventManager[] managers)
    // {
    //     for (int i = 0; i < managers.Length; i++)
    //     {
    //         managers[i].GameStart();
    //     }
    // }

    // public static void GameStart(this List<IGlobalEventManager> managers)
    // {
    //     for (int i = 0; i < managers.Count; i++)
    //     {
    //         managers[i].GameStart();
    //     }
    // }

    // public static void GamePause(this IGlobalEventManager[] managers)
    // {
    //     for (int i = 0; i < managers.Length; i++)
    //     {
    //         managers[i].GamePause();
    //     }
    // }

    // public static void GamePause(this List<IGlobalEventManager> managers)
    // {
    //     for (int i = 0; i < managers.Count; i++)
    //     {
    //         managers[i].GamePause();
    //     }
    // }

    // public static void GameResume(this IGlobalEventManager[] managers)
    // {
    //     for (int i = 0; i < managers.Length; i++)
    //     {
    //         managers[i].GameResume();
    //     }
    // }

    // public static void GameResume(this List<IGlobalEventManager> managers)
    // {
    //     for (int i = 0; i < managers.Count; i++)
    //     {
    //         managers[i].GameResume();
    //     }
    // }

    // public static void GameReset(this IGlobalEventManager[] managers)
    // {
    //     for (int i = 0; i < managers.Length; i++)
    //     {
    //         managers[i].GameReset();
    //     }
    // }

    // public static void GameReset(this List<IGlobalEventManager> managers)
    // {
    //     for (int i = 0; i < managers.Count; i++)
    //     {
    //         managers[i].GameReset();
    //     }
    // }

    // public static void GameExitScene(this IGlobalEventManager[] managers)
    // {
    //     for (int i = 0; i < managers.Length; i++)
    //     {
    //         managers[i].GameExitScene();
    //     }
    // }

    // public static void GameExitScene(this List<IGlobalEventManager> managers)
    // {
    //     for (int i = 0; i < managers.Count; i++)
    //     {
    //         managers[i].GameExitScene();
    //     }
    // }


    // public static void GameQuit(this IGlobalEventManager[] managers)
    // {
    //     for (int i = 0; i < managers.Length; i++)
    //     {
    //         managers[i].GameQuit();
    //     }
    // }

    // public static void GameQuit(this List<IGlobalEventManager> managers)
    // {
    //     for (int i = 0; i < managers.Count; i++)
    //     {
    //         managers[i].GameQuit();
    //     }
    // }



    // #endregion

    // #region Update Methods
    // public static void GameUpdate(this IUpdateGlobalEventManager[] managers)
    // {
    //     for (int i = 0; i < managers.Length; i++)
    //     {
    //         managers[i].GameUpdate();
    //     }
    // }

    // public static void GameUpdate(this List<IUpdateGlobalEventManager> managers)
    // {
    //     for (int i = 0; i < managers.Count; i++)
    //     {
    //         managers[i].GameUpdate();
    //     }
    // }
    // #endregion

}
