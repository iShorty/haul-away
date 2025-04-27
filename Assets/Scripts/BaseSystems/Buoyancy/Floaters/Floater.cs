using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    #region Constants
    // // public const float ABSOLUTE_GRAVITY = 9.81f;
    // public static readonly float ABSOLUTE_GRAVITY = Mathf.Abs(Physics.gravity.y);

    public const float DISPLACEMENTCHANGE_APPROXIMATION = 0.1f;
    #endregion

    //TURN THIS INTO SCRIPTABLE OBJECTS LATER ON 
    #region Exposed Variables
    // #if UNITY_EDITOR
    //     [Header("Settings")]
    //     [SerializeField]
    //     [Tooltip("Should the FloaterInfo be inherited from the parent?")]
    // #endif
    //     bool _inheritFromParent = true;


    // [Tooltip("The Scriptable Object that holds the information on how a floater behaves in water")]
    // [SerializeField]
    // #if UNITY_EDITOR
    //     [ConditionalReadOnly(nameof(_inheritFromParent), false, true)]
    // #endif


    #endregion

    #region Hidden Fields
    [SerializeField, HideInInspector]
    int _numberOfSharedFloaters = 0;

    #endregion

    #region Properties
    Rigidbody rb => _parentFloaterGroup.Rigidbody;
    FloaterInformation FloatersInfo => _parentFloaterGroup.FloatersInfo;
    FloaterSinkInfo SinkInfo => _parentFloaterGroup.SinkInfo;

    #endregion

    #region Runtime Variables
    BaseFloaterGroup _parentFloaterGroup = default;

    //Use this to simulate sinking
#if UNITY_EDITOR
    [Header("===== RUNTIME =====")]
    [SerializeField, ReadOnly]
#endif
    float _displacementAmount = default;

#if UNITY_EDITOR
    [SerializeField, ReadOnly]
#endif
    float _sinkSpeed = default;
    #endregion

    // #if UNITY_EDITOR
    //     public void EditorValidate(FloaterInformation info, int numberOfSharedFloaters, Rigidbody parentRb)
    //     {
    //         _rb = parentRb;
    //         _numberOfSharedFloaters = numberOfSharedFloaters;

    //         if (_inheritFromParent)
    //             _info = info;
    //     }
    // #endif

    public void GameAwake(BaseFloaterGroup parent, int numberOfSharedFloaters)
    {
        _parentFloaterGroup = parent;
        _numberOfSharedFloaters = numberOfSharedFloaters;

        // if (_inheritFromParent)
        // _info = info;
    }

    public void GameEnable()
    {
        _sinkSpeed = Random.Range(SinkInfo.SinkSpeedRange.x, SinkInfo.SinkSpeedRange.y);
        _displacementAmount = FloatersInfo.DisplacementAmount;
    }


    public void GameFixedUpdate()
    {
        //=============== FAKE GRAVITY ===================
        // F = Mass x Acceleration
        //Simulate the pull of gravity downwards at the floater's positions instead of having the boat's rb to experience gravity at the center of the boat (which is stupid cause the corners of the boat needs to be pulled downards to prevvent the boat from being easily flipper)
        rb.AddForceAtPosition(Physics.gravity / _numberOfSharedFloaters, transform.position, ForceMode.Acceleration);

        //============== BUOYANCY CALCULATIONS ===================
        float currWaveHeight = WaveManager.GetWaveHeight(transform.position.x);

        if (transform.position.y < currWaveHeight)
        {
            // Buoyancy Force = Density x Gravity x Volume of Fluid displaced
            //V = AREA X HEIGHT
            //What we are doing here is finding the current object's height, dividing by the depth of water b4 item is considered completely submerged to get a percentage of how submerged the object is. After getting the height, we multiply by the total volume of the object to get the approx vol of fluid displaced
            float approxDisplacedVolume = Mathf.Clamp01((currWaveHeight - transform.position.y) / FloatersInfo.DepthBeforeSubmerged) * _displacementAmount;
            //======================= APPLYING BUOYANCY FORCE ==================
            //then we slot the fluid displaced vol into the boyancy force formula (density of water is assume to be 1 )
            rb.AddForceAtPosition(FloatersInfo.BuoyancyDampening * Vector3.up * approxDisplacedVolume * Constants.ABSOLUTE_GRAVITY, transform.position, ForceMode.Acceleration);

            //Apply dampening to all the addforce/torque functiosn below 
            approxDisplacedVolume *= FloatersInfo.DragDampening;

            //======================= APPLYING WATER DRAG FORCE ==================
            //Fdrag = (-1/2) x Density of Fluid x Velocity Magnitutude Of Moving Object ^2 x Area of moving object intercepting fluid x Drag Coefficient x Normalized Velocity Vector of Moving Object
            //we use velocity changemode here because mass is not in the equation
            rb.AddForce(-rb.velocity * FloatersInfo.VelocityDrag * approxDisplacedVolume * Time.fixedDeltaTime, ForceMode.VelocityChange);
            rb.AddTorque(-rb.angularVelocity * FloatersInfo.AngularVelocityDrag * approxDisplacedVolume * Time.fixedDeltaTime, ForceMode.VelocityChange);

        }
    }

    #region Float/Sink Behaviour
    ///<Summary>
    ///Returns true when floater's displacement amount reached 0
    ///</Summary>
    public bool GameUpdate_Sink()
    {
        bool consideredAsSunk = (0 + _displacementAmount) <= DISPLACEMENTCHANGE_APPROXIMATION;
        if (consideredAsSunk)
        {
            _displacementAmount = 0;
            return true;
        }
        _displacementAmount = Mathf.MoveTowards(_displacementAmount, 0, Time.deltaTime * _sinkSpeed);
        return false;
    }

    ///<Summary>
    ///Returns true when floater's displacement amount reached the floater's info DisplacementAmount
    ///</Summary>
    public bool GameUpdate_Rise()
    {
        bool consideredAsRisen = (FloatersInfo.DisplacementAmount - _displacementAmount) <= DISPLACEMENTCHANGE_APPROXIMATION;
        if (consideredAsRisen)
        {
            _displacementAmount = FloatersInfo.DisplacementAmount;
            return true;
        }

        _displacementAmount = Mathf.MoveTowards(_displacementAmount, FloatersInfo.DisplacementAmount, Time.deltaTime * _sinkSpeed);
        return false;
    }
    #endregion

}
