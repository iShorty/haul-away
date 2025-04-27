using UnityEngine;

///<Summary>Just a scriptableobject which serializes a reference to itself so that you can click on the field and it will be pinged in the asset folder. Feel free to remove inheriting from this once your game project has come to optimization phase</Summary>
public abstract class BetterScriptableObject : ScriptableObject
{
#if UNITY_EDITOR
    [SerializeField]
    [ReadOnly]
    [Tooltip("Serializes a reference to itself so that you can click on the field and it will be pinged in the asset folder. This reference wont be compiled into the final build.")]
    ///<Summary>Serializes a reference to itself so that you can click on the field and it will be pinged in the asset folder. This reference wont be compiled into the final build.</Summary>
    ScriptableObject _this = default;

    ///<Summary>Toggle this to trigger the onvalidate function.</Summary>
    [Tooltip("Toggle this to trigger the onvalidate function.")]
    [SerializeField]
    protected bool _triggerOnValidate = default;

    
    protected virtual void OnValidate()
    {
        _this = this;
    }
#endif

}