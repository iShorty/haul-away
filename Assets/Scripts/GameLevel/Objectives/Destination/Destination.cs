using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public partial class Destination : MonoBehaviour
{
    [SerializeField]
    IndicatorInfo _destinationInfo = default;

    [SerializeField]
    CargoInfo _preferredCargoInfo = default;



    Rigidbody destinationRb => GetComponent<Rigidbody>();

    #region Awake OnEnable OnDisable
#if UNITY_EDITOR
    private void Awake()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        Collider c = GetComponent<Collider>();
        Debug.Assert(c != null, $"Destination {name} does not have a collider attached!", this);
        Debug.Assert(c.isTrigger == true, $"Destination {name} does not have its collider set to istrigger true!", this);
        Debug.Assert(gameObject.layer == Constants.For_Layer_and_Tags.LAYERINDEX_TERRAIN, $"Destination instance is not using the layer index for {Constants.For_Layer_and_Tags.LAYERNAME_TERRAIN}!", this);
        Debug.Assert(rigidbody, $"There must be a rigidbody on this script's object!", this);
        Debug.Assert(rigidbody.isKinematic, $"{name}'s rigidbody must have its isKinematic true!", this);
        // Debug.Assert(_preferredCargoInfo != null, $"Destination {name} is missing its preferred cargo info!", this);

        //Add this instance of destination to the environment manager
        // EnvironmentManager.AddDestination(this);
    }
#endif

    private void OnEnable()
    {
        // MasterGameManager.OnGameEnd += HandleGameEnd;
        GlobalEvents.OnGameStart += HandleGameStart;
    }

    private void OnDisable()
    {
        // MasterGameManager.OnGameEnd -= HandleGameEnd;
        GlobalEvents.OnGameStart -= HandleGameStart;
    }
    #endregion

    #region GameEvent Handlers
    //Hide or enable collider when game ends or starts
    private void HandleGameEnd(int finalScore)
    {
        GetComponent<Collider>().enabled = false;
        UIIndicatorPool.TryRemoveIndicator(destinationRb);
    }

    void HandleGameStart()
    {
        GetComponent<Collider>().enabled = true;
        //Get instance of destination ui indicator
        DestinationIndicator indicator = UIIndicatorPool.GetIndicator(_destinationInfo, PlayerManager.PlayerCanvas.transform, destinationRb) as DestinationIndicator;
        indicator.SetPreferredCargoSprite(_preferredCargoInfo.CargoSprite);
    }
    #endregion


    #region Trigger Methods
    private void OnTriggerEnter(Collider other)
    {
        EvaluateTrigger(other);
    }

    // private void OnTriggerStay(Collider other)
    // {
    //     EvaluateTrigger(other);
    // }

    private void EvaluateTrigger(Collider other)
    {
        //Only accept cargoobjects
        if (!PlayerPickableManager.IsCargo(other)) return;

        //Check if cargo is on ship. if so, dont add points
        if (BoatManager.IsPropOnBoat(other.attachedRigidbody)) return;


        //Else, add points
        BaseCargo cargo = other.attachedRigidbody.GetComponent<BaseCargo>();

        //If cargo collected is preferred cargo, evoke twice the amt
        if (cargo.CargoInfo == _preferredCargoInfo)
        {
            MasterGameManager.LevelExtension_Score_SendIncrementScore(Mathf.RoundToInt(cargo.CargoInfo.Value * PREFERRED_MULTIPLIER));
            OnCollectPreferredCargo?.Invoke();
        }
        else
        {
            MasterGameManager.LevelExtension_Score_SendIncrementScore(cargo.CargoInfo.Value);
        }

        OnDeliverCargoToDestination?.Invoke(this);

        //Return cargo to pool
        cargo.ReturnToPool();
    }
    #endregion

}
