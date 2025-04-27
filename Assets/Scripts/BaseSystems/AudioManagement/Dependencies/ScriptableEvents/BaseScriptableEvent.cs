namespace ScriptableObjectEvents
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    // [CreateAssetMenu(fileName = nameof(BaseScriptableEvent), menuName =CREATEASSETMENU_SCRIPTABLEEVENT + "/" +  nameof(BaseScriptableEvent))]
    ///<Summary>A scriptable object which caches an event.</Summary>
    public abstract class BaseScriptableEvent : BetterScriptableObject
    {
        public const string CREATEASSETMENU_SCRIPTABLEEVENT = "ScriptableEvents";

#if UNITY_EDITOR
        [SerializeField]
        [ReadOnly]
        List<string> _subscribedMethods = new List<string>();

        ///<Summary>For debugging purposes only. Adds the action's name into a list of strings to be shown on inspector</Summary>
        protected void RegisterMethodName(Delegate action)
        {
            _subscribedMethods.Add(action.Method.Name);
        }

        ///<Summary>For debugging purposes only. Removes the action's name into a list of strings to be shown on inspector</Summary>
        protected void UnRegisterMethodName(Delegate action)
        {
            _subscribedMethods.Remove(action.Method.Name);
        }

        #region ============ Templates ===============
        // #if UNITY_EDITOR
        // RegisterMethodName(action);
        // #endif

        // #if UNITY_EDITOR
        // UnRegisterMethodName(action);
        // #endif

        // public virtual void SubscribeEvent() { }

        // public virtual void UnSubscribeEvent() { }

        // public virtual void RaiseEvent() { }
        #endregion


#endif




    }
}