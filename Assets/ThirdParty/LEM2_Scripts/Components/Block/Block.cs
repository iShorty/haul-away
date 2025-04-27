namespace LinearEffects
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    //A block class will hold the order of the effects to be executed and then call
    //the respective effectexecutor to execute those effects
    [Serializable]
    public partial class Block
#if UNITY_EDITOR
    : ISavableData
#endif
    {
        #region Definitions
        [Serializable]
        public partial class BlockSettings
        {
            //======================= NODE PROPERTIES (ie properties which block node uses & saves) =========================
            public string BlockName;
        }


        #endregion

        #region Exposed Fields
        [Header("<== Click To Open ==>")]
        [SerializeField]
        BlockSettings _blockSettings;

        // ///<Summary>This array is the order in which you get your Data. For eg, let Data be a monobehaviour that stores cakes. By looping through OData, you are retrieving the cakes from the Holder class which is where the CakeData[] is being stored and serialized.</Summary>
        [SerializeField]
        protected EffectOrder[] _orderArray = new EffectOrder[0];

        #endregion

        #region Hidden Field
        int _scanFrontier = 0;
        bool _isBeingHalted = false;

        ///<Summary>The indices of all the updates being updated in a block</Summary>
        List<int> _updatingEffectIndices = new List<int>();
        #endregion

        #region Properties
        public string BlockName
        {
            get
            {
                return _blockSettings.BlockName;
            }
        }

        #endregion

        #region ----------- Public Methods -----------------
        #region Get Methods
        public int GetEffectDataElementIndex(int index)
        {
            return _orderArray[index].DataElementIndex;
        }

        public int GetLastEffectDataElementIndex()
        {
            return _orderArray[_orderArray.Length - 1].DataElementIndex;
        }
        #endregion

        #region Update Methods
        ///<Summary>Immediately ends the block updating cycle. Whatever effects which are still updating will have their EndExecuteEffect() called </Summary>
        public void EndBlockEffect()
        {
            for (int i = 0; i < _updatingEffectIndices.Count; i++)
            {
                int index = _updatingEffectIndices[i];
                EffectOrder effect = _orderArray[index];
                effect.EndEffect();
            }
            _updatingEffectIndices.Clear();

            //Reset the scan frontier
            _scanFrontier = 0;
        }

        ///<Summary>Runs all of the effect code on the block by sequentially going down the Effect Order array. Returns true when all of the block's effects have been fully finished. This should be called inside an update loop if effect requires more than one frame to carry out. </Summary>
        public bool ExecuteBlockEffects()
        {
            bool allScannedThrough = ScanBlockEffects();
            //Reupdates the halted status
            bool allEffectsFinished = ExecuteUpdateEffects(out _isBeingHalted);
            //So long as there is no halt in code flow, scan the block effects
            if (!_isBeingHalted)
            {
                //If allEffects are updated finished and all block effects are scanned,
                if (allScannedThrough && allEffectsFinished)
                {
                    //Reset update variables
                    _scanFrontier = 0;
                    _updatingEffectIndices.Clear();
                    return true;
                }

                //Else if updating allEffects are not finished,  return false
                //Else if scanning returns false and allEffectsFinished is true, return false
                return false;
            }

            //Else if there is halt in the code flow,
            //Dont scan just return false cause we havent completely scanned finish yet
            return false;

        }

        ///<Summary>Updates all updatable effects. Returns true if all of the updating effects are done updating. Outs a bool to determine if there is still a halt code flow boolean</Summary>
        private bool ExecuteUpdateEffects(out bool isThereHaltCodeFlow)
        {
            bool isAllEffectsUpdated = true;
            isThereHaltCodeFlow = false;

            for (int i = 0; i < _updatingEffectIndices.Count; i++)
            {
                int index = _updatingEffectIndices[i];
                EffectOrder effect = _orderArray[index];

                //Update the updateable effects
                if (!effect.CallEffect(out isThereHaltCodeFlow))
                {
                    //---------- Effect has not been finished ------------
                    isAllEffectsUpdated = false;
                    continue;
                }

                //---------- Effect has finished ------------
                _updatingEffectIndices.RemoveAt(i);

                //If there is still effects needing to update,
                if (_updatingEffectIndices.Count > 0)
                {
                    //Else just keep looping
                    i--;
                    continue;
                }

                //Else if this the previously removed index was the last one
                //Since this effect is the one stopping block flow, set it to be false cause its getting removed
                isThereHaltCodeFlow = false;
                return isAllEffectsUpdated;
            }

            return isAllEffectsUpdated;
        }

        ///<Summary>Loop through the orderArray starting from the _scanFrontier index. Call the effects looped through and check if it is an update effect. If so, add it into the UpdateEffectIndices list so that it could be updated everyframe from the next frame onwards.Also, check if the update effect has HaltUntilFinished boolean checked. If so, stop scanning and set the _scanFrontier as the current index. Returns true if all the effects are scanned through and have been called once.</Summary>
        private bool ScanBlockEffects()
        {
            //Dont scan when block code is being halted
            if (_isBeingHalted)
            {
                return false;
            }

            //Start from the frontier
            for (int i = _scanFrontier; i < _orderArray.Length; i++)
            {
                EffectOrder effect = _orderArray[i];

                //If the returned value is false, that means that this effect is an update effect else it will be an instant effect
                if (effect.CallEffect(out _isBeingHalted))
                {
                    continue;
                }

                //Add this effect to the update list 
                _updatingEffectIndices.Add(i);

                //if this update effect isnt halting code flow for scanning, continue
                if (!_isBeingHalted)
                {
                    continue;
                }

                //Stop scanning if code flow is being halted and save where scan left off
                _scanFrontier = i + 1;
                return false;

            }

            //Set this to the orderArrayLength to prevent re-scanning 
            _scanFrontier = _orderArray.Length;
            return true;
        }


        #endregion
    }

    #endregion


}