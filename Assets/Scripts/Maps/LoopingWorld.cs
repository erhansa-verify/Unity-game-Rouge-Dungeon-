using UnityEngine;
using UnityEngine.Tilemaps;

public class LoopingWorld : MonoBehaviour
{
    [Header("Map Settings")]
    public int width = 100;
    public int height = 100;
    public float noiseScale = 10f;
    public TileBase groundTile;
    public TileBase waterTile;
    public Tilemap tilemap;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap not assigned!");
            return;
        }

        // Generate simple perlin noise map
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noise = Mathf.PerlinNoise(x / noiseScale, y / noiseScale);
                TileBase tile = noise > 0.4f ? groundTile : waterTile;
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        Debug.Log("Procedural map generated!");
    }
}
