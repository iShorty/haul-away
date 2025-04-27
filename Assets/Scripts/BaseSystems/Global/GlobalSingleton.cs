using UnityEngine;

public class GlobalSingleton : MonoBehaviour
{
    public static GlobalSingleton Instance { get; private set; } = null;

    protected virtual void Awake()
    {
        //If instance of singleton is present when this instance is called onawake, that means that this instance is a copy
        if (Instance != null)
        {
            //Bye bye
            //Used DestroyImmediate to make sure that the duplicate singleton does not allow its child scripts to call their "Awake" and "OnDestroy" unity functions
            //A way to combat this is to have a scene (cold scene) to dedicate loading the global singleton on Application start. Then remove all the other instances of global singletons in other scenes.
            DestroyImmediate (gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }
}
