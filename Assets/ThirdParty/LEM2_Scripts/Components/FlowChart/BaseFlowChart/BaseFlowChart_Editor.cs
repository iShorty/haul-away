#if UNITY_EDITOR
namespace LinearEffects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    public partial class BaseFlowChart : MonoBehaviour
    {

        protected partial class FlowChartSettings
        {
            [Tooltip("If this is set to true, all Exectuor components will be hidden in the inspector window")]
            [SerializeField]
            public bool HideExecutors = false;
        }

        public const string PROPERTYNAME_BLOCKARRAY = "_blocks";

        public Block Editor_GetBlock(string label)
        {
            int index = _blocks.FindIndex(x => x.BlockName == label);
            if (index == -1) return null;
            return _blocks[index];
        }


        #region ----------------- Hiding In Inspector -----------------------
        [SerializeField, HideInInspector]
        bool _prevHideOption = false;

        protected virtual void Editor_Awake()
        {
            if (_settings.HideExecutors)
                Editor_HideExecutors(_settings.HideExecutors);
        }


        protected virtual void OnValidate()
        {
            if (_prevHideOption == _settings.HideExecutors)
            {
                return;
            }

            _prevHideOption = _settings.HideExecutors;
            Editor_HideExecutors(_prevHideOption);
        }


        protected virtual void Reset()
        {
            Editor_ResetFlowChart();
        }

        protected virtual void OnDestroy()
        {
            Editor_HideExecutors(false);
        }


        private void Editor_HideExecutors(bool state)
        {
            HideFlags flag = state ? HideFlags.HideInInspector : HideFlags.None;
            BaseEffectExecutor[] hideExecutors = GetComponents<BaseEffectExecutor>();

            foreach (var item in hideExecutors)
            {
                item.hideFlags = flag;
            }
        }



        #endregion

        #region Editor Inspector Methods
        ///<Summary>Clears all blocks in the flowchart and removing all the Executors on the flowchart</Summary>
        public void Editor_ResetFlowChart()
        {
            _blocks = new Block[0];
            BaseEffectExecutor[] allExecutorsOnFlowChart = GetComponents<BaseEffectExecutor>();
            foreach (var item in allExecutorsOnFlowChart)
            {
                //Ignore the DestroyImmediate error
                DestroyImmediate(item);
            }
        }
        #endregion
    }

}
#endif