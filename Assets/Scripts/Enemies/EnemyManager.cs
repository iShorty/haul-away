// using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class EnemyManager : GenericPools<Enemy, EnemyManager>, IGlobalEventManager
// public partial class EnemyManager : GenericScenePools<Enemy, EnemyManager>, IGlobalEventManager
{
    // public List<Enemy> enemyBoarders { get; private set; }
    // public int numberOfEnemiesTotal { get; private set; }
    // public int numberOfEnemiesRemaining => enemies.Count;
    public int numberOfBoardersRemaining => enemyBoarders.Count;

    [SerializeField, Header("----- Manager Settings -----")]
    private int _executionPriority = 0;

    // Stick this into an SO?
    [Header("Used to position enemies near the player boat")]
    public static float _forwardArcAngleMin = 25f;
    public static float _forwardArcAngleMax = 65f;
    private bool _runFixedUpdate = false;

    public event System.Action<Enemy, int> onRemoveEnemy = null;
    public static event System.Action onAddFirstEnemyBoarder = null;
    public static event System.Action onRemoveLastEnemyBoarder = null;


#if UNITY_EDITOR
    [Header("----- RunTime -----")]
    [SerializeField]
#endif
    List<Enemy> enemies = default;
#if UNITY_EDITOR
    [SerializeField]
#endif
    List<Enemy> enemyBoarders = default;
#if UNITY_EDITOR
    [SerializeField]
#endif
    int numberOfEnemiesTotal = default;
    int numberOfEnemiesRemaining => enemies.Count;

    public GameObject GO => gameObject;
    // static Dictionary<Rigidbody, Enemy> enemyDict { get; set; } = default;

    public int ExecutionOrder => _executionPriority;

    #region Static Fields/Methods

    public static EnemyManager Instance => instance;

    public static void RegisterEnemy(Enemy enemy)
    {
        instance.enemies.Add(enemy);
        instance.numberOfEnemiesTotal++;
    }

    public static void UnregisterEnemy(Enemy enemyKilled)
    {
        int enemiesRemainingNotification = instance.numberOfEnemiesRemaining - 1;
        if (instance.onRemoveEnemy != null)
        {
            instance.onRemoveEnemy.Invoke(enemyKilled, enemiesRemainingNotification);
        }
        instance.enemies.Remove(enemyKilled);
    }

    public static bool IsEnemy(Collider c)
    {
#if UNITY_EDITOR
        bool isEnemy = !IsSeaMine(c) && (c.gameObject.layer == Constants.For_Layer_and_Tags.LAYERINDEX_ENEMYBOATMODEL);
        if (isEnemy)
        {
            Debug.Assert(c.attachedRigidbody, $"The collider {c.name} needs to have a rigidbody in order to be a Enemy!", c);
        }

        return isEnemy;
#else

        return !IsSeaMine(c) && c.gameObject.layer == Constants.For_Layer_and_Tags.LAYERINDEX_ENEMYBOATMODEL;
#endif
    }

    public static bool IsSeaMine(Collider c)
    {
        return c.CompareTag(Constants.For_Layer_and_Tags.TAG_SEAMINE);
    }

    #endregion

    public void GameAwake()
    {
        enemies = new List<Enemy>();
        enemyBoarders = new List<Enemy>();

        GlobalEvents.OnGameUpdate_DURINGGAME += DuringGameUpdate;
        GlobalEvents.OnGameFixedUpdate_DURINGGAME += DuringGameFixedUpdate;
        GlobalEvents.OnGameReset += ResetGame;
    }

    public void OnDestroy()
    {
        GlobalEvents.OnGameUpdate_DURINGGAME -= DuringGameUpdate;
        GlobalEvents.OnGameFixedUpdate_DURINGGAME -= DuringGameFixedUpdate;
        GlobalEvents.OnGameReset -= ResetGame;
        // base.OnDestroy();

    }

    #region Update Handles

    void DuringGameUpdate()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            enemies[i].GameUpdate();
        }
    }

    void DuringGameFixedUpdate()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            enemies[i].FixedGameUpdate();
        }
    }

    #endregion

    public void RegisterBoarder(Enemy enemyBoarder)
    {
        if (enemyBoarders.Count <= 0)
            onAddFirstEnemyBoarder?.Invoke(); // before adding first boarder unit
        enemyBoarders.Add(enemyBoarder);
        numberOfEnemiesTotal++;
    }

    public void UnRegisterBoarder(Enemy enemyBoarderKilled)
    {
        // If called properly, unnecessary check.
        if (enemyBoarders.Contains(enemyBoarderKilled))
        {
            enemyBoarders.Remove(enemyBoarderKilled);
        }
        if (enemyBoarders.Count <= 0)
        {
            onRemoveLastEnemyBoarder?.Invoke(); // After removing last boarder unit
        }
    }



    #region One-Frame Events Handle

    private void ResetGame()
    {
        //Add whatever reseting of values or clean up code u wan when lvl is reseting
    }

    #endregion



}
