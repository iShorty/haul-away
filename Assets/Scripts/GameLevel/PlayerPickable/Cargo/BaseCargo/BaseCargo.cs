using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BaseFloaterGroup)), SelectionBase]
public partial class BaseCargo : PlayerPickable, IGrappleable, IBombable
{
    #region Exposed Fields
    [field: Header("===== CARGO INFO ====="), SerializeField, RenameField(nameof(CargoInfo))]
    ///<Summary>The cargo info which this cargo instance belongs to. Return to object pooler using this cargoinfo</Summary>
    public CargoInfo CargoInfo { get; protected set; } = default;


    #endregion

    // protected BaseMeshSilhoutte _silhoutteMesh = default;
    protected SinkTimerIndicator _sinkTimerIndicator = default;

    #region PlayerPickable Overrides
    public override PlayerPickableInfo PickableInfo => CargoInfo;

    protected override void Awake()
    {
        // _silhoutteMesh = GetComponentInChildren<BaseMeshSilhoutte>();
#if UNITY_EDITOR
        //Do checks 
        // Debug.Assert(_silhoutteMesh != null, $"The playerpickable {name} does not have BaseMeshSilhoutte assigned!", this);
        Debug.Assert(CompareTag(Constants.For_Layer_and_Tags.TAG_CARGO), $"The basecargo {name} does not have the Cargo tag on it!", this);
#endif
        // _silhoutteMesh.GameAwake();
        base.Awake();

    }

    // protected override void OnEnable()
    // {
    //     _silhoutteMesh.ToggleSilhoutte(BaseMeshSilhoutte.SilhouetteMode.OFF);
    //     base.OnEnable();
    // }

    protected override void OnDisable()
    {
        base.OnDisable();
        //Return sinktimer indicator to uiindicator pool if there is one
        if (_sinkTimerIndicator)
        {
            ReturnSinkIndicator();
        }
    }

    #region PropState
    // protected override void SetPropStateTo_KINEMATIC()
    // {
    //     _silhoutteMesh.ToggleSilhoutte(BaseMeshSilhoutte.SilhouetteMode.OFF);
    //     base.SetPropStateTo_KINEMATIC();
    // }

    //Toggle silhoutte when this cargo is either in floating or in water mode
    protected override void SetPropStateTo_FLOATING()
    {
        // _silhoutteMesh.ToggleSilhoutte(BaseMeshSilhoutte.SilhouetteMode.SEENONLY_WHENBLOCKED);
        _sinkTimerIndicator = UIIndicatorPool.GetIndicator(CargoInfo.SinkIndicatorInfo, PlayerManager.PlayerCanvas.transform, PropRigidBody) as SinkTimerIndicator;
        base.SetPropStateTo_FLOATING();
    }

    // protected override void SetPropStateTo_INWATER()
    // {
    //     _silhoutteMesh.ToggleSilhoutte(BaseMeshSilhoutte.SilhouetteMode.SEENONLY_WHENBLOCKED);
    //     base.SetPropStateTo_INWATER();
    // }

    protected override void SetPropStateTo_SINKING()
    {
        //Turn off silhoutte 
        // _silhoutteMesh.ToggleSilhoutte(BaseMeshSilhoutte.SilhouetteMode.OFF);
        ReturnSinkIndicator();
        base.SetPropStateTo_SINKING();
    }

    protected override bool Prop_GameUpdate_FLOATING()
    {
        //Update sinktimerindicator here
        _sinkTimerIndicator.SetImageFill(_propTimer / _floaterGroup.SinkInfo.FloatDuration);
        return base.Prop_GameUpdate_FLOATING();
    }

    #endregion

    #region Interactable Overrides
    //Dont toggle outline when not on boat
    public override void LeaveDetection()
    {
        if (CurrentPropState == PropState.ONLAND)
        {
            _outlinedMesh.ToggleOutline(BaseMeshOutline.OutlineMode.OFF);
            return;
        }

        // _silhoutteMesh.ChangeInfoColor(CargoInfo.CargoColors.NotValidColor);
    }


    //Dont toggle outline
    public override void EnterDetection()
    {
        if (CurrentPropState == PropState.ONLAND)
        {
            _outlinedMesh.ToggleOutline(BaseMeshOutline.OutlineMode.PARTIALLYHIDDEN);
            return;
        }

        // _silhoutteMesh.ChangeInfoColor(CargoInfo.CargoColors.ValidColor);
    }

    #endregion


    protected override void RegisterToUpdateLoop()
    {
        PlayerPickableManager.RegisterPlayerPickable(this);
    }

    protected override void OnSinkTimerUp()
    {
        ReturnToPool();
    }
    #endregion

    public void ReturnToPool()
    {
        BaseCargoPool.ReturnInstanceOf(this);   
    }


}
