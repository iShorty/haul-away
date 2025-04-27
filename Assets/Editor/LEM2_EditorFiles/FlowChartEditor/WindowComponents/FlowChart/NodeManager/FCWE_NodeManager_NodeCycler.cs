
namespace LinearEffectsEditor
{
    using UnityEngine;
    using UnityEditor;
    using LinearEffects;
    using System.Collections.Generic;

    //This code handles all the creation and deletion of block nodes in the window editor 
    public partial class FlowChartWindowEditor : EditorWindow
    {

        #region Constants
        const string DELETEWARNING_TITLE = "Are you sure you want to delete?"
        , DELETEWARNING_MESSAGE = "Deletion cannot be undoned once executed"
        , DELETEWARNING_OK = "Continue"
        , DELETEWARNING_CANCEL = "Cancel"
        ;

        const float DUPLICATE_OFFSET = 5f;
        #endregion

        bool _deleteWarningBox = default;


        #region Lifetime Methods

        void NodeManager_NodeCycler_OnGUI()
        {
            if (_newBlockFromEnum != AddNewBlockFrom.None)
            {
                NodeManager_NodeCycler_AddNewNode();
            }

            //=========== DRAW DELETE BOX ==============
            NodeManager_NodeCycler_DrawDeleteWarning();
        }

        #endregion


        string NodeManager_NodeCycler_GetUniqueBlockName(string defaultName)
        {
            //Add a space to ensure that there is a whitespace
            string s = defaultName + " ";

            for (int i = 0; i < int.MaxValue; i++)
            {
                string unqName = s + i;
                //Keep looping if there is an entry called "New Block 1", "New Block 2", ....
                if (_allBlockNodesDictionary.ContainsKey(unqName))
                    continue;

                //Else if the dict doesnt hv this name,
                return unqName;
            }

            Debug.LogError($"There is no way you create 2147483647 blocks with all their names as {defaultName} <number>.... you monster why are you like this....");
            return null;
        }

        #region Creating NodeBlocks
        BlockNode NodeManager_NodeCycler_CreateNewNodeConstructor(SerializedProperty blockProperty)
        {
            return new BlockNode(blockProperty, NodeManager_ArrowConnectionCycler_ConnectToBlockNode);
        }

        void NodeManager_NodeCycler_TriggerCreateNewNode(AddNewBlockFrom from)
        {
            _newBlockFromEnum = from;
        }

        BlockNode NodeManager_NodeCycler_AddNewNode()
        {
            BlockNode node;
            switch (_newBlockFromEnum)
            {
                case AddNewBlockFrom.ContextMenu:
                    node = NodeManager_NodeCycler_AddNewNode(Event.current.mousePosition);
                    break;

                case AddNewBlockFrom.ToolBar:
                    node = NodeManager_NodeCycler_AddNewNode(CenterScreen);
                    break;

                //Fall back code
                default:
                    node = NodeManager_NodeCycler_AddNewNode(Vector2.zero);
                    break;
            }

            return node;
        }

        BlockNode NodeManager_NodeCycler_AddNewNode(Vector2 position)
        {
            //Ensure that block gets a unique label name every time a new node is added

            Block b = new Block(position, NodeManager_NodeCycler_GetUniqueBlockName(Block.EDITOR_DEFAULT_BLOCK_NAME));

            SerializedProperty newBlockProperty = _allBlocksArrayProperty.AddToSerializedPropertyArray(b);
            BlockNode node = NodeManager_NodeCycler_CreateNewNodeConstructor(newBlockProperty);

            _allBlockNodes.Add(node);
            _allBlockNodesDictionary.Add(node.Label, node);
            _newBlockFromEnum = AddNewBlockFrom.None;
            return node;
        }



        #endregion

