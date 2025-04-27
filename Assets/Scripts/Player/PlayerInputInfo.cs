using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInput", menuName = Constants.ASSETMENU_CATEGORY_PLAYER + "/PlayerInputInfo")]
public class PlayerInputInfo : ScriptableObject
{
    [field: Header("===== JOYSTICKS ====="), SerializeField, RenameField(nameof(VerticalAxis))]
    public string VerticalAxis { get; private set; } = "PlayerX_Vertical";

    [field: RenameField(nameof(HorizontalAxis)), SerializeField]
    public string HorizontalAxis { get; private set; } = "PlayerX_Horizontal";

    [field: Header("===== BUTTONS ====="), RenameField(nameof(SprintAxis)), SerializeField]
    public string SprintAxis { get; private set; } = "PlayerX_Shift";

    [field: RenameField(nameof(UseAxis)), SerializeField]
    public string UseAxis { get; private set; } = "PlayerX_Use";

    [field: RenameField(nameof(LeaveAxis)), SerializeField]
    public string LeaveAxis { get; private set; } = "PlayerX_Leave";

    [field: RenameField(nameof(TossAxis)), SerializeField]
    public string TossAxis { get; private set; } = "PlayerX_Toss";

    [field: RenameField(nameof(ToggleLeftAxis)), SerializeField]
    public string ToggleLeftAxis { get; private set; } = "PlayerX_ToggleLeft";

    [field: RenameField(nameof(ToggleRightAxis)), SerializeField]
    public string ToggleRightAxis { get; private set; } = "PlayerX_ToggleRight";

#if UNITY_EDITOR
    //============ DEBUG ================
    [field: RenameField(nameof(UseDebugAxis)), Header("===== DEBUG ZONE ====="), SerializeField, Space(20f)]
    public bool UseDebugAxis { get; private set; } = false;

    [field: RenameField(nameof(DebugVerticalAxis)), SerializeField, Header("----- JOYSTICKS -----")]
    public string DebugVerticalAxis { get; private set; } = "Debug_PlayerX_Vertical";

    [field: RenameField(nameof(DebugHorizontalAxis)), SerializeField]
    public string DebugHorizontalAxis { get; private set; } = "Debug_PlayerX_Horizontal";

    [field: RenameField(nameof(DebugSprintAxis)), SerializeField, Header("----- BUTTONS -----")]
    public string DebugSprintAxis { get; private set; } = "Debug_PlayerX_Shift";

    [field: RenameField(nameof(DebugUseAxis)), SerializeField]
    public string DebugUseAxis { get; private set; } = "Debug_PlayerX_Use";

    [field: RenameField(nameof(DebugLeaveAxis)), SerializeField]
    public string DebugLeaveAxis { get; private set; } = "Debug_PlayerX_Leave";

    [field: RenameField(nameof(DebugTossAxis)), SerializeField]
    public string DebugTossAxis { get; private set; } = "Debug_PlayerX_Toss";

    [field: RenameField(nameof(DebugToggleLeftAxis)), SerializeField]
    public string DebugToggleLeftAxis { get; private set; } = "Debug_PlayerX_ToggleLeft";

    [field: RenameField(nameof(DebugToggleRightAxis)), SerializeField]
    public string DebugToggleRightAxis { get; private set; } = "Debug_PlayerX_ToggleRight";
#endif

    // [field: Header("===== XBOX JOYSTICKS ====="), SerializeField, RenameField(nameof(Xbox_VerticalAxis))]
    // public string Xbox_VerticalAxis { get; private set; } = "Xbox_PlayerX_Vertical";

    // [field: RenameField(nameof(Xbox_HorizontalAxis)), SerializeField]
    // public string Xbox_HorizontalAxis { get; private set; } = "Xbox_PlayerX_Horizontal";

    // [field: Header("===== XBOX BUTTONS ====="), RenameField(nameof(Xbox_SprintAxis)), SerializeField]
    // public string Xbox_SprintAxis { get; private set; } = "Xbox_PlayerX_Shift";

    // [field: RenameField(nameof(Xbox_UseAxis)), SerializeField]
    // public string Xbox_UseAxis { get; private set; } = "Xbox_PlayerX_Use";

    // [field: RenameField(nameof(Xbox_LeaveAxis)), SerializeField]
    // public string Xbox_LeaveAxis { get; private set; } = "Xbox_PlayerX_Leave";

