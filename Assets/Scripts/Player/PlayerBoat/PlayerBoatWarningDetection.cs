using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>Handles indicators for enemy coming too near or too close to the player</Summary>
[RequireComponent(typeof(SphereCollider))]
public class PlayerBoatWarningDetection : MonoBehaviour
{
    [SerializeField]
    IndicatorInfo _warningInfo = default;

    private void Awake()
    {
#if UNITY_EDITOR
        Debug.Assert(gameObject.layer != Constants.For_Layer_and_Tags.LAYERINDEX_PLAYERBOATMODEL, "Warning detection sphere collider must not be on the playerboat layer!",this);
#endif
        SphereCollider c = GetComponent<SphereCollider>();
        c.radius = Constants.For_PlayerBoat.DETECTION_RAIDUS_WARNING;
        c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!EnemyManager.IsEnemy(other)) return;

        //If enemy collider enters 
        EvaluateTrigger(other);


    }

    //If enemy exits warning detection sphere, try remove its indicator
    private void OnTriggerExit(Collider other)
    {
        if (!EnemyManager.IsEnemy(other)) return;

        //If enemy collider enters 
        UIIndicatorPool.TryRemoveIndicator(other.attachedRigidbody);
    }

    void EvaluateTrigger(Collider other)
    {
        //If within danger sphere,
        if (PlayerBoatDangerDetection.CheckWithinDangerSphere(other))
        {
            return;
        }

        //Else just do a warning
        UIIndicatorPool.GetIndicator(_warningInfo, PlayerManager.PlayerCanvas.transform, other.attachedRigidbody);
    }


}
