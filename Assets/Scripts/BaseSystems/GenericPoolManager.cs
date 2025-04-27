using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericPoolManager : GenericManager<GenericPoolManager>
{
    // [SerializeField, Header("Initialization Settings")]
    // public int ExecutionPriority =>_executionOrder

    BaseGenericPool[] _allPools = default;

    protected override void OnGameAwake()
    {
       _allPools = GetComponentsInChildren<BaseGenericPool>();
        for (int i = 0; i < _allPools.Length; i++)
        {
            _allPools[i].SetUpPools();
        }
    }

    public override void OnDestroy()
    {
        
    }
}
