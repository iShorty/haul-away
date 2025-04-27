using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class which has basic floating & sinking function. 
[RequireComponent(typeof(BaseFloaterGroup), typeof(Rigidbody))]
public abstract partial class FloatableProp : MonoBehaviour
{
    #region Definitions
    public enum PropState
    {
        INITIALIZED = 1
        ,
        ///<Summary>State when item has the normal dynamic rigidbody physics</Summary>
        ONLAND = 0
        ,
        ///<Summary>State when item has the water physics but will only float and never sink</Summary>
        INWATER = -1
        ,
        ///<Summary>State when item has the water physics and a floating timer running before it enters sinking state</Summary>
        FLOATING = -2
      ,
        ///<Summary>State when item is sinking into the water. Uses water physics</Summary>
        SINKING = -3
      ,
        ///<Summary>State where item has no physics  </Summary>
        KINEMATIC = -4
    }
    #endregion


    #region Runtime

    [SerializeField]
    protected PropState _currentPropState = PropState.INITIALIZED;
    public PropState CurrentPropState => _currentPropState;

#if UNITY_EDITOR
    [Header("===== PROP RUNTIME =====")]
    [SerializeField, ReadOnly]
#endif
    protected float _propTimer = default;
    #endregion

    #region Hidden Fields
    protected BaseFloaterGroup _floaterGroup = default;
    // protected abstract float floatDuration { get; }
    // protected abstract float sinkDuration { get; }
    #endregion

    #region Properties
    public Rigidbody PropRigidBody { get; protected set; }
    #endregion



    #region Enable Disable
    public virtual void GameAwake()
    {
        PropRigidBody = GetComponent<Rigidbody>();
        _floaterGroup = GetComponent<BaseFloaterGroup>();
        _floaterGroup.enabled = false;

        PropState startingPropstate = _currentPropState;
        _currentPropState = PropState.INITIALIZED;
        SetPropState(startingPropstate);
    }


    // protected virtual void Awake()
    // {
    //     _floaterGroup = GetComponent<BaseFloaterGroup>();
    //     if (_floaterGroup)
    //         _floaterGroup.enabled = false;
    // }

    protected virtual void OnEnable() { }

    protected virtual void OnDisable() { }

    #endregion


    #region Set State

    public void PubSetPropState(PropState stateToSet)
    {
        SetPropState(stateToSet);
    }
    
    //for outside scripts to set the state of the item (water trigger, boat inventory trigger, grappling harpoon and spawner)
    protected virtual void SetPropState(PropState stateToSet)
    {
        if (_currentPropState == stateToSet) return;

        _currentPropState = stateToSet;

        //Reset timer
        _propTimer = 0;

        //Sinking shuldnt be set here because it is handled by 
        switch (stateToSet)
        {
            //========= SET STATE TO ONLAND ==============
            case PropState.ONLAND:
                SetPropStateTo_ONLAND();
                break;

            case PropState.KINEMATIC:
                SetPropStateTo_KINEMATIC();
                break;

            //========= SET STATE TO INWATER ==============
            case PropState.INWATER:
                SetPropStateTo_INWATER();
                break;


            //========= SET STATE TO FLOATING ==============
            case PropState.FLOATING:
                SetPropStateTo_FLOATING();
                break;

            //========= SET STATE TO SINKING ==============
            case PropState.SINKING:
                SetPropStateTo_SINKING();
                break;

            //========= DEFAULT ==============
            default:
#if UNITY_EDITOR
                Debug.LogError("Code should not flow here!");
#endif
                break;
        }

    }


    protected virtual void SetPropStateTo_ONLAND()
    {
        PropRigidBody.isKinematic = false;
        _floaterGroup.enabled = false;
    }

    protected virtual void SetPropStateTo_KINEMATIC()
    {
        PropRigidBody.isKinematic = true;
        _floaterGroup.enabled = false;
    }

    protected virtual void SetPropStateTo_INWATER()
    {
        PropRigidBody.isKinematic = false;
        _floaterGroup.enabled = true;
        RegisterToUpdateLoop();
    }

    protected virtual void SetPropStateTo_FLOATING()
    {
        _propTimer = _floaterGroup.SinkInfo.FloatDuration;
        PropRigidBody.isKinematic = false;
        _floaterGroup.enabled = true;
        RegisterToUpdateLoop();
    }

    protected virtual void SetPropStateTo_SINKING()
    {
        _propTimer = _floaterGroup.SinkInfo.SinkDuration;
        _floaterGroup.StartSinking();
    }

    #endregion

    #region Public OnTrigger Events
    public virtual void Prop_OnEvaluateWaterTrigger()
    {
        // Debug.Log(name + " is in water ", this);
        // if(GetComponent<OctopusController>()) {
        //     Debug.Log("floatable prop, oct in water " + _currentPropState);
        // }
        SetPropState(PropState.FLOATING);
    }

    public virtual void Prop_OnEvaluateBoatInventoryTrigger()
    {
        if (_currentPropState == PropState.KINEMATIC) return;
        SetPropState(PropState.ONLAND);
    }
    #endregion

}