        #region Deleting NodeBlocks
        void NodeManager_NodeCycler_DeleteButton()
        {
            //Check if the block or anyone of the selected block has more than 1 effects inside of it then
            foreach (var blockNode in _selectedBlocks)
            {
                int effectCount = blockNode.GetEffectCount;
                //If effect count is more than 0 exit loop and open up warning window
                if (effectCount > 0)
                {
                    // Debug.Log($"There are {effectCount} effects! Open warning window");
                    //add a pop up to say that there is no way of getting it back
                    _deleteWarningBox = true;
                    return;
                }
            }

            //Delete nodes if all of the nodes are empty of effect orders
            NodeManager_NodeCycler_DeleteSelectedNodes();
        }

        void NodeManager_NodeCycler_DrawDeleteWarning()
        {
            if (!_deleteWarningBox)
            {
                return;
            }

            switch (EditorUtility.DisplayDialogComplex(DELETEWARNING_TITLE, DELETEWARNING_MESSAGE, DELETEWARNING_OK, DELETEWARNING_CANCEL, string.Empty))
            {
                //============= OK BUTTON PRESSED ==============
                case 0:
                    NodeManager_NodeCycler_DeleteSelectedNodes();
                    //Close the warning box
                    _deleteWarningBox = false;
                    break;


                //=============== CANCEL BUTTON PRESSED ===============
                case 1:
                    _deleteWarningBox = false;
                    break;
                // ==================== ALT BUTTON PRESSED ================
                default:
                    _deleteWarningBox = false;
                    break;
            }

        }


        //if player accepts, yeet the entire selected blocks
        ///<Summary>Deletes selected nodes</Summary>
        void NodeManager_NodeCycler_DeleteSelectedNodes()
        {
            //Close the block editor 
            BlockEditor_HandleOnNoBlockNodeFound();

            //ORDER OF REMOVAL:
            //Remove any arrow connection lines (dependent on list & dictionary)
            //Remove their order data & effects from their respective holders
            //Remove them from the list and the dictionary
            //Remove the block itself from the block array property

            //----------------------- Removing Arrow Connection Lines -----------------
            foreach (var blockNode in _selectedBlocks)
            {
                NodeManager_ArrowConnectionCycler_DeleteAllArrowConnectionLinesFrom(blockNode.Label);
                NodeManager_ArrowConnectionCycler_DeleteAllArrowConnectionLinesTo(blockNode.Label);
            }


            //----------------------- Removing their Order Data & Effects from their respective Holders -----------------
            foreach (var blockNode in _selectedBlocks)
            {
                //Get block from flow chart and then remove all order data
                Block block = new Block();
                block.LoadFromSerializedProperty(blockNode.BlockProperty);
                // Block block = _flowChart.Editor_GetBlock(blockNode.Label);

                //Delete order data as well as from the respective holders from block
                blockNode.BlockProperty.serializedObject.Update();
                block.EditorProperties_RemoveAllOrderData();
                block.SaveToSerializedProperty(blockNode.BlockProperty);
                blockNode.BlockProperty.serializedObject.ApplyModifiedProperties();

            }

            //----------------------- Removing from list & dictionary -----------------
            foreach (var blockNode in _selectedBlocks)
            {
                //Remove from list
                _allBlockNodes.Remove(blockNode);
                _allBlockNodesDictionary.Remove(blockNode.Label);
            }

            //------------ Delete the Blocks from the Block Array Property -------------
            _allBlocksArrayProperty.serializedObject.Update();
            foreach (var blockNode in _selectedBlocks)
            {
                //Delete block from block array
                _allBlocksArrayProperty.DeleteArrayElement
                   (
                       (x) =>
                       {
                           string a = x.FindPropertyRelative(Block.EDITOR_PROPERTYPATH_BLOCKNAME).stringValue;
                           return a == blockNode.Label;
                       }
                   );
            }
            _allBlocksArrayProperty.serializedObject.ApplyModifiedProperties();


            NodeManager_ClearAllSelectedNodes();
            NodeManager_SaveManager_LoadCachedBlockNodes();
        }

        #endregion

