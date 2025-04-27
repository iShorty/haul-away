using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManagement;
//Handles all of the respawning methods for the playermanager
public partial class PlayerManager
{
    [SerializeField] VFXInfo respawnVFXInfo = default;

    #region Initialize & Destroy
    private void RespawnChecker_GameAwake()
    {
        MasterGameManager.OnOneSecondLoop += RespawnChecker_HandleOneSecondLoop;

        for (int playerIndex = 0; playerIndex < _respawnIndicators.Length; playerIndex++)
        {
            Transform playerRespawnPoint = BoatManager.GetPlayerRespawnPosition(playerIndex);
            RespawnIndicator indicator = _respawnIndicators[playerIndex];
            indicator.GameAwake(playerIndex);
            indicator.Initialize(BoatManager.MainCamera, playerRespawnPoint);
            indicator.transform.SetParent(_playerCanvas.transform);
            indicator.gameObject.SetActive(false);
        }
    }

    private void RespawnChecker_GameDestroy()
    {
        MasterGameManager.OnOneSecondLoop -= RespawnChecker_HandleOneSecondLoop;
    }
    #endregion

    #region Updates
    ///<Summary>Will check every one second, whether or not any of the players requires respawning</Summary>
    private void RespawnChecker_HandleOneSecondLoop(float totalGameTimeLeft, int minutes, float seconds)
    {
        //Stuff to check:
        //Are they on the boat?
        //Are they very far from the boat? 
        //(dont need check if they in water because:
        //1) if they are but they arent far away, let the sink timer handle the respawning
        //2) if they arent (some how gets on land), let the players rescue themselves or trigger this respawn check on another loop
        for (int i = 0; i < _players.Count; i++)
        {
            //if player is already respawning
            if (_respawnIndicators[i].gameObject.activeSelf) continue;

            var p = _players[i];

            bool isOnBoat = BoatManager.BoatInventory.IsFloatablePropOnBoat(p.PropRigidBody);

            //Ignore players on boat or already sinking
            if (isOnBoat || p.CurrentPropState == FloatableProp.PropState.SINKING) continue;

            //If dist between player & boat controller is smaller than the respawn distance sqr,
            if (Vector3.SqrMagnitude(p.transform.position - BoatManager.Controller.transform.position) < Constants.For_Player.AUTO_RESPAWN_DISTANCE_SQR)
            {
                continue;
            }

            p.StartRespawn();

        }



    }

    void RespawnChecker_GameUpdate()
    {
        for (int i = 0; i < _respawnIndicators.Length; i++)
        {
            var indicator = _respawnIndicators[i];
            //Update the indicator only when it is set active (due to a player triggering the respawn code)
            if (indicator.gameObject.activeSelf && indicator.GameUpdate())
            {
                //If indicator has finished counting down, hide the indicator 
                indicator.gameObject.SetActive(false);
                EndRespawnPlayer(i);
                //Trigger the next phase of respawning which is the clawing part
                //Get respawn hand to trigger its flowchart
                // BoatManager.RespawnHands.TriggerRespawnHand(i);
            }
        }
    }

    #endregion


    #region Respawn Procedures
    void _StartRespawnPlayer(int index)
    {
        PlayerController player = GetPlayer(index);

        //Hide the player and teleport it to the correct respawn position
        player.gameObject.SetActive(false);

        //Pop up ui according to the index of the player
        RespawnIndicator playerIndicator = _respawnIndicators[index];
        //this will trigger it to get updated by the playermanager
        playerIndicator.StartCountDown(_playerRespawnTime);
        playerIndicator.gameObject.SetActive(true);

    }

    void _EndRespawnPlayer(int index)
    {

        PlayerController player = GetPlayer(index);

        RaiseOnPlayerRespawn(player.transform);

        Transform spawnPosition = BoatManager.GetPlayerRespawnPosition(index);

        player.transform.SetParent(spawnPosition);
        player.transform.localPosition = Vector3.zero;
        player.transform.localRotation = Quaternion.identity;
        player.transform.SetParent(transform);

        player.gameObject.SetActive(true);
        player.EndRespawn();

        AudioEvents.RaiseOnPlay3DAtLocation(AudioClipType.SFX_PlayerSpawnPoof, player.transform.position, true, true);
        VFXObj e = VFXPool.GetInstanceOf(respawnVFXInfo.Prefab, player.transform.position);
        e.Initialise();

        //Push the players abt forward when they respawn to account for boat moving forward
        // player.PropRigidBody.AddForce(player.transform.forward * BoatManager.Controller.LinearSpeed * Constants.For_Player.RESPAWN_FORCE_MULTIPLIER,ForceMode.Impulse);
    }
    #endregion


}
