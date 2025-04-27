using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXPool : GenericPools<VFXObj, VFXPool>, IGlobalEventManager
{

    [Header("----- Manager Settings -----")]
    [SerializeField]
    private int _executionPriority;
    

    #region  Properties

    public int ExecutionOrder => _executionPriority;
    public static VFXPool Instance => instance;

    #endregion  


    #region HiddenField

    List<VFXObj> activeVFX = new List<VFXObj>();


    #endregion


    public static void RegisterVFX(VFXObj vfx)
    {
        instance.activeVFX.Add(vfx);
    }

    public static void UnregisterVFX(VFXObj vfx)
    {
        instance.activeVFX.Remove(vfx);
    }


    public void GameAwake()
    {
        GlobalEvents.OnGameUpdate_DURINGGAME += DuringGameUpdate;
        GlobalEvents.OnGameReset += HandleGameReset;
    }

    public  void OnDestroy()
    {
        GlobalEvents.OnGameUpdate_DURINGGAME -= DuringGameUpdate;
        GlobalEvents.OnGameReset -= HandleGameReset;

    }

    private void HandleGameReset()
    {
        // 

    }

    void DuringGameUpdate()
    {
        //Update the rest
        for (int i = 0; i < activeVFX.Count; i++)
        {
            activeVFX[i].GameUpdate();
        }
    }


    #region Pooler Methods

    public static void ReturnInstanceOf(VFXObj o)
    {
        ReturnInstanceOf(o.VFXInfo.Prefab, o);
    }

    // public static VFXObj GetInstanceOf(VFXInfo info, Transform parent, Vector3 localPosition, Vector3 localRotation)
    // {
    //     var obj = instance.GetInstance(info.Prefab, parent, localPosition);
    //     obj.
    //     return 
    // }



    #endregion
}


