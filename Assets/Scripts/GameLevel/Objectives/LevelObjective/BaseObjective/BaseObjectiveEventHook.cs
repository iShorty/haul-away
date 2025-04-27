using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>The monobehaviour instance which will be instantiated by the GameManager when the level is entered and then hooks into the respective events the objective is supposed to subscribe to</Summary>
public abstract class BaseObjectiveEventHook : MonoBehaviour
{
    #region  ------------------ Statics --------------------
    ///<Summary>Called when a star objective condition has been fulfilled</Summary>
    public static event Action<int> OnStarConditionFulFilled = null;

    ///<Summary>Called when a star objective condition has been fulfilled. The raiser must be an objective hook that inherits from this script</Summary>
    protected static void RaiseStarConditionFulFilled(BaseObjectiveEventHook hook, int objectiveIndex)
    {
        hook.FulFilled = true;
        OnStarConditionFulFilled?.Invoke(objectiveIndex);
    }
    #endregion


    ///<Summary>The hook's LevelObjectiveInfo's index in the array of objectives in a level</Summary>
    protected int _objectiveIndex = -1;

    public bool FulFilled { get; protected set; } = false;
    // public bool Fulfilled => _fulfilled;


    ///<Summary>Dump all of the event subscriptions here, plese account for the other game events also like Reset</Summary>
    protected virtual void Awake()
    {
        GlobalEvents.OnGameStart += ResetEventHook;
        GlobalEvents.OnGameReset += ResetEventHook;
    }

    ///<Summary>Dump all of the event unsubscriptions here</Summary>
    protected virtual void OnDestroy()
    {
        GlobalEvents.OnGameStart -= ResetEventHook;
        GlobalEvents.OnGameReset -= ResetEventHook;
    }

    #region ------------------- Overridable Methods ------------------------------
    ///<Summary>Create a serialized field to cache the info type passed in. If you have to cast it, try using "as" keyword first. Dont worry this will only be called once per level</Summary>
    public abstract void SetObjectiveInfo(BaseLevelObjectiveInfo info);

    ///<Summary>Resets the event hook monobehaviour when game start or game reset is called. Override this to reset your values. Call base after your override to update the text properly</Summary>
    protected virtual void ResetEventHook()
    {
        FulFilled = false;
        UpdateObjectiveText();
    }

    ///<Summary>Updates the objective text in the main game ui. Please call InGameUI_UpdateObjectiveText() from the GameUI script inside this function. AND place this function inside whatever events you subscribe to to check if your condition is fulfilled</Summary>
    protected abstract void UpdateObjectiveText();

    #endregion

    #region  ------------------- Public Methods --------------------
    public void SetObjectiveIndex(int objectiveIndex)
    {
        _objectiveIndex = objectiveIndex;
    }
    #endregion

}
