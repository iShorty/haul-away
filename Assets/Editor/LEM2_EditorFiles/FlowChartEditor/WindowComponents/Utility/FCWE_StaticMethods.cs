namespace LinearEffectsEditor
{
    using UnityEditor;
    using LinearEffects;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public partial class FlowChartWindowEditor : EditorWindow
    {
        public delegate void CompareDataElementIndexCallback(SerializedProperty dataElementProperty, int dataElementIndex);

        ///<Summary>Ensures and checks that there will always be the executor typed component with the System.Serializable attribute for its Effect class on the gameobject and that it always has the OnRemoval event assigned on the baseEffectExecutor. Returns true if the component class passes the checks else return false</Summary>
        public static bool StaticMethods_EnsureExecutorComponent(GameObject gameObject, Type holderType, out BaseEffectExecutor holder)
        {
            holder = null;

            if (!holderType.IsSubclassOf(typeof(BaseEffectExecutor)))
            {
                Debug.Log($"Type {holderType} does not inherit from {typeof(BaseEffectExecutor)} and therefore adding this type to the OrderData is not possible!");
                return false;
            }


            if (!gameObject.TryGetComponent(holderType, out Component component))
            {
                //If no component found, insert the orderdata into the order array
                component = gameObject.AddComponent(holderType);
                holder = component as BaseEffectExecutor;

                if (CheckHolderSerializableImplementation(holder))
                {
                    holder.Editor_InitializeSubs(StaticMethods_HandleOnRemoveEvent);
                    return holder;
                }

                //Else if there is no serializable attribute on the effect class, destroy the newly added component
                return false;
            }

            holder = component as BaseEffectExecutor;

            return CheckHolderSerializableImplementation(holder) ? holder : null;
        }

        ///<Summary>Returns true if holder's effect type has a System.Serializable attribute on it. If false, it will destroy the holder component</Summary>
        static bool CheckHolderSerializableImplementation(BaseEffectExecutor holder)
        {
            if (holder.Editor_CheckAttributeImplementation(out string errorLog))
            {
                return true;
            }

            DestroyImmediate(holder);
            Debug.LogError(errorLog);
            return false;
        }

        static void StaticMethods_HandleOnRemoveEvent(int removedIndex, string effectorName)
        {
            //Compare the index and if the removed index is smaller than the data element index being looped checked thru, decrement it
            instance.NodeManager_SaveManager_CompareDataElementIndex
            (
                (SerializedProperty dataElementProperty, int dataElementIndex) =>
                {
                    if (removedIndex < dataElementIndex)
                    {
                        //set the data element index to something decremented
                        dataElementIndex--;
                        dataElementProperty.serializedObject.Update();
                        dataElementProperty.intValue = dataElementIndex;
                        dataElementProperty.serializedObject.ApplyModifiedProperties();
                    }
                }
                ,
                effectorName
            )
            ;
        }

        //This method does a check between element index but only via serialized property
        void NodeManager_SaveManager_CompareDataElementIndex(CompareDataElementIndexCallback compareDataElementIndex, string effectorName)
        {
            //Search through every block
            for (int blockIndex = 0; blockIndex < _allBlocksArrayProperty.arraySize; blockIndex++)
            {
                SerializedProperty blockProperty = _allBlocksArrayProperty.GetArrayElementAtIndex(blockIndex);
                SerializedProperty orderArray = blockProperty.FindPropertyRelative(Block.PROPERTYNAME_ORDERARRAY);

                for (int orderIndex = 0; orderIndex < orderArray.arraySize; orderIndex++)
                {
                    //Check if block's effect order name is the same as the fullEffectname
                    SerializedProperty orderElement = orderArray.GetArrayElementAtIndex(orderIndex);
                    string orderElementEffectName = orderElement.FindPropertyRelative(Block.EffectOrder.PROPERTYNAME_EXECUTORNAME).stringValue;

                    if (orderElementEffectName != effectorName)
                    {
                        continue;
                    }

                    // Debug.Log($"EffectName {effectorName} FullName: {orderElementEffectName}");


                    //Check if the removed index is smaller than this order element's index
                    SerializedProperty dataElementProperty = orderElement.FindPropertyRelative(Block.EffectOrder.PROPERTYNAME_DATAELEMENTINDEX);
                    int dataElmtIndex = dataElementProperty.intValue;
                    compareDataElementIndex?.Invoke(dataElementProperty, dataElmtIndex);
                }


            }

        }

    }


}