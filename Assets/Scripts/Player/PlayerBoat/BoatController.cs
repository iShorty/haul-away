using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioManagement;
#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour, IBombable
{

    [Header("References")]
    // [SerializeField]
    public Rigidbody _rigidBody = default;

    [field: Header("===== RESPAWN POSITIONS =====")]
    [field: SerializeField, RenameField(nameof(PlayerRespawnPositions))]
    ///<Summary>The respawn positions of each player baesd on their player index (which is zeroindex based)</Summary>
    public Transform[] PlayerRespawnPositions { get; protected set; } = new Transform[0];


    [Header("Propulsion Point - Reference")]
    [SerializeField]
    Transform _propulsionPoint = default;

    [Header("Aim Position - Reference")]
    public Transform _aimPoint = default;

    [Header("====== WATER FLOATER ========")]
    [SerializeField]
    BaseFloaterGroup _floaterGroup = default;

    //----------------------------------------------
    // [Header("Speeds - Values")]
    // ///<Summary>When true, the boat is moving forward</Summary>
    // public bool _DirectionToggle = true;

    [Header("====== Fuel Speeds ========")]
    [SerializeField]
    [Range(0, 100)]
    float _rotationalSpeed = 5f;

    // [SerializeField]
    // [Range(0, 100)]
    // float _minlinearSpeed = 3f;
    // [SerializeField]
    // [Range(0, 100)]
    // float _maxlinearSpeed = 100f;

    [Header("====== Downwards Speeds ========")]
    [SerializeField, Range(0, 1000)]
    private int _downwardsMultiplier = 1;

    // [SerializeField]
    // [Range(0, 1f)]
    // float _reverseSpeedScalar = 0.5f;
    [Header("====== Bump Velocity Change ========")]
    [SerializeField]
    [Tooltip("The velocity applied when the boat bumps into a terrain")]
    [Range(0, 5f)]
    ///<Summary>The velocity applied when the boat bumps into a terrain</Summary>
    float velocityChangeValue = 1f;

    // [SerializeField, Tooltip("Fuel consumption rate")]
    // AnimationCurve _fuelConsumptionCurve = default;

    // [Header("====== Fuel Consumption Rate ========")]
    // [SerializeField]
    // [Range(0f, 1000f)]
    // float _minConsumeRate = 1f;
    // [SerializeField]
    // [Range(0f, 1000f)]
    // float _maxConsumeRate = 100f;
    [Header("====== Boat Acceleration ========")]
    [SerializeField]
    float _acceleration = 5f;

    [SerializeField]
    [Range(0f, 100f)]
    float _maxFuel = 100f;

    // [Header("Fuel - Values")]
    // [SerializeField]
    // Text _fuelTimerUI = default;

    [Header("SFX - Water Sound Freq")]
    [SerializeField] float _oceanSFXFrequency = 1f;

#if UNITY_EDITOR
    [Header("Debugging Settings")]
    [SerializeField]
    bool _enableBoatMove = false;

    [SerializeField]
    bool _enableAddFuel = default;

    private float _horizontalInput;
#endif

    #region ---------- Runtime ---------------
#if UNITY_EDITOR
    [SerializeField, ReadOnly, Header("----- Runtime -----")]
#endif
    float _fuelConsumptionRate = default;
#if UNITY_EDITOR
    [SerializeField, ReadOnly]
#endif
    float _targetSpeed = default;


    #endregion


    #region Hidden Fields
    float _metersMoved = default;
    float _boatDistanceCounter;


    // bool _engineActive = true;

    #endregion

    #region  Properties

    ///<Summary>
    /// How much time there is left before the fuel completely runs out
    ///</Summary>
    public float FuelAmount
    {
        get
        {
            return fuelAmount;
        }
        private set
        {
            fuelAmount = Mathf.Clamp(value, 0, _maxFuel);
        }
    }
    private float fuelAmount = 0;

    // public float LinearSpeed => _linearSpeed;
    float _linearSpeed = default;


    // float fuelConsumptionRate => Mathf.Clamp(_fuelConsumptionCurve.Evaluate(FuelAmount / _maxFuel) * _maxConsumeRate, _minConsumeRate, _maxConsumeRate);

    public List<GameObject> Obstacles { get; private set; } = new List<GameObject>();

    #endregion


    #region Awake/Start
    public void GameAwake()
    {
        _floaterGroup = GetComponent<BaseFloaterGroup>();
#if UNITY_EDITOR
        Debug.Assert(PlayerRespawnPositions.Length > 0, $"Boat manager has 0 respawn positions!", this);
#endif
    }

    //Handle global events (is called by manager cause we need the boat to pause before player pauses)
    public void GamePause()
    {
        _floaterGroup.Rigidbody.isKinematic = true;
    }
    public void GameResume()
    {
        _floaterGroup.Rigidbody.isKinematic = false;
    }


    #endregion

    #region -------------------- Enable  ----------------------
    private void OnEnable()
    {
        // _fuelTimerUI.text = $"Fuel: {FuelAmount.ToString("F0")}";
    }
    #endregion

    #region -------------------- Updates ------------------------
    public void GameUpdate()
    {
        _floaterGroup.GameUpdate();

#if UNITY_EDITOR
        #region ------------ Editor ------------------
        Keyboard keyboard = InputSystem.GetDevice<Keyboard>();
        if (keyboard.fKey.wasPressedThisFrame)
        {
            AddFuel(30f);
        }
        #endregion
#endif

        // if (_engineActive == false) return;

        UpdateLinearSpeed();
        TickFuelTimer();

#if UNITY_EDITOR
        #region ----------- Editor ---------------------
        if (_enableBoatMove)
        {
            bool leftKeyPressed = keyboard.leftArrowKey.isPressed;
            bool rightKeyPressed = keyboard.rightArrowKey.isPressed;

            if ((leftKeyPressed && rightKeyPressed) || (!leftKeyPressed && !rightKeyPressed))
            {
                _horizontalInput = 0;
                return;
            }

            if (leftKeyPressed)
            {
                _horizontalInput = -1;
                return;
            }

            if (rightKeyPressed)
            {
                _horizontalInput = 1;
                return;
            }
        }
        #endregion
#endif

    }

    public void GameFixedUpdate()
    {
        _floaterGroup.GameFixedUpdate();


        //============== PUSHING BOAT FORWARD WHEN FUEL > 0 ==================
        if (FuelAmount <= 0) return;
        // if (FuelAmount <= 0 || _engineActive == false) return;

#if UNITY_EDITOR
        AddTorqueToBoat(_horizontalInput);
#endif

        PropelBoat();

    }
    #endregion

    #region Public Method
    ///<Summary>
    /// Add the fuel's worth of time to the FuelTimer.
    ///</Summary>
    public void AddFuel(float addingFuelTime)
    {
        FuelAmount += addingFuelTime;
        // _fuelTimerUI.text = $"Fuel: {FuelAmount.ToString("F0")}";
    }

    ///<Summary>
    /// Add torque to the boat
    ///</Summary>
    public void AddTorqueToBoat(float direction)
    {
        if (direction != 0)
        {
            // _rigidBody.transform.Rotate(0, direction * Time.deltaTime * _rotationalSpeed, 0);
            // _rigidBody.angularVelocity += Vector3.up * direction * _rotationalSpeed * Time.fixedDeltaTime;
            _rigidBody.AddTorque(Vector3.up * direction * _rotationalSpeed, ForceMode.Acceleration);
        }
    }

    public void SetBoatMovement(float targetSpeed, float targetFuelConsumptionRate)
    {
        _fuelConsumptionRate = targetFuelConsumptionRate;
        _targetSpeed = targetSpeed;
        // _engineActive = isMovingForward;
    }

    #endregion


    void TickFuelTimer()
    {
        if (FuelAmount <= 0) return;

        //FuelAmount = FuelAmount - Time.deltaTime* _consumeRate
        FuelAmount -= Time.deltaTime * _fuelConsumptionRate;
        // _fuelTimerUI.text = $"Fuel: {FuelAmount.ToString("F0")}";
    }

    void PropelBoat()
    {
        float waveHeight = WaveManager.GetWaveHeight(_propulsionPoint.position.x);
        Vector3 finalDirection = transform.forward;

        finalDirection.y = 0;
        finalDirection = finalDirection.normalized;
        _rigidBody.AddForceAtPosition(finalDirection * _linearSpeed, transform.position, ForceMode.Acceleration);
        // _rigidBody.AddForce(transform.forward * _linearSpeed, ForceMode.Acceleration);
        OceanSFXCounter(_rigidBody.velocity);
    }

    void UpdateLinearSpeed()
    {
        _linearSpeed = Mathf.MoveTowards(_linearSpeed, _targetSpeed, _acceleration * Time.deltaTime);
        // _linearSpeed = Mathf.Clamp((FuelAmount / _maxFuel) * _maxlinearSpeed, _minlinearSpeed, _maxlinearSpeed) * (_DirectionToggle ? 1 : -_reverseSpeedScalar);
    }


    #region -------------- Collision --------------------

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == Constants.For_Layer_and_Tags.LAYERINDEX_TERRAIN || EnemyManager.IsEnemy(collision.collider))
        {
            // AudioManager.theAM.PlaySFX("Boat Impact");
            AudioEvents.RaiseOnPlay2DSFX(AudioClipType.SFX_BoatImpact, true);
            GameUtils.Bump(collision, transform, _rigidBody, velocityChangeValue);
        }
    }

    #endregion


    #region ------------------ IBombable ------------------

    // If exploded, explode.
    public virtual void BombBlast(float force, Vector3 bombPosition, float blastRadius, float upwardsModifier)
    {
        // force *= Constants.BOMB_INWATER_FORCE_DAMPENING;
        _rigidBody.AddExplosionForce(force, bombPosition, blastRadius, upwardsModifier);
    }

    #endregion

    void OceanSFXCounter(Vector3 velocity)
    {
        // footsteps sound
        if (_boatDistanceCounter >= 1f / _oceanSFXFrequency)
        {
            _boatDistanceCounter = 0f;
            // AudioManager.theAM.PlayOceanSFX();
            AudioEvents_Ocean.PlayOceanSFX();
        }
        _boatDistanceCounter += _linearSpeed * Time.deltaTime;
    }

}
