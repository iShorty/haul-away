#if UNITY_EDITOR
namespace LinearEffects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public partial class Block
    {
        public partial class EffectOrder
        {
            #region ISavable Methods
            #region Constants
            public const string PROPERTYNAME_EXECUTORNAME = "Editor_ExecutorName"
            , PROPERTYNAME_FULLEXECUTORNAME = "Editor_FullExecutorName"
            , PROPERTYNAME_REFHOLDER = "_refHolder"
            , PROPERTYNAME_DATAELEMENTINDEX = "_dataElmtIndex"
            ;
            // public const string PROPERTYNAME_ERRORLOG = "ErrorLog";
            #endregion

            ///<Summary>For editor use only. Please do not use it during runtime. The FullExecutorName is the entire string path with all the slashes.</Summary>
            public string Editor_ExecutorName;

            ///<Summary>For editor use only. Please do not use it during runtime. The ExecutorName is whatever you call the Executor at the end of the last slash.</Summary>
            public string Editor_FullExecutorName;

            // public string ErrorLog = "Error";

            ///<Summary>For editor use only. Saves the EffectOrder to a serializedProperty</Summary>
            public void SaveToSerializedProperty(SerializedProperty property)
            {
                property.FindPropertyRelative(PROPERTYNAME_REFHOLDER).objectReferenceValue = _refHolder;
                property.FindPropertyRelative(PROPERTYNAME_DATAELEMENTINDEX).intValue = _dataElmtIndex;
                // property.FindPropertyRelative(PROPERTYNAME_ERRORLOG).stringValue = ErrorLog;
                property.FindPropertyRelative(PROPERTYNAME_EXECUTORNAME).stringValue = Editor_ExecutorName;
                property.FindPropertyRelative(PROPERTYNAME_FULLEXECUTORNAME).stringValue = Editor_FullExecutorName;
            }

            ///<Summary>For editor use only. Loads from a serializedProperty into the EffectOrder </Summary>
            public void LoadFromSerializedProperty(SerializedProperty property)
            {
                _refHolder = (BaseEffectExecutor)property.FindPropertyRelative(PROPERTYNAME_REFHOLDER).objectReferenceValue;
                _dataElmtIndex = property.FindPropertyRelative(PROPERTYNAME_DATAELEMENTINDEX).intValue;
                // ErrorLog = property.FindPropertyRelative(PROPERTYNAME_ERRORLOG).stringValue;
                Editor_ExecutorName = property.FindPropertyRelative(PROPERTYNAME_EXECUTORNAME).stringValue;
                Editor_FullExecutorName = property.FindPropertyRelative(PROPERTYNAME_FULLEXECUTORNAME).stringValue;
            }
            #endregion

            //All these functions are used during unity editor time to manage the Holder's array as well as the OrderData itself
            //none of these will be used in the actual build except for the variables stored
            public virtual void Editor_OnAddNew(BaseEffectExecutor holder)
            {
                _refHolder = holder;
                _dataElmtIndex = _refHolder.Editor_AddNewObject();
            }

            //To be called before removing the order intsance from the list
            public virtual void Editor_OnRemove()
            {
                _refHolder.Editor_RemoveObjectAt(_dataElmtIndex, Editor_ExecutorName);
            }

            //For when the holder is not null
            public virtual void Editor_OnInsertCopy()
            {
                //Tell the holder to do a copy of my current data index details and add it to the end of the array
                _dataElmtIndex = _refHolder.Editor_DuplicateDataElement(_dataElmtIndex);
            }

            public virtual void Editor_OnInsertCopy(BaseEffectExecutor holder)
            {
                _refHolder = holder;
                Editor_OnInsertCopy();
            }


            ///<Summary>Does a manual removal check in the case where the FlowChart editor's OnRemoval event does not include the block in which this EffectOrder is being serialized on</Summary>
            public virtual void Editor_ManualRemovalCheck(int removedIndex)
            {
                if (removedIndex < _dataElmtIndex)
                {
                    _dataElmtIndex--;
                }
            }

            // ///<Summary>Does a manual insert check in the case where the FlowChart editor's OnInsert event does not include the block in which this EffectOrder is being serialized on</Summary>
            // public virtual void ManualInsertCheck(int insertedIndex)
            // {
            //     if (insertedIndex < _dataElmtIndex)
            //     {
            //         _dataElmtIndex++;
            //     }
            // }



        }
    }

}
#endif