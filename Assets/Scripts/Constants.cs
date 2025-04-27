using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Constants
{
    //=========== UNCLASSIFIED ===========
    public static readonly float ABSOLUTE_GRAVITY = Mathf.Abs(Physics.gravity.y);
    public static readonly Vector3 POOLEDOBJECT_HIDE_POSITION = new Vector3(0, -1000, 0);
    public const float BASEITEM_SINKING_ANIMATION_DURATION = 7f
    , KILLHEIGHTMIN = -25f
    , KILLHEIGHTMAX = 100f
    ;

    public const string MATERIAL_DEFAULT_PROPERTYNAME_COLOR = "_BaseColor";
    public const string MATERIAL_URPDECAL_PROPERTYNAME_TEXTURE = "_MainTex";
    public const string MATERIAL_URPDECAL_PROPERTYNAME_Color = "_Color";
#if UNITY_EDITOR
    public const string MATERIAL_URPDECAL_PASSNAME = "URPDecal";
#endif

    //=============== PLAYER CONSTANTS =============
    public static class For_Player
    {
        //------ Water Constants ---------
        // public const float SINKDELAY = 4f
        // , FLOATDELAY = 10f
        // ;
        ///<Summary>The distance between the player and the boat in order to consider the player valid for auto respawning at a shorter time.</Summary>
        public const float AUTO_RESPAWN_DISTANCE_SQR = 1000f;
        public const float RESPAWN_FORCE_MULTIPLIER = 50f;


        //---------- Interaction ------------
        ///<Summary>Little offset so that the object's collider doesnt hit the player capsule collider</Summary>
        public const float PICKUP_OBJECT_OFFSET_Y = 0.05f;

        public const float PLACE_OBJECT_OFFSET_Y = 0.05f;
        public const float PLACE_OBJECT_OFFSET_Z = 0.075f;

        #region ------- Animator -----------
        public const string ANIMATOR_PARAM_VELOCITY_SQRMAG = "VelocitySqrMagnitude";
        public const string ANIMATOR_PARAM_PICKUP = "PickUp";
        public const string ANIMATOR_PARAM_ENTERDAZED = "EnterDazed";
        public const string ANIMATOR_PARAM_EXITDAZED = "ExitDazed";
        public const string ANIMATOR_PARAM_STEERVALUE = "SteerDirection";


        ///<Summary>If movementinput ^2 is more than this leeway value, consider it as the player moving</Summary>
        public const float MOVEMENTINPUTSQR_LEEWAY = 0.01f;

        public const float PICKUP_ANIMATION_DELAY = 0.25f;
        #endregion

        //------- Color -----------
        public const string PLAYER_RINGUI_NAME = "RingUI";
        ///<Summary>Returns the respective colors which belongs to each playerindex. Please note that the player index must be zero-based</Summary>
        public static Color GetColor(int playerIndex)
        {
            switch (playerIndex)
            {
                case 0:
                    return PLAYER_0_COLOR;
                case 1:
                    return PLAYER_1_COLOR;

                case 2:
                    return PLAYER_2_COLOR;

                case 3:
                    return PLAYER_3_COLOR;

                default:
                    Debug.LogWarning($"Somehow, player index of {playerIndex} has been passed in as a para!");
                    return Color.white;
            }
        }


        //Blue:  #0AA8EF 102, 199, 255
        static readonly Color PLAYER_0_COLOR = new Color(10 / 255f, 168 / 255f, 239 / 255f);
        //Red: #EB3F2A
        static readonly Color PLAYER_1_COLOR = new Color(235 / 255f, 63 / 255f, 42 / 255f);
        //Green: #1FD00E
        static readonly Color PLAYER_2_COLOR = new Color(31 / 255f, 208 / 255f, 14 / 255f);
        //Yellow: #F3DF30
        static readonly Color PLAYER_3_COLOR = new Color(243 / 255f, 223 / 255f, 48 / 255f);

        public const string ACTION_NAME_MOVEMENT = "Movement"
               , ACTION_NAME_INTERACT = "Interact"
               , ACTION_NAME_LEAVE = "Leave"
               , ACTION_NAME_USE = "Use"
               , ACTION_NAME_TOGGLELEFT = "ToggleLeft"
               , ACTION_NAME_TOGGLERIGHT = "ToggleRight"
               , ACTION_NAME_TOGGLECAMERA = "ToggleCamera"
               , ACTION_NAME_PAUSE = "Pause"
               ;

    }


    public static class For_Enemy
    {
        public const string ORCA_ANIMATION_PARAM_ATTACK = "Attack";
    }

    //============== LAYER CONSTANTS =====================
    public static class For_Layer_and_Tags
    {
        // public const string LAYERNAME_DEFAULT = "Default";
        // public static readonly int LAYERINDEX_DEFAULT = LayerMask.NameToLayer(LAYERNAME_DEFAULT);
        // public static readonly int LAYERMASK_DEFAULT = 1 << LAYERINDEX_DEFAULT;

        //--------- INTERACTABLE LAYER --------------
        ///<Summary>Player interactable layer contains cargo, player projectiles and stations which player can interact with</Summary>
        public const string LAYERNAME_PLAYERINTERACTABLE = "Interactable_Player";
        ///<Summary>Player interactable layer contains cargo, player projectiles and stations which player can interact with</Summary>
        public static readonly int LAYERINDEX_PLAYERINTERACTABLE = LayerMask.NameToLayer(LAYERNAME_PLAYERINTERACTABLE);

        ///<Summary>Enemy interactable layer contains enemy projectiles which player can interact with</Summary>
        public const string LAYERNAME_ENEMYINTERACTABLE = "Interactable_Enemy";
        ///<Summary>Enemy interactable layer contains enemy projectiles which player can interact with</Summary>
        public static readonly int LAYERINDEX_ENEMYINTERACTABLE = LayerMask.NameToLayer(LAYERNAME_ENEMYINTERACTABLE);

        //In the case where the mesh filter & gameobject i
        //Layer mask to include both the interactable index & detected interactable only
        public static readonly int LAYERMASK_INTERACTABLE_FINALMASK = 1 << LAYERINDEX_PLAYERINTERACTABLE | 1 << LAYERINDEX_ENEMYINTERACTABLE;
        // public static readonly int LAYERMASK_INTERACTABLE_FINALMASK = 1 << LAYERINDEX_INTERACTABLE | 1 << LAYERINDEX_DETECTEDINTERACTABLE;

        // public const string LAYERNAME_DETECTEDINTERACTABLE = "DetectedInteractable";
        // public static readonly int LAYERINDEX_DETECTEDINTERACTABLE = LayerMask.NameToLayer(LAYERNAME_DETECTEDINTERACTABLE);



        //--------- WATER LAYER --------------
        public const string LAYERNAME_WATER = "Water";
        public static readonly int LAYERINDEX_WATER = LayerMask.NameToLayer(LAYERNAME_WATER);
        // public static readonly int LAYERMASK_WATER = 1 << LAYERINDEX_WATER;

        //--------- TERRAIN LAYER --------------
        public const string LAYERNAME_TERRAIN = "Terrain";
        public static readonly int LAYERINDEX_TERRAIN = LayerMask.NameToLayer(LAYERNAME_TERRAIN);
        // public static readonly int LAYERMASK_TERRAIN = 1 << LAYERINDEX_TERRAIN;

        #region ====================== Enemy ======================
        #region -------------- Enemy Layers -------------------
        //-------------- Enemy boat model collider layers ------------
        ///<Summary>Enemy boatmodel layer contains the enemy boat mesh as colliders.</Summary>
        public const string LAYERNAME_ENEMYBOATMODEL = "EnemyBoatModel";
        ///<Summary>Enemy boatmodel layer contains the enemy boat mesh as colliders.</Summary>
        public static readonly int LAYERINDEX_ENEMYBOATMODEL = LayerMask.NameToLayer(LAYERNAME_ENEMYBOATMODEL);
        ///<Summary>Enemy boatmodel layer contains the enemy boat mesh as colliders.</Summary>
        public static readonly int LAYERMASK_ENEMYBOATMODEL = 1 << LAYERINDEX_ENEMYBOATMODEL;
        #endregion


        #region -------------- Enemy boat tall tall collider layer -------------------
        ///<Summary>BOATONBOAT_ENEMY layer contains the tall tall box collider of the enemy. It will only collide with other BOATONBOAT layers</Summary>
        public const string LAYERNAME_BOATONBOAT_ENEMY = "BoatOnBoat_Enemy";
        ///<Summary>BOATONBOAT_ENEMY layer contains the tall tall box collider of the enemy. It will only collide with other BOATONBOAT layers</Summary>
        public static readonly int LAYERINDEX_BOATONBOAT_ENEMY = LayerMask.NameToLayer(LAYERNAME_BOATONBOAT_ENEMY);
        ///<Summary>BOATONBOAT_ENEMY layer contains the tall tall box collider of the enemy. It will only collide with other BOATONBOAT layers</Summary>
        public static readonly int LAYERMASK_BOATONBOAT_ENEMY = 1 << LAYERINDEX_BOATONBOAT_ENEMY;

        #region Enemy Detection

        //--------- ENEMY DETECTION LAYER --------------
        ///<Summary>EnemyDetection layer contains a sphere collider of the enemy. It will only collide with PLAYERBOATMODEL layer (and things it needs to detect).</Summary>
        public const string LAYERNAME_ENEMYDETECTION = "EnemyDetection";
        public static readonly int LAYERINDEX_ENEMYDETECTION = LayerMask.NameToLayer(LAYERNAME_ENEMYDETECTION);
        // public static readonly int LAYERMASK_ENEMYDETECTION = 1 << LAYERINDEX_ENEMYDETECTION;

        #endregion


        #endregion



        #endregion



        #region Player

        //====================== PLAYERBOAT STUFF ======================
        //--------- PLAYERBOAT LAYER --------------
        public const string LAYERNAME_PLAYERBOATMODEL = "PlayerBoatModel";
        public static readonly int LAYERINDEX_PLAYERBOATMODEL = LayerMask.NameToLayer(LAYERNAME_PLAYERBOATMODEL);
        public static readonly int LAYERMASK_PLAYERBOATMODEL = 1 << LAYERINDEX_PLAYERBOATMODEL;

        public const string LAYERNAME_BOATONBOAT_PLAYER = "BoatOnBoat_Player";
        public static readonly int LAYERINDEX_BOATONBOAT_PLAYER = LayerMask.NameToLayer(LAYERNAME_BOATONBOAT_PLAYER);

        //--------- PLAYERDECK LAYER --------------
        public const string LAYERNAME_PLAYERDECK = "PlayerBoatNav";
        public static readonly int LAYERINDEX_PLAYERDECK = LayerMask.NameToLayer(LAYERNAME_PLAYERDECK);
        #endregion


        #region --------------- Enemy LayerMasks --------------------
        public static readonly int LAYERMASK_PROJECTILE = LAYERMASK_PLAYERBOATMODEL | LAYERMASK_INTERACTABLE_FINALMASK | LAYERMASK_ENEMYBOATMODEL;
        #endregion


        //============== TAG CONSTANTS =====================
        //Use CompareTag instead of (string == tag) check. Int comparison is faster
        public const string TAG_CARGO = "Cargo";
        public const string TAG_UNGRAPPLEABLE = "UnGrappleable";
        public const string TAG_PLAYER = "Player";
        public const string TAG_STATION = "Station";
        public const string TAG_FUEL = "Fuel";
        public const string TAG_DESTINATION = "Finish";

        public const string TAG_SEAMINE = "SeaMine";
        // public const string TAG_BOAT = "Boat";
        // public const string TAG_FINISH = "Finish";


    }


    //======= SCRIPTABLE OBJECT CREATEASSET CONSTANTS ==================
    public const string ASSETMENU_CATEGORY_PLAYER = "Player"
    , ASSETMENU_CATEGORY_PLAYER_BOAT = ASSETMENU_CATEGORY_PLAYER + "/Boat"
    , ASSETMENU_CATEGORY_WATERPHYSICS = "WaterPhysics"
    , ASSETMENU_CATEGORY_PLAYERPICKABLE = "PlayerPickable"
    , ASSETMENU_CATEGORY_VFXINFO = "VFX"
    , ASSETMENU_CATEGORY_DETECTIONTARGETCONDITIONS = "DetectionTargetConditions"
    , ASSETMENU_CATEGORY_LEVELINFO = "Level"
    , ASSETMENU_CATEGORY_LEVELOBJECTIVE = ASSETMENU_CATEGORY_LEVELINFO + "/Objectives"
    , ASSETMENU_CATEGORY_UI = "UI"
    , ASSETMENU_CATEGORY_UIShaderEffects = ASSETMENU_CATEGORY_UI + "/ShaderEffects"
    ;



    public static class For_Projectiles
    {
        public const float H_VALUE = 3
        , GRAVITY = -9.81f
        ;

        public const float BOMB_SCALAR = 1f;

    }
    //========== BOMB CONSTANTS =========
    public const float BOMB_UPWARDSMODIFIER = 1.5f
    , BOMB_INWATER_FORCE_DAMPENING = 0.5f
    ;


    public static class For_PlayerBoat
    {
        //========= MULTIPURPOSE CONSTANTS ============
        public const float DETECTION_RAIDUS_WARNING = 75f;
        public const float DETECTION_RAIDUS_DANGER = 50f;
        public const float DETECTION_RAIDUS_CARGO = 50f;
        public static readonly float DETECTION_RAIDUS_DANGERSQR = DETECTION_RAIDUS_DANGER * DETECTION_RAIDUS_DANGER;

    }

    public static class For_PlayerStations
    {
        //========= MULTIPURPOSE CONSTANTS ============
        public const int MULTIPURPOSE_CANNON_DMG = 1;
        public const int MULTIPURPOSE_BEZIER_HEIGHT = 15;

        public const string MULTIPURPOSE_ANIMATION_PARAM_FIRE = "Fire";
        public const float MULTIPURPOSE_ANIMATION_DELAY = 0.25f;

        public const int MULTIPURPOSE_WATERLEVEL = 0;

        public static string MULTIPURPOSE_ANIMATION_PARAM_GRAPPLEOUT = "GrappleOut";
        public static string MULTIPURPOSE_ANIMATION_PARAM_GRAB = "Grab";

        public static string MULTIPURPOSE_ANIMATION_PARAM_TOSS = "Toss";

        public const float FUEL_ENGINESTATION_COOLDOWN = 1f;

        public static readonly Color STEERING_REVERSEMODE_LIGHTBULB_COLOR = new Color(3f, 0, 0);

        public const float FUELSTORAGESTATION_COALMESH_STARTHEIGHT = 0.32f;
    }

    public static class For_InGameUI
    {
        //---------- Level Timer ------------
        public const string LEVELTIMER_STRINGFORMAT = "{0:00}:{1:00}";

        //----------- Game Screen -----------
        public static readonly Color STARCOLOUR_INACTIVE = new Color(1, 1, 1, 0.5f);
        public static readonly Color STARCOLOUR_ACTIVE = new Color(1, 1, 1, 1);

        //----------- Indicators -----------
        public const float WARNING_INDICATOR_TIMEDELAY = 5f;
        // ///<Summary>Max number of stars per level</Summary>
        // public const int MAX_NUMBER_OF_STARS = 3;
    }

    public static class For_Scene
    {
        ///<Summary>Cold start scene is where global singleton and all one-time app called scripts are initialized</Summary>
        public const string SCENENAME_COLDSTART = "ColdStart";

        public const string SCENENAME_MAINMENU = "Menu"
        , SCENENAME_TUTORIAL_01 = "Tutorial 1"
        ;
    }

    public static class For_MasterGameManager
    {
        public const float GAMESTART_DELAY = 0f;

        //------------ Timer Constants --------------
        public static readonly WaitForSeconds UPDATEINTERVAL_ONESECOND = new WaitForSeconds(1f);

    }


}
