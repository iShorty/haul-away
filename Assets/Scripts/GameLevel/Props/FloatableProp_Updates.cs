using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class FloatableProp : MonoBehaviour
{
    #region Update

    #region Abstract
    ///<Summary>
    ///Return this prop to a pool if it has any. If it doesnt, then destroy this gameObject or do something with it
    ///</Summary>
    protected abstract void OnSinkTimerUp();


    ///<Summary>
    ///Called when FloatableProp enters the INWATER or FLOATING state. If this script is inherited into a script that doesnt call update, you will have to create an external script which will do the updating for you and add it into up list to update every frame and every fixedupdate frame
    ///</Summary>
    protected abstract void RegisterToUpdateLoop();

    #endregion

    ///<Summary>
    ///Returns true when the BaseItem enters a state where there is no need for Updating
    ///</Summary>
    public virtual bool GameUpdate()
    {
        switch (_currentPropState)
        {
            case PropState.FLOATING:
                return Prop_GameUpdate_FLOATING();

            case PropState.SINKING:
                return Prop_GameUpdate_SINKING();

            case PropState.INWATER: return false;

            case PropState.ONLAND: return true;

            case PropState.KINEMATIC: return true;


            default:
#if UNITY_EDITOR
                Debug.LogError("Code should not flow here!");
#endif
                return true;

        }

    }

    protected virtual bool Prop_GameUpdate_SINKING()
    {
        //Call the floatergroups sinking method
        _floaterGroup.GameUpdate();

        if (CountDownTimer())
        {
            OnSinkTimerUp();
            return true;
        }

        return false;
    }

    protected virtual bool Prop_GameUpdate_FLOATING()
    {
        if (CountDownTimer())
        {
            //Time to sink
            SetPropState(PropState.SINKING);
        }

        return false;
    }

    protected virtual bool CountDownTimer()
    {
        _propTimer -= Time.deltaTime;

        if (_propTimer <= 0) return true;

        return false;
    }

    #endregion

//Doesnt have protected virtual methods for each state for fixedupdate because the methods called are oneliners
    #region FixedUpdate

    public virtual void FixedGameUpdate()
    {
        switch (_currentPropState)
        {
            case PropState.INWATER:
                _floaterGroup.GameFixedUpdate();
                break;

            case PropState.FLOATING:
                _floaterGroup.GameFixedUpdate();
                break;

            case PropState.SINKING:
                _floaterGroup.GameFixedUpdate();
                break;
            default:
                break;
        }
    }

    #endregion


}
