using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManagement;

[RequireComponent(typeof(BoxCollider))]
public class WaterManager : LevelSingleton<WaterManager>
{
    [SerializeField] VFXInfo waterSplashInfo = default;

    #region Const
    static readonly Vector3 WATERCOLLIDERSIZE = new Vector3(10000, 0.5f, 10000);
    #endregion

    #region Handle Manager Methods

    protected override void GameAwake()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
#if UNITY_EDITOR
        Debug.Assert(collider, $"Water manager must have a boxcollider on it!", this);
        Debug.Assert(collider.isTrigger, $"The water collider {collider.name} must be set to isTrigger!", this);
        Debug.Assert(collider.gameObject.layer == Constants.For_Layer_and_Tags.LAYERINDEX_WATER, $"The water collider doesnt have the water layer index assigned!", this);
#endif
        collider.size = WATERCOLLIDERSIZE;
    }

    #endregion


    #region OnTrigger Events
    void OnTriggerEnter(Collider collider)
    {
        EvaluateTrigger(collider);
    }

    void EvaluateTrigger(Collider collider)
    {
        // #if UNITY_EDITOR
        //         Debug.Log($"{collider.name} has entered the water!", collider);
        // #endif
        if (collider.attachedRigidbody == null) return;

        FloatableProp p = collider.attachedRigidbody.GetComponent<FloatableProp>();

        if (!p)
        {
#if UNITY_EDITOR
            if (collider.GetComponent<BomberController>() != null)
                Debug.Log(collider.name + " wtf bomber", collider);
#endif
            return;
        }

        p.Prop_OnEvaluateWaterTrigger();

        VFXObj e = VFXPool.GetInstanceOf(waterSplashInfo.Prefab, p.transform.position);
        e.Initialise();
        
        AudioEvents.RaiseOnPlay3DAtLocation(AudioClipType.SFX_WaterSplash, e.transform.position, true, true);
    }

    #endregion

}
