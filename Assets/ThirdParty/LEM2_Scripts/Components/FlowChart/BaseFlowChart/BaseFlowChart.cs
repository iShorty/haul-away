namespace LinearEffects
{
    using System.Collections.Generic;
    using System;
    using UnityEngine;

    //Flow chart is the class which holds/creates blocks
    // which can be called at different times (ie when game starts, game ends , )
#if UNITY_EDITOR
    [DisallowMultipleComponent,ExecuteInEditMode]
#endif
    public partial class BaseFlowChart : MonoBehaviour
    {
        #region ------------ Definitions -------------
        [System.Serializable]
        //Incase i ever want to add stuff to the flowchart in the future
        protected partial class FlowChartSettings { }

        #endregion

        #region ---------- Exposed Field ---------------
        [SerializeField]
        protected Block[] _blocks = new Block[0];

        [Header("Settings")]
        [SerializeField]
        protected FlowChartSettings _settings = new FlowChartSettings();
        #endregion

        #region ----------- Properties ---------------
        ///<summary>Checks if this flowchart is playing any block effects</summary>
        public bool IsPlaying => _activeBlockList.Count > 0;
        #endregion

        #region ------------- Runtime Field ---------------
        ///<summary>A dictionary which holds all of the blocks on this flowchart. The key is the Block's Name and the value is the respective block </summary>
        protected Dictionary<string, Block> _blockDictionary = default;

        ///<summary>A hashset which holds all of the blocks on this flowchart has been called to execute its effects.</summary>
        protected HashSet<Block> _activeBlockHashset = default;

        ///<summary>A list which holds the blocks that will be called every frame (and removed when its effects are fully exectuted) <summary>
        protected List<Block> _activeBlockList = default;

        #endregion

        #region ================== Public Methods ===========================

        #region --------------------- GetBlock --------------------------------
        ///<Summary>Get the block via index in the block array</Summary>
        public Block GetBlock(int index)
        {
            return _blocks[index];
        }

        ///<Summary>Get the block via block name in the block dictionary</Summary>
        public Block GetBlock(string blockName)
        {
            return _blockDictionary[blockName];
        }
        #endregion

        #region --------------------- PlayBlock ---------------------
        ///<Summary>Plays a block via block index</Summary>
        public void PlayBlock(int index)
        {
            Block intendedToPlayBlock = _blocks[index];
            PlayBlock(intendedToPlayBlock);
        }

        ///<Summary>Plays a block via block name</Summary>
        public void PlayBlock(string blockName)
        {
#if UNITY_EDITOR
            Debug.Assert(_blockDictionary.ContainsKey(blockName), $"The flowchart {name} does not have the blockname {blockName}", this);
#endif
            Block intendedToPlayBlock = _blockDictionary[blockName];
            PlayBlock(intendedToPlayBlock);
        }

        ///<Summary>Plays a block via block instance</Summary>
        public void PlayBlock(Block block)
        {
            if (_activeBlockHashset.Contains(block))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"The block {block.BlockName} on the flowchart {name} is already playing!", this);
#endif
                return;
            }

            //Plays the block. If the block does not need multiple frames to carryout, they will not be addedd to the update loop
            if (!block.ExecuteBlockEffects())
            {
                _activeBlockList.Add(block);
                _activeBlockHashset.Add(block);
            }
        }
        #endregion

        #region --------------------- StopBlock Methods ---------------------
        public void TryStopBlock(string blockName)
        {
            if (!CheckBlockIsPlaying(blockName)) return;
            Block block = GetBlock(blockName);
            block.EndBlockEffect();
            _activeBlockHashset.Remove(block);
            _activeBlockList.Remove(block);
        }
        #endregion

        #region --------------------- Get Effect ---------------------
        public EffectType GetEffect<ExecutorType, EffectType>(string blockName, int effectIndex)
        where ExecutorType : EffectExecutor<EffectType>
        where EffectType : Effect, new()
        {
            Block block = GetBlock(blockName);
            int dataElmtIndex = block.GetEffectDataElementIndex(effectIndex);

            ExecutorType executor = GetComponent<ExecutorType>();

            EffectType effectData = executor.GetEffectData(dataElmtIndex);
            return effectData;
        }

        public EffectType GetLastEffect<ExecutorType, EffectType>(string blockName)
      where ExecutorType : EffectExecutor<EffectType>
      where EffectType : Effect, new()
        {
            Block block = GetBlock(blockName);
            int dataElmtIndex = block.GetLastEffectDataElementIndex();

            ExecutorType executor = GetComponent<ExecutorType>();

            EffectType effectData = executor.GetEffectData(dataElmtIndex);
            return effectData;
        }
        #endregion

        #region --------------------- Checks ---------------------
        ///<Summary>Checks if a block is playing or not. Returns true if block is playing</Summary>
        public bool CheckBlockIsPlaying(int index)
        {
            Block block = _blocks[index];
            return _activeBlockHashset.Contains(block);
        }

        ///<Summary>Checks if a block is playing or not. Returns true if block is playing</Summary>
        public bool CheckBlockIsPlaying(string blockName)
        {
#if UNITY_EDITOR
            Debug.Assert(_blockDictionary.ContainsKey(blockName), $"The flowchart {name} does not have the blockname {blockName}", this);
#endif
            Block block = _blockDictionary[blockName];
            return _activeBlockHashset.Contains(block);
        }
        #endregion

        #endregion


        #region ------------------ Life Cycle ------------------------------------------
        ///<Summary>Proxy Awake method call. Initializes the things needed for a flowchart to work. If you want an already established script, use FlowChart.cs instead</Summary>
        public void GameAwake()
        {
            _blockDictionary = new Dictionary<string, Block>();
            foreach (var block in _blocks)
            {
                _blockDictionary.Add(block.BlockName, block);
            }
            _activeBlockHashset = new HashSet<Block>();
            _activeBlockList = new List<Block>();
        }


        ///<Summary>Proxy Update method call. It will update the blocks' effects (where some of the effects ought to be called over multiple frames)</Summary>
        public void GameUpdate()
        {
            for (int i = 0; i < _activeBlockList.Count; i++)
            {
                Block block = _activeBlockList[i];
                if (block.ExecuteBlockEffects())
                {
                    _activeBlockHashset.Remove(block);
                    _activeBlockList.RemoveEfficiently(i);
                    i--;
                }
            }

        }


        #endregion

    }
}

