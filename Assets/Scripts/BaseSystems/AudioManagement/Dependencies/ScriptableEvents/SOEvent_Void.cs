namespace ScriptableObjectEvents
{
    using System;
    using UnityEngine;


    ///<Summary>ScriptableObject which caches an event. Returns void when invoked and does not require any parameters</Summary>
    [CreateAssetMenu(fileName = nameof(SOEvent_Void), menuName = BaseScriptableEvent.CREATEASSETMENU_SCRIPTABLEEVENT + "/" + nameof(SOEvent_Void))]
    public class SOEvent_Void : BaseScriptableEvent
    {

        protected event Action voidEvent = null;

        public virtual void SubscribeEvent(Action action)
        {
            voidEvent += action;
#if UNITY_EDITOR
            RegisterMethodName(action);
#endif
        }

        public virtual void UnSubscribeEvent(Action action)
        {
            voidEvent -= action;
#if UNITY_EDITOR
            UnRegisterMethodName(action);
#endif
        }

        public virtual void RaiseEvent()
        {
            voidEvent?.Invoke();
        }



    }
}