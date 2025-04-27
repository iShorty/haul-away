#if UNITY_EDITOR
namespace LinearEffects
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    public partial class Block
    {

        #region Initialization
        //Used in FCWE_NodeManager_NodeCreation.cs
        public Block(Vector2 position, string blockName, Color blockColour)
        {
            _blockSettings = new BlockSettings();
            _blockSettings.BlockName = blockName;
            _blockSettings.Editor_BlockPosition = position;
            _blockSettings.Editor_BlockColour = blockColour;
        }

        //Used in FCWE_NodeManager_NodeCreation.cs
        public Block(Vector2 position, string blockName)
        {
            EditorProperties_DefaultConstruction();
            _blockSettings.BlockName = blockName;
            _blockSettings.Editor_BlockPosition = position;
        }

        //Used in BlockScriptableInstance.cs
        public Block()
        {
            EditorProperties_DefaultConstruction();
        }

        void EditorProperties_DefaultConstruction()
        {
            _blockSettings = new BlockSettings();
            _blockSettings.Editor_BlockColour = EDITOR_DEFAULT_BLOCK_COLOUR;
        }

        #endregion

        #region Sets & Gets
        ///<Summary>For forceful renaming of block names when the flowchart window manager already contains a block with similar name</Summary>
        public void Editor_SetBlockName(string blockName) { _blockSettings.BlockName = blockName; }

        public EffectOrder[] OrderArray => _orderArray;

        #endregion

        #region Adding Methods
        ///<Summary>Adds a new order element into the block class. We need a gameobject which this Block class is being serialized on.</Summary>
        public EffectOrder EditorProperties_AddNewOrderElement(BaseEffectExecutor executor, string fullEffectorName, string executorName)
        {
            //Get new effect order
            EffectOrder addedOrder = EditorProperties_GetNewOrderData(executor, fullEffectorName, executorName);
            // EffectOrder addedOrder = EditorProperties_GetNewOrderData(gameObject, type, fullEffectorName, executorName);
            ArrayExtension.Add(ref _orderArray, addedOrder);
            return addedOrder;
        }

        #region  Insert Methods
        //I assume this is for copy/cut pasting of orders
        ///<Summary>Inserts a new instance of OData into a currently existing holder type instance (if there is no holder type present, a new one will be added)</Summary>
        public virtual void EditorProperties_InsertOrderElement(EffectOrder orderData, int index)
        {

            //Call Insert copy functions
            orderData.Editor_OnInsertCopy();
            ArrayExtension.Insert(ref _orderArray, index, orderData);
        }


        #endregion


        #region Get New OrderData Methods
        //Since this class is not deriving from a monobehaviour, we need to pass in the reference of the gameobject this class is being serialized on
        ///<Summary>Returns an instance of initialized EffectOrder (Therefore we need typeOfHolder to ensure that such a component exists on the block's gameobject so that the OrderData can hold a reference to it)</Summary>
        protected EffectOrder EditorProperties_GetNewOrderData(BaseEffectExecutor executor, string fullExecutorName, string executorName)
        {
            Block.EffectOrder o = new Block.EffectOrder();

            o.Editor_ExecutorName = executorName;
            o.Editor_FullExecutorName = fullExecutorName;

            o.Editor_OnAddNew(executor);
            return o;
        }
        #endregion

        #endregion

        #region Remove Methods
        public void EditorProperties_RemoveAllOrderData()
        {
            //Remove order data from the largest element index
            for (int i = _orderArray.Length - 1; i > -1; i--)
            {
                EditorProperties_RemoveOrderElementAt(i);
            }
        }

        public virtual void EditorProperties_RemoveOrderElementAt(int index)
        {
            //Call callbacks
            _orderArray[index].Editor_OnRemove();
            ArrayExtension.RemoveAt(ref _orderArray, index);
        }

        public virtual void EditorProperties_ManualOnRemovalCheck(int removedEffectOrderIndex, string executorSearchBoxName)
        {
            EffectOrder removedEffectOrder = _orderArray[removedEffectOrderIndex];
            int removedDataElmtIndex = removedEffectOrder.DataElementIndex;
            foreach (var item in _orderArray)
            {
                //Check if the effect order has the same executor name as the one that the removed effect was situated in
                if (item.Editor_ExecutorName != executorSearchBoxName)
                {
                    continue;
                }

                item.Editor_ManualRemovalCheck(removedEffectOrderIndex);
            }
        }

        #endregion

    }

}
#endif
