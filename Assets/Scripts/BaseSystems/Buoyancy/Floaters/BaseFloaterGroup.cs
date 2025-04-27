using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class doesnt have unity's fixedupdate method and hence will not be useful in prototyping but good for controlled fixedupdate calls
[RequireComponent(typeof(Rigidbody))]
public class BaseFloaterGroup : MonoBehaviour
{
    #region Definiton
    public enum State { NONE, SINKING, RISING }

    #endregion

    [field: SerializeField,
#if UNITY_EDITOR
RenameField(nameof(FloatersInfo))
#endif
]
    public FloaterInformation FloatersInfo { get; protected set; } = default;

    [field: SerializeField,
#if UNITY_EDITOR
RenameField(nameof(SinkInfo))
#endif
]

    public FloaterSinkInfo SinkInfo { get; protected set; } = default;

    // #if UNITY_EDITOR
    //     protected virtual void OnValidate()
    //     {
    //         Rigidbody = GetComponent<Rigidbody>();
    //         _floaters = GetComponentsInChildren<Floater>();
    //         for (int i = 0; i < _floaters.Length; i++)
    //         {
    //             _floaters[i].EditorValidate(_floatersInfo, _floaters.Length, Rigidbody);
    //         }
    //     }
    // #endif


    // [field: SerializeField, HideInInspector]

    public Rigidbody Rigidbody { get; protected set; } = null;
    // [SerializeField, HideInInspector]
    Floater[] _floaters = new Floater[0];

#if UNITY_EDITOR
    [Header("===== RUNTIME =====")]
    [ReadOnly, SerializeField]
#endif
    State _currentState = State.NONE;


    protected virtual void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        _floaters = GetComponentsInChildren<Floater>();
        foreach (var item in _floaters)
        {
            item.GameAwake(this, _floaters.Length);
        }
    }


    protected virtual void OnEnable()
    {
        _currentState = State.NONE;

        for (int i = 0; i < _floaters.Length; i++)
        {
            _floaters[i].GameEnable();
        }

        // if(GetComponent<OctopusMovement>())
        //     Debug.Log("BaseFloaterGroup OnEnable " + gameObject.name, gameObject);
        Rigidbody.useGravity = false;
    }

    protected virtual void OnDisable()
    {
        // if(GetComponent<OctopusMovement>())
        //     Debug.Log("BaseFloaterGroup OnDisable " + gameObject.name, gameObject);
        Rigidbody.useGravity = true;
    }



    public void GameFixedUpdate()
    {
        for (int i = 0; i < _floaters.Length; i++)
        {
            _floaters[i].GameFixedUpdate();
        }
    }

    #region Update
    ///<Summary>
    ///Returns true when floater group has reached NONE state
    ///</Summary>
    public bool GameUpdate()
    {
        switch (_currentState)
        {

            case State.SINKING:
                return Update_SINK();

            case State.RISING:
                return Update_RISING();

            case State.NONE: return true;
            default:
#if UNITY_EDITOR
                Debug.LogError("Code should not flow here!");
#endif
                return true;
        }

    }

    ///<Summary>
    ///Returns true when floater group has reached NONE state
    ///</Summary>
    bool Update_SINK()
    {
        bool allTrue = true;
        for (int i = 0; i < _floaters.Length; i++)
        {
            allTrue = _floaters[i].GameUpdate_Sink();
        }

        if (allTrue)
        {
            _currentState = State.NONE;
        }
        return allTrue;
    }

    ///<Summary>
    ///Returns true when floater group has reached NONE state
    ///</Summary>
    bool Update_RISING()
    {
        bool allTrue = true;
        for (int i = 0; i < _floaters.Length; i++)
        {
            allTrue = _floaters[i].GameUpdate_Rise();
        }

        if (allTrue)
        {
            _currentState = State.NONE;
        }
        return allTrue;
    }
    #endregion

    #region State
    public void StartSinking() => SetState(State.SINKING);
    public void StartRising() => SetState(State.RISING);

    void SetState(State state)
    {
        _currentState = state;
    }
    #endregion


}
