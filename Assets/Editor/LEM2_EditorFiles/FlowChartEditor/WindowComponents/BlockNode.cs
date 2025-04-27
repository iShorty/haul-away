namespace LinearEffectsEditor
{
    using LinearEffects;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    //Graphical representations of block in the flowchart window editor
    public class BlockNode
    {
        #region Definition
        public delegate void CreateNewArrowConnectionLineCallback(BlockNode endNode);
        #endregion

        #region Constants
        //========================= NODE CONSTANTS =========================================
        static readonly Rect NODEBLOCK_SIZE = new Rect(Vector2.zero, new Vector2(100f, 50f));
        const float NODEBLOCK_SELECTION_THICKNESS = 5f
        ;

        const string NODEBLOCK_ARROWMODE_BUTTON_TEXT = "Connect to \n";
        static readonly Vector2 NODEBLOCK_ARROWMODE_BUTTON_SIZE = new Vector2(75f, 20f);
        static readonly Vector2 NODEBLOCK_ARROWCONNECTIONPOINT_OFFSET = new Vector2(20, 20);

        static readonly float NODEBLOCK_SELECTION_THICKNESS_SUM = NODEBLOCK_SELECTION_THICKNESS * 2;
        static readonly Color SELECTION_COLOUR = new Color(.486f, .99f, 0, 0.5f);

        #endregion

        #region Variables
        Rect _rect;

        //Savable variables
        string _label;
        Color _blockColour;
        // public string ConnectedTowardsBlockNames { get; set; }
        // public string[] ConnectedTowardsBlockNames { get; set; } = new string[0];

        //Runtime
        public HashSet<String> ConnectedTowardsBlockNamesHashset { get; private set; } = new HashSet<string>();

        ///<Summary>The delegate assigned wil be called when the Connect button is pressed</Summary>
        CreateNewArrowConnectionLineCallback onConnect;

        #endregion

        #region Properties
        public SerializedProperty BlockProperty { get; private set; }
        // public string ID { get; private set; }
        public bool IsSelected { set; private get; }

        public string Label => _label;
        public Vector2 Position => _rect.position;

        //Rect properties
        public Vector2 Center => _rect.center;
        public Vector2 OutConnectionPoint => _rect.center + NODEBLOCK_ARROWCONNECTIONPOINT_OFFSET;
        public Vector2 InConnectionPoint => _rect.center - NODEBLOCK_ARROWCONNECTIONPOINT_OFFSET;

        public Color Colour => _blockColour;

        public int GetEffectCount => BlockProperty.FindPropertyRelative(Block.PROPERTYNAME_ORDERARRAY).arraySize;

        // //Runtime variables
        // ArrowConnectionLine _arrowLine;

        // ArrowConnectionLine ArrowLine
        // {
        //     get
        //     {
        //         _arrowLine = string.IsNullOrEmpty(_connectedTowardsBlockName) ? null : new ArrowConnectionLine();
        //         return _arrowLine;
        //     }
        //     set
        //     {
        //         _arrowLine = value;
        //     }
        // }
        #endregion




        #region Saving & Initialization

        // public BlockNode(SerializedProperty blockProperty, Vector2 position)
        // {
        //     BlockProperty = blockProperty;
        //     _rect = NODEBLOCK_SIZE;
        //     // ID = System.Guid.NewGuid().ToString();
        //     IsSelected = false;

        //     LoadFrom();
        //     _rect.position = position;
        // }

        public BlockNode(SerializedProperty blockProperty, CreateNewArrowConnectionLineCallback onConnect)
        {
            this.onConnect = onConnect;
            BlockProperty = blockProperty;
            _rect = NODEBLOCK_SIZE;
            // ID = System.Guid.NewGuid().ToString();
            IsSelected = false;

            LoadFrom();


        }

        ///<Summary>Creates a new arrow connection line instance if there is a block name in which this block is connected towards. For now it is called only after Loading of Cached Blocks </Summary>
        public void TryEstablishConnection()
        {
            foreach (var item in ConnectedTowardsBlockNamesHashset)
            {
                //Create a new connection line with the connected blockname if there is any
                if (!string.IsNullOrEmpty(item))
                {
                    // Debug.Log(item);
                    FlowChartWindowEditor.NodeManager_ArrowConnectionCycler_CreateNewArrowConnectionLine(this, item);
                }
            }

        }

        //   public BlockNode(SerializedProperty blockProperty,GUIStyle remove)
        // {
        //     BlockProperty = blockProperty;
        //     _rect = NODEBLOCK_SIZE;
        //     // ID = System.Guid.NewGuid().ToString();
        //     IsSelected = false;

        //     LoadFrom();
        // }

        // ///<Summary>Method which is used to reset node's value incase you do not want to create a new instance of BlockNode class</Summary>
        // public void SetBlockProperty(SerializedProperty blockProperty)
        // {
        //     BlockProperty = blockProperty;
        //     IsSelected = false;
        //     LoadFrom();
        // }

        //Loads the block's cached variables into this node 
        void LoadFrom()
        {
            _label = BlockProperty.FindPropertyRelative(Block.EDITOR_PROPERTYPATH_BLOCKNAME).stringValue;
            _blockColour = BlockProperty.FindPropertyRelative(Block.EDITOR_PROPERTYPATH_BLOCKCOLOUR).colorValue;
            _rect.position = BlockProperty.FindPropertyRelative(Block.EDITOR_PROPERTYPATH_BLOCKPOSITION).vector2Value;

            //Loading connection lines
            SerializedProperty connectedNamesProperty = BlockProperty.FindPropertyRelative(Block.EDITOR_PROPERTYPATH_CONNECTEDTOWARDS_BLOCKNAME);
            ConnectedTowardsBlockNamesHashset.Clear();
            for (int i = 0; i < connectedNamesProperty.arraySize; i++)
            {
                ConnectedTowardsBlockNamesHashset.Add(connectedNamesProperty.GetArrayElementAtIndex(i).stringValue);
            }

        }

        public void Save()
        {
            BlockProperty.serializedObject.Update();
            BlockProperty.FindPropertyRelative(Block.EDITOR_PROPERTYPATH_BLOCKNAME).stringValue = _label;
            BlockProperty.FindPropertyRelative(Block.EDITOR_PROPERTYPATH_BLOCKCOLOUR).colorValue = _blockColour;
            BlockProperty.FindPropertyRelative(Block.EDITOR_PROPERTYPATH_BLOCKPOSITION).vector2Value = _rect.position;

            //Saving Connection lines
            //Loading connection lines
            SerializedProperty connectedNamesProperty = BlockProperty.FindPropertyRelative(Block.EDITOR_PROPERTYPATH_CONNECTEDTOWARDS_BLOCKNAME);
            connectedNamesProperty.ClearArray();

            int index = 0;
            foreach (var item in ConnectedTowardsBlockNamesHashset)
            {
                connectedNamesProperty.InsertArrayElementAtIndex(index);
                connectedNamesProperty.GetArrayElementAtIndex(index).stringValue = item;
                index++;
            }

            // BlockProperty.FindPropertyRelative(Block.PROPERTYPATH_CONNECTEDTOWARDS_BLOCKNAME).stringValue = ConnectedTowardsBlockNames;

            BlockProperty.serializedObject.ApplyModifiedProperties();
        }

        public void ReloadNodeProperties()
        {
            _label = BlockProperty.FindPropertyRelative(Block.EDITOR_PROPERTYPATH_BLOCKNAME).stringValue;
            _blockColour = BlockProperty.FindPropertyRelative(Block.EDITOR_PROPERTYPATH_BLOCKCOLOUR).colorValue;

            // ConnectedTowardsBlockNames = BlockProperty.FindPropertyRelative(Block.PROPERTYPATH_CONNECTEDTOWARDS_BLOCKNAME).stringValue;
        }

        #endregion

        #region Window Functions
        public bool CheckIfClicked()
        {
            return _rect.Contains(Event.current.mousePosition, true);
        }

        public bool CheckRectOverlap(Rect selectionBox)
        {
            return _rect.Overlaps(selectionBox, true);
        }

        public void ProcessMouseDrag(Vector2 mouseDelta)
        {
            _rect.position += mouseDelta;
        }

        public void ProcessMouseUp()
        {
            BlockProperty.serializedObject.Update();
            BlockProperty.FindPropertyRelative(Block.EDITOR_PROPERTYPATH_BLOCKPOSITION).vector2Value = _rect.position;
            BlockProperty.serializedObject.ApplyModifiedProperties();
        }

        ///<Summary>Checks if there is a connection line which originates from this block towards the block with the connectedBlock label</Summary>
        public bool CheckConnectionTowards(string connectedBlock)
        {
            // bool connectionExists = false;

            // foreach (var item in ConnectedTowardsBlockNames)
            // {
            // connectionExists = item == connectedBlock;
            // }
            return ConnectedTowardsBlockNamesHashset.Contains(connectedBlock);
        }
        #endregion

        #region Drawing Functions
        // ///<Summary>Handles drawing the arrow lines if any </Summary>
        // public void DrawNodeArrowLines()
        // {
        //     if (ArrowLine != null)
        //     {
        //         ArrowLine.Draw(_rect.center, FlowChartWindowEditor.NodeManager_GetBlockNode(_connectedTowardsBlockName)._rect.center);
        //     }
        // }

        ///<Summary>Handles drawing the block background, the blockname label and a highlight background if block is selected </Summary>
        public void Draw_ToolBarState_NORMAL()
        {
            //=============== DRAW SELECTED HIGHLIGHT ==================
            if (IsSelected)
            {
                DrawHighLightedNode();
                return;
            }

            DrawUnHighLightedNode();
        }



        ///<Summary>If the blocknode is selected, it will draw as the same things as NORMAL mode. Else, it will draw a block background, blockname label and a button which will have the text "Connect" which when pressed will connect the currently selected node towards the node which button was pressed </Summary>
        public void Draw_ToolBarState_ARROW()
        {
            //=============== NODE IS SELECTED ==================
            if (IsSelected)
            {
                DrawHighLightedNode();
                return;
            }

            //======= NODE IS ALREADY CONNECTED FROM SELECTED NODE TOWARDS THIS NODE =================
            if (FlowChartWindowEditor.NodeManager_ArrowConnectionCycler_IsConnectedFromSelectedBlockNode(_label))
            {
                DrawUnHighLightedNode();
                return;
            }

            //============ DRAW BOX WITHOUT LABEL ===============
            Color prevColour = GUIExtensions.Start_GUI_ColourChange(_blockColour);
            GUI.Box(_rect, string.Empty, FlowChartWindowEditor.BlockNodeBoxStyle);
            GUIExtensions.End_GUI_ColourChange(prevColour);

            //Draw button that allows for connecting of node
            if (GUI.Button(_rect, NODEBLOCK_ARROWMODE_BUTTON_TEXT + _label, FlowChartWindowEditor.BlockNodeConnectButtonStyle))
            {
                onConnect?.Invoke(this);
            }

        }


        #region Base Functions
        void DrawHighLightedNode()
        {
            //Modify rect
            Rect rectCopy = _rect;
            rectCopy.width += NODEBLOCK_SELECTION_THICKNESS_SUM;
            rectCopy.height += NODEBLOCK_SELECTION_THICKNESS_SUM;
            rectCopy.x -= NODEBLOCK_SELECTION_THICKNESS;
            rectCopy.y -= NODEBLOCK_SELECTION_THICKNESS;

            Color prevColour = GUIExtensions.Start_GUI_ColourChange(SELECTION_COLOUR);
            GUI.Box(rectCopy, string.Empty, FlowChartWindowEditor.BlockNodeBoxStyle);
            GUIExtensions.End_GUI_ColourChange(prevColour);

            //============ DRAW BOX BG ===============
            prevColour = GUIExtensions.Start_GUI_ColourChange(_blockColour);
            GUI.Box(_rect, _label, FlowChartWindowEditor.BlockNodeBoxStyle);
            GUIExtensions.End_GUI_ColourChange(prevColour);
        }

        private void DrawUnHighLightedNode()
        {
            //============ DRAW BOX ===============
            Color prevColour = GUIExtensions.Start_GUI_ColourChange(_blockColour);
            GUI.Box(_rect, _label, FlowChartWindowEditor.BlockNodeBoxStyle);
            GUIExtensions.End_GUI_ColourChange(prevColour);
        }
        #endregion

        #endregion
    }

}