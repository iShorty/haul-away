using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>Handles indicators for enemy coming too near or too close to the player</Summary>
[RequireComponent(typeof(SphereCollider))]
public class PlayerBoatDangerDetection : MonoBehaviour
{
    [SerializeField]
    IndicatorInfo _dangerInfo = default;

    // int _numberOfWarningSigns = 0;

    private void Awake()
    {
#if UNITY_EDITOR
        Debug.Assert(gameObject.layer != Constants.For_Layer_and_Tags.LAYERINDEX_PLAYERBOATMODEL, "Danger detection sphere collider must not be on the playerboat layer!", this);
#endif
        SphereCollider c = GetComponent<SphereCollider>();
        c.radius = Constants.For_PlayerBoat.DETECTION_RAIDUS_DANGER;
        c.isTrigger = true;
        // _numberOfWarningSigns = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!EnemyManager.IsEnemy(other)) return;

        // //If this is the first time
        // if (_numberOfWarningSigns == 0)
        // {
        //     AudioManagement.AudioEvents.RaiseOnPlayBGM(AudioClipType.BGM_CombatMusic, AudioManagement.BGMAudioPlayer.BGM_PlayType.FADEIN_LOOP);
        // }
        // _numberOfWarningSigns++;
        //If enemy collider enters 
        UIIndicatorPool.GetIndicator(_dangerInfo, PlayerManager.PlayerCanvas.transform, other.attachedRigidbody);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!EnemyManager.IsEnemy(other)) return;

        // _numberOfWarningSigns--;

        // //If this is last warning
        // if (_numberOfWarningSigns == 0)
        // {
        //     AudioManagement.AudioEvents.RaiseOnPlayBGM(MasterGameManager.CurrentLevelInfo.BGM, AudioManagement.BGMAudioPlayer.BGM_PlayType.FADEIN_LOOP);
        // }

        //If enemy collider exits, we need to remove the  indicator (most likely a danger indicator)
        UIIndicatorPool.TryRemoveIndicator(other.attachedRigidbody);
    }

    ///<Summary>Returns true if collider is within the danger raidius of the boat (This is needed because Warning and Danger trigger spheres overlapp one another)</Summary>
    public static bool CheckWithinDangerSphere(Collider other)
    {
        float distSqr = Vector3.SqrMagnitude(other.transform.position - BoatManager.Controller.transform.position);

        //Check if other is inside of danger zone
        if (distSqr < Constants.For_PlayerBoat.DETECTION_RAIDUS_DANGERSQR)
        {
            //Do danger
            return true;
        }
        return false;
    }

}
