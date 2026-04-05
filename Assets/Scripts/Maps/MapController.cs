using UnityEngine;
using System.Collections.Generic;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    Vector3 playerLastPossition;

    [Header("Optimization")]
    public List<GameObject> spawnedChunks;
    GameObject latestChunk;
    public float maxOpDist; // Must be greate than the size of the tilemap
    float opDist;
    float optimizerCooldown;
    public float optimizeCooldownDuration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerLastPossition = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ChunkChecker();
        ChunkOptimizer();
    }

    void ChunkChecker()
    {
        if (!currentChunk)
        {
            return;
        }

        Vector3 moveDir = player.transform.position - playerLastPossition;
        playerLastPossition = player.transform.position;

        string directionName = GetDirectionName(moveDir);

        CheckAdjacentChunks(directionName);
    }

    void CheckAndSpawn(string direction)
    {
        Transform point = currentChunk.transform.Find(direction);
        if (point == null) return;

        if (!Physics2D.OverlapCircle(point.position, checkerRadius, terrainMask))
        {
            SpawnChunk(point.position);
        }
    }


    void CheckAdjacentChunks(string directionName)
    {
        // Always check the main direction first
        CheckAndSpawn(directionName);

        bool up = directionName.Contains("Up");
        bool down = directionName.Contains("Down");
        bool left = directionName.Contains("Left");
        bool right = directionName.Contains("Right");

        // Diagonal movement → also check its two cardinal directions
        if ((up || down) && (left || right))
        {
            if (up) CheckAndSpawn("Up");
            if (down) CheckAndSpawn("Down");
            if (left) CheckAndSpawn("Left");
            if (right) CheckAndSpawn("Right");
            return;
        }

        // Horizontal movement
        if (right)
        {
            CheckAndSpawn("Right Up");
            CheckAndSpawn("Right Down");
        }
        else if (left)
        {
            CheckAndSpawn("Left Up");
            CheckAndSpawn("Left Down");
        }
        // Vertical movement
        else if (up)
        {
            CheckAndSpawn("Right Up");
            CheckAndSpawn("Left Up");
        }
        else if (down)
        {
            CheckAndSpawn("Right Down");
            CheckAndSpawn("Left Down");
        }
    }


    string GetDirectionName(Vector3 direction)
    {
        direction = direction.normalized;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Horizontal movement
            if (direction.y > 0.5f)
            {
                // Also moving upwards
                return direction.x > 0 ? "Right Up" : "Left Up";
            }
            else if (direction.y < -0.5f)
            {
                // Also moving downwards
                return direction.x > 0 ? "Right Down" : "Left Down";
            }
            else
            {
                // Purely horizontal
                return direction.x > 0 ? "Right" : "Left";
            }
        }
        else
        {
            // Vertical movement
            if (direction.x > 0.5f)
            {
                // Also moving right
                return direction.y > 0 ? "Right Up" : "Right Down";
            }
            else if (direction.x < -0.5f)
            {
                // Also moving left
                return direction.y > 0 ? "Left Up" : "Left Down";
            }
            else
            {
                // Purely vertical
                return direction.y > 0 ? "Up" : "Down";
            }
        }
    }

    void SpawnChunk(Vector3 spawnPosition)
    {
        int rand = Random.Range(0, terrainChunks.Count);
        latestChunk = Instantiate(terrainChunks[rand], spawnPosition, Quaternion.identity);
        spawnedChunks.Add(latestChunk);
    }

    void ChunkOptimizer()
    {
        optimizerCooldown -= Time.deltaTime;

        if (optimizerCooldown <= 0f)
        {
            optimizerCooldown = optimizeCooldownDuration;
        }
        else
        {
            return;
        }

        foreach (GameObject chunk in spawnedChunks)
        {
            opDist = Vector3.Distance(player.transform.position, chunk.transform.position);
            if (opDist > maxOpDist)
            {
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }
}
