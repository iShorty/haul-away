namespace LinearEffects
{
    using UnityEngine;

    [System.Serializable]
    public abstract partial class BaseEffectExecutor : MonoBehaviour
    {
        ///<Summary>
        ///Returns true when effect has completed its execution. Also outs a bool which determines whether or not a block should wait a frame (or multiple frames in the case of an Update Effect Executor) before continuing calling the rest of the effects in the block which is below this effect. The block call system is not perfect, please read the comments at this method's code line to find out more.
        ///</Summary>
        public abstract bool ExecuteEffectAtIndex(int index, out bool haltCodeFlow);

        ///<Summary>
        ///Forcibly ends an effect's update call. EndExecuteEffect() will be called
        ///</Summary>
        public abstract void StopEffectUpdate(int index);



    }

}