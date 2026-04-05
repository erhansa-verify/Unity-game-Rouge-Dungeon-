using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups; // List of enemy groups in this wave
        public int waveQuota; // Number of enemies to spawn in this wave
        public float spawnInterval; // Time interval between spawns
        public float spawnCount; // Total number of enemies spawned so far
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount; // Number of enemies of this type to spawn
        public int spawnCount; // Number of enemies of this type spawned so far
        public GameObject enemyPrefab;
    }

    public List<Wave> waves; // List of waves
    public int currentWaveCount; // Current wave index

    [Header("Spawner Attributes")]
    float spawnTimer; // Timer use to determine when to spawn next enemy
    public int enemiesAlive;
    public int maxEnemiesAllowed; // Maximum number of enemies allowed at once
    public bool maxEnemiesReached = false; // Flag to indicate if max enemies limit is reached
    public float waveInterval; // Time interval between waves
    bool isWaveActive = false;

    [Header("Spawn Position")]
    public List<Transform> relativeSpawnPoints; // List of possible spawn points

    Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<PlayerStats>().transform;
        CalculateWaveQuota();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the current wave is completed
        if(currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0 && !isWaveActive) // Check if wave is completed
        {
            StartCoroutine(BeginNextWave());
        }
        spawnTimer += Time.deltaTime;

        // Check if it's time to spawn the next enemy
        if(spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            SpawnEnemies();
            spawnTimer = 0f;
        }
    }

    IEnumerator BeginNextWave()
    {
        isWaveActive = true;

        // Wait for the specified interval before starting the next wave
        yield return new WaitForSeconds(waveInterval);

        // Move to the next wave if available
        if(currentWaveCount < waves.Count - 1)
        {
            isWaveActive = false;
            currentWaveCount++;
            CalculateWaveQuota();
        }
    }

    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota;
        Debug.Log("Wave " + waves[currentWaveCount].waveName + " Quota: " + currentWaveQuota);
    }

    /// <summary>
    /// This method will stop spawning enemies if the maximum allowed enemies are already present.
    /// The method will only spawn enemies in a particular wave until it is time for the next wave's enemies to be spawned.
    /// </summary>

    void SpawnEnemies()
    {
        // Check if we can spawn more enemies in the current wave
        if(waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {
            // Iterate through each enemy group in the current wave
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                // Check if we can spawn more enemies of this type
                if (enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    // Spawn the enemy at a random spawn point relative to the player
                    Instantiate(enemyGroup.enemyPrefab, player.position + relativeSpawnPoints[Random.Range(0, relativeSpawnPoints.Count)].position, Quaternion.identity);

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;

                    // Limit the number of enemies alive to the maximum allowed
                    if(enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }
                }
            }
        }
    }

    // Call this method when an enemy is killed to update the count of alive enemies
    public void OnEnemyKilled()
    {
        // Decrement the count of alive enemies when an enemy is killed
        enemiesAlive--;

        // Reset maxEnemiesReached flag if we are below the limit
        if(enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }
}
