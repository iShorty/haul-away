namespace ScriptableObjectEvents
{
    using System;
    using UnityEngine;


    ///<Summary>ScriptableObject which caches an event. Returns void when invoked, requires a float parameter</Summary>
    [CreateAssetMenu(fileName = nameof(SOEvent_RVoid_Float), menuName = BaseScriptableEvent.CREATEASSETMENU_SCRIPTABLEEVENT + "/" + nameof(SOEvent_RVoid_Float))]
    public class SOEvent_RVoid_Float : BaseScriptableEvent
    {

        protected event Action<float> void_FloatEvent = null;

        public virtual void SubscribeEvent(Action<float> action)
        {
            void_FloatEvent += action;
#if UNITY_EDITOR
            RegisterMethodName(action);
#endif
        }

        public virtual void UnSubscribeEvent(Action<float> action)
        {
            void_FloatEvent -= action;
#if UNITY_EDITOR
            UnRegisterMethodName(action);
#endif
        }

        public virtual void RaiseEvent(float f)
        {
            void_FloatEvent?.Invoke(f);
        }



    }
}