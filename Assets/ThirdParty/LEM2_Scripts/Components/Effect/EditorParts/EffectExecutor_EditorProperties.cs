#if UNITY_EDITOR
namespace LinearEffects
{
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using System.Linq;
    public abstract partial class EffectExecutor<T>
    {

        protected override Type Editor_EffectClassType => typeof(T);

        #region Window Editor Method
        //We need this to allow easy access to converting _effectDatas to object[] and setting of _effectDatas's value to the T generic type
        ///<Summary>Returns the _effectDatas array and sets it to the appropriate Effect Generic type </Summary>
        object[] Editor_DataArrayObject
        {
            get
            {
                return _effectDatas;
            }
            set
            {
                _effectDatas = value.Select(x => (T)x).ToArray();
            }
        }

        public override int Editor_AddNewObject()
        {
            //========= ADDING NEW OBJECT TYPE TO THE ARRAY =============
            object[] objectArray = Editor_DataArrayObject;
            Type dataType = objectArray.GetType().GetElementType();
            int elementIndex = ArrayExtension.AddReturn(ref objectArray, Activator.CreateInstance(dataType));
            Editor_DataArrayObject = objectArray;


            return elementIndex;
        }

        public override void Editor_RemoveObjectAt(int index, string executorSearchBoxName)
        {
            //========= REMOVING OBJECT TYPE FROM THE ARRAY =============
            object[] objectArray = Editor_DataArrayObject;
            ArrayExtension.RemoveAt(ref objectArray, index);
            Editor_DataArrayObject = objectArray;

            //Only call this event if there is more than 0 elements in the object array after removal
            if (Editor_DataArrayObject.Length > 0)
            {
                //Getting baseEffector's effectname
                // Type thisType = GetType();
                // string effectName = thisType.Name;
                OnRemoveObject?.Invoke(index, executorSearchBoxName);
                return;
            }

            //Else, remove destroy this component
            DestroyImmediate(this);
        }

        public override int Editor_DuplicateDataElement(int index)
        {
            // Duplicates a copy of the class using reflection's deep copy 
            object[] objectArray = Editor_DataArrayObject;

            object objectToDuplicate = objectArray[index];

            object copy = ReflectionExtensions.DeepCopy(objectToDuplicate);

            int elementIndex = ArrayExtension.AddReturn(ref objectArray, copy);
            Editor_DataArrayObject = objectArray;
            return elementIndex;
        }

        #endregion

    }

}
#endif