        #region Duplicating NodeBlocks
        void NodeManager_NodeCycler_DuplicateSelectedNodes()
        {
            List<BlockNode> duplicatedNodes = new List<BlockNode>();

            foreach (BlockNode nodeToDuplicate in _selectedBlocks)
            {
                //Close blocknode editor if it is being yeeted
                if (isBlockEditorOpen && _blockEditor.Block.BlockName == nodeToDuplicate.Label)
                {
                    BlockEditor_HandleOnNoBlockNodeFound();
                }

                _allBlocksArrayProperty.serializedObject.Update();
                BlockNode duplicatedNode = NodeManager_NodeCycler_DuplicateNode(nodeToDuplicate);
                _allBlocksArrayProperty.serializedObject.ApplyModifiedProperties();
                duplicatedNodes.Add(duplicatedNode);
            }

            NodeManager_ClearAllSelectedNodes();

            //Now select all of the duplicated nodes
            foreach (var duplicatedNode in duplicatedNodes)
            {
                int nodeIndex = _allBlockNodes.FindIndexFromLastIndex(x => x.Label == duplicatedNode.Label);
                NodeManager_SelectNode(nodeIndex);
            }

        }

        BlockNode NodeManager_NodeCycler_DuplicateNode(BlockNode nodeToDuplicate)
        {
            //============== DUPLICATING THE SELECTED NODE'S BLOCK ============
            Block blockToDuplicate = _flowChart.Editor_GetBlock(nodeToDuplicate.Label);
            Vector2 newPosition = nodeToDuplicate.Position + Vector2.one * DUPLICATE_OFFSET;

            //Create a new block with the identical position
            Block duplicateBlock = new Block(newPosition, NodeManager_NodeCycler_GetUniqueBlockName(blockToDuplicate.BlockName), nodeToDuplicate.Colour);

            //============ DUPLICATE EVERYTHING FROM BLOCKTODUPLICATE ONTO THE DUPLICATE ==============
            SerializedProperty orderArray = nodeToDuplicate.BlockProperty.FindPropertyRelative(Block.PROPERTYNAME_ORDERARRAY);

            for (int i = 0; i < orderArray.arraySize; i++)
            {
                //Get the an effect order to duplicate
                SerializedProperty effectOrderProperty = orderArray.GetArrayElementAtIndex(i);

                //Do a effect type check here incase some naming conventions change
                string fullEffectName = effectOrderProperty.FindPropertyRelative(Block.EffectOrder.PROPERTYNAME_FULLEXECUTORNAME).stringValue;
                string effectName = effectOrderProperty.FindPropertyRelative(Block.EffectOrder.PROPERTYNAME_EXECUTORNAME).stringValue;

                // if (!EffectsData.TryGetExecutor(fullEffectName, out Type executorType))
                // {
                //     Debug.LogError($"The effectname : {effectName} cannot be found in CommandData anymore! Full path: {fullEffectName}");
                //     return;
                // }

                //Copy the values of current effect order to duplicate from onto the new instance of effect order
                Block.EffectOrder effectOrder = new Block.EffectOrder();
                effectOrder.LoadFromSerializedProperty(effectOrderProperty);

                //Add the effectorder into the duplicate block
                //Always ensure that there is a executor component on the flowchart gameobject
                // StaticMethods_EnsureExecutorComponent(_flowChart.gameObject, executorType);
                duplicateBlock.EditorProperties_InsertOrderElement(effectOrder, i);
                // duplicateBlock.EditorProperties_InsertOrderElement(_flowChart.gameObject, type, effectOrder, i);
            }


            //====== ADDING THE DUPLICATED NODE BACK TO THE ARRAYPROPERTY =============
            SerializedProperty duplicatedBlockProperty = _allBlocksArrayProperty.AddToSerializedPropertyArray(duplicateBlock);
            BlockNode duplicatedBlockNode = NodeManager_NodeCycler_CreateNewNodeConstructor(duplicatedBlockProperty);


            //Record all the newly added node
            _allBlockNodes.Add(duplicatedBlockNode);
            _allBlockNodesDictionary.Add(duplicatedBlockNode.Label, duplicatedBlockNode);
            return duplicatedBlockNode;
        }


        #endregion

    }

}

