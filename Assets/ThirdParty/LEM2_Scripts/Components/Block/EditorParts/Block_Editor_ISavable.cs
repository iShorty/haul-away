#if UNITY_EDITOR
namespace LinearEffects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    public partial class Block
    {
        public partial class BlockSettings
        {
            public Color Editor_BlockColour;
            [HideInInspector]
            public Vector2 Editor_BlockPosition;
            [HideInInspector]
            public string[] Editor_ConnectedTowardsBlockNames = new string[0];
        }


        #region Constants
        //Being used in FCWE_NodeManager_NodeCycler.cs
        public const string EDITOR_DEFAULT_BLOCK_NAME = "New Block";

        //All the default and propertypath name constants will be stored here in the Unity_editor section
        static readonly Color EDITOR_DEFAULT_BLOCK_COLOUR = new Color(0, 0.4f, 0.8f, 1f);

        //========================= BLOCK PROPERTYNAMES CONSTANTS =========================================
        public const string EDITOR_PROPERTYNAME_SETTINGS = "_blockSettings";
        public const string EDITOR_PROPERTYPATH_BLOCKNAME = EDITOR_PROPERTYNAME_SETTINGS + ".BlockName";
        public const string EDITOR_PROPERTYPATH_BLOCKCOLOUR = EDITOR_PROPERTYNAME_SETTINGS + ".Editor_BlockColour";
        public const string EDITOR_PROPERTYPATH_BLOCKPOSITION = EDITOR_PROPERTYNAME_SETTINGS + ".Editor_BlockPosition";
        public const string EDITOR_PROPERTYPATH_CONNECTEDTOWARDS_BLOCKNAME = EDITOR_PROPERTYNAME_SETTINGS + ".Editor_ConnectedTowardsBlockNames";

        public const string PROPERTYNAME_ORDERARRAY = "_orderArray";
        #endregion



        //Add all your future variables inside here for saving from a block to a serializedProperty
        public void SaveToSerializedProperty(SerializedProperty blockProperty)
        {
            if (blockProperty.type != typeof(Block).Name)
            {
                Debug.Log($"The serializedProperty {blockProperty} is not of Block class! Property trying to be copied to: {blockProperty.type}");
                return;
            }

            //================ BLOCK SETTINGS ========================
            blockProperty.FindPropertyRelative(Block.EDITOR_PROPERTYPATH_BLOCKCOLOUR).colorValue = _blockSettings.Editor_BlockColour;
            blockProperty.FindPropertyRelative(Block.EDITOR_PROPERTYPATH_BLOCKNAME).stringValue = _blockSettings.BlockName;
            blockProperty.FindPropertyRelative(Block.EDITOR_PROPERTYPATH_BLOCKPOSITION).vector2Value = _blockSettings.Editor_BlockPosition;

            //-------- Connection Lines Array ----------
            SerializedProperty connectionLinesArrayProperty = blockProperty.FindPropertyRelative(Block.EDITOR_PROPERTYPATH_CONNECTEDTOWARDS_BLOCKNAME);
            connectionLinesArrayProperty.ClearArray();
            for (int i = 0; i < _blockSettings.Editor_ConnectedTowardsBlockNames.Length; i++)
            {
                connectionLinesArrayProperty.InsertArrayElementAtIndex(i);
                connectionLinesArrayProperty.GetArrayElementAtIndex(i).stringValue = _blockSettings.Editor_ConnectedTowardsBlockNames[i];
            }
            // blockProperty.FindPropertyRelative(Block.PROPERTYPATH_CONNECTEDTOWARDS_BLOCKNAME).stringValue = _blockSettings.ConnectedTowardsBlockName;


            //============= SAVING ORDER ARRAY =====================
            SerializedProperty orderArrayProperty = blockProperty.FindPropertyRelative(Block.PROPERTYNAME_ORDERARRAY);
            orderArrayProperty.ClearArray();
            //Apply clearing the array first
            orderArrayProperty.serializedObject.ApplyModifiedProperties();
            for (int i = 0; i < _orderArray.Length; i++)
            {
                orderArrayProperty.AddToSerializedPropertyArray(_orderArray[i]);
            }

        }

        //Add all your future variables inside here for loading from a serializedProperty to a block
        public void LoadFromSerializedProperty(SerializedProperty blockProperty)
        {
            if (blockProperty.type != typeof(Block).Name)
            {
                Debug.Log($"The serializedProperty {blockProperty} is not of Block class! Property trying to be copied to: {blockProperty.type}");
                return;
            }

            _blockSettings.Editor_BlockColour = blockProperty.FindPropertyRelative(Block.EDITOR_PROPERTYPATH_BLOCKCOLOUR).colorValue;
            _blockSettings.BlockName = blockProperty.FindPropertyRelative(Block.EDITOR_PROPERTYPATH_BLOCKNAME).stringValue;
            _blockSettings.Editor_BlockPosition = blockProperty.FindPropertyRelative(Block.EDITOR_PROPERTYPATH_BLOCKPOSITION).vector2Value;


            //-------- Connection Lines Array ----------
            SerializedProperty connectionLinesArrayProperty = blockProperty.FindPropertyRelative(Block.EDITOR_PROPERTYPATH_CONNECTEDTOWARDS_BLOCKNAME);
            _blockSettings.Editor_ConnectedTowardsBlockNames = new string[connectionLinesArrayProperty.arraySize];
            for (int i = 0; i < connectionLinesArrayProperty.arraySize; i++)
            {
                _blockSettings.Editor_ConnectedTowardsBlockNames[i] = connectionLinesArrayProperty.GetArrayElementAtIndex(i).stringValue;
            }
            // _blockSettings.ConnectedTowardsBlockName = blockProperty.FindPropertyRelative(Block.PROPERTYPATH_CONNECTEDTOWARDS_BLOCKNAME).stringValue;

            //============= LOADING ORDER ARRAY =====================
            SerializedProperty orderArrayProperty = blockProperty.FindPropertyRelative(Block.PROPERTYNAME_ORDERARRAY);
            _orderArray = new EffectOrder[orderArrayProperty.arraySize];
            for (int i = 0; i < _orderArray.Length; i++)
            {
                SerializedProperty currentElement = orderArrayProperty.GetArrayElementAtIndex(i);
                _orderArray[i] = new EffectOrder();
                _orderArray[i].LoadFromSerializedProperty(currentElement);
            }
        }



    }

}
#endif