    // [field: RenameField(nameof(Xbox_TossAxis)), SerializeField]
    // public string Xbox_TossAxis { get; private set; } = "Xbox_PlayerX_Toss";

    // [field: RenameField(nameof(Xbox_ToggleLeftAxis)), SerializeField]
    // public string Xbox_ToggleLeftAxis { get; private set; } = "Xbox_PlayerX_ToggleLeft";

    // [field: RenameField(nameof(Xbox_ToggleRightAxis)), SerializeField]
    // public string Xbox_ToggleRightAxis { get; private set; } = "Xbox_PlayerX_ToggleRight";



    public Vector2 GetMovementInput()
    {
        Vector2 input;
        input.x = GetMovementXInput();
        input.y = GetMovementYInput();
        return input;
    }
    
//     public float GetMovementXInput() => GetFloatInput(Xbox_HorizontalAxis
// #if UNITY_EDITOR
//     , DebugHorizontalAxis
// #endif
//     );
//     public float GetMovementYInput() => GetFloatInput(Xbox_VerticalAxis
// #if UNITY_EDITOR
//     , DebugVerticalAxis
// #endif

//     );

//     #region Button Inputs

//     public bool GetUseInput() => GetBoolInput(Xbox_UseAxis
// #if UNITY_EDITOR
//     , DebugUseAxis
// #endif

//     );
//     public bool GetTossInput() => GetBoolInput(Xbox_TossAxis
// #if UNITY_EDITOR
//     , DebugTossAxis
// #endif
//     );
//     public bool GetLeaveInput() => GetBoolInput(Xbox_LeaveAxis
// #if UNITY_EDITOR
//     , DebugLeaveAxis
// #endif
//     );

//     public bool GetToggleLeftInput() => GetBoolInput(Xbox_ToggleLeftAxis
// #if UNITY_EDITOR
//     , DebugToggleLeftAxis
// #endif
//     );

//     public bool GetToggleRightInput() => GetBoolInput(Xbox_ToggleRightAxis
// #if UNITY_EDITOR
//     , DebugToggleRightAxis
// #endif
//     );



    public float GetMovementXInput() => GetFloatInput(HorizontalAxis
#if UNITY_EDITOR
    , DebugHorizontalAxis
#endif
    );
    public float GetMovementYInput() => GetFloatInput(VerticalAxis
#if UNITY_EDITOR
    , DebugVerticalAxis
#endif

    );

    #region Button Inputs

    public bool GetUseInput() => GetBoolInput(UseAxis
#if UNITY_EDITOR
    , DebugUseAxis
#endif

    );
    public bool GetTossInput() => GetBoolInput(TossAxis
#if UNITY_EDITOR
    , DebugTossAxis
#endif
    );
    public bool GetLeaveInput() => GetBoolInput(LeaveAxis
#if UNITY_EDITOR
    , DebugLeaveAxis
#endif
    );

    public bool GetToggleLeftInput() => GetBoolInput(ToggleLeftAxis
#if UNITY_EDITOR
    , DebugToggleLeftAxis
#endif
    );

    public bool GetToggleRightInput() => GetBoolInput(ToggleRightAxis
#if UNITY_EDITOR
    , DebugToggleRightAxis
#endif
    );



    #endregion



    #region Base Get Inputs (Dont delete the NonEditor Overloads)
#if UNITY_EDITOR
    float GetFloatInput(string axis, string debugAxis)
    {
        float f = Input.GetAxisRaw(axis);

        if (!UseDebugAxis) return f;

        f = Mathf.Approximately(f, 0f) ? Input.GetAxisRaw(debugAxis) : f;
        return f;
    }
#endif

    float GetFloatInput(string axis)
    {
        float f = Input.GetAxisRaw(axis);
        return f;
    }


#if UNITY_EDITOR
    bool GetBoolInput(string axis, string debugAxis)
    {
        bool desire = Input.GetButtonDown(axis);
        if (!UseDebugAxis)
        {
            return desire;
        }
        //========= DEBUG =============
        desire = desire == false ? Input.GetButtonDown(debugAxis) : desire;
        return desire;
    }
#endif

    bool GetBoolInput(string axis)
    {
        bool desire = Input.GetButtonDown(axis);
        return desire;
    }
    #endregion

}
