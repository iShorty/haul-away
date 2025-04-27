#if UNITY_EDITOR
namespace LinearEffects
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;


    public abstract partial class BaseEffectExecutor
    {
        //This is stored here because EffectExecutor class is a generic class
        public const string EDITOR_PROPERTYNAME_EFFECTDATAS = "_effectDatas";

        public delegate void Editor_ChangeObjectArrayCallBack(int objectIndex, string effectName);

        #region Subscribing to EventList
        protected Editor_ChangeObjectArrayCallBack OnRemoveObject = null;

        public virtual void Editor_InitializeSubs(Editor_ChangeObjectArrayCallBack onRemove)
        {
            OnRemoveObject = onRemove;
        }
        #endregion

        public abstract int Editor_AddNewObject();

        public abstract void Editor_RemoveObjectAt(int index, string executorSearchBoxName);

        public abstract int Editor_DuplicateDataElement(int index);

        ///<Summary>Used to check whether the T generic type has the System.Serializable attribute on it. Returns true if there is Serializable Attribute on the Effect ClassType</Summary>
        public bool Editor_CheckAttributeImplementation(out string errorLog)
        {
            errorLog = string.Empty;
            Type effectClassType = Editor_EffectClassType;

            if (Attribute.IsDefined(effectClassType, typeof(SerializableAttribute)))
            {
                return true;
            }

            errorLog = $"The Effect Class: {effectClassType} does not have the attribute System.Serializable on it!";
            return false;
        }

        protected abstract Type Editor_EffectClassType { get; }

    }

}
#endif