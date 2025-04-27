using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemySpawner : MonoBehaviour
{

    [SerializeField]
    [Header("Enemy - Reference")]
    EnemyShipController _enemyPrefab = default;

    [SerializeField]
    Transform[] _spawnPositions = default;

    [SerializeField]
    [Header("Enemy Spawn - Values")]
    [Min(1)]
    int _count = 1;

    bool _canSpawnOnlyOnce = false;

#if UNITY_EDITOR
    private void Awake()
    {
        if (_spawnPositions.Length != _count) { Debug.LogError($"Spawn positions and enemy spawn Count must be equal!", this); }
    }
#endif

    private void OnTriggerEnter(Collider other)
    {
        //Only trigger if the collider is part of the boat
        if (!BoatManager.IsPartOfBoat(other)) return;
        if (_canSpawnOnlyOnce) return;

        _canSpawnOnlyOnce = true;
        SpawnEnemies();

    }

    private void SpawnEnemies()
    {
        List<Transform> allPossibleSpawn = new List<Transform>(_spawnPositions);

        for (int i = 0; i < _count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, allPossibleSpawn.Count);
            Transform spawnPosition = allPossibleSpawn[randomIndex];
            EnemyShipController enemy = Instantiate(_enemyPrefab, spawnPosition.position, Quaternion.identity);
            // BaseUIIndicator enemyIndicator = UIIndicatorPool.GetEnemyIndicator(enemy.transform);
            allPossibleSpawn.RemoveAt(randomIndex);
        }
    }
}
