using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralTilemap : MonoBehaviour
{
    private Tilemap levelTiles;
    public TileBase tile;

    public int wallCount;
    public static int tileSeed = 0;

    public List<int> startX = new List<int>();
    public List<int> startY = new List<int>();

    private void Awake()
    {
        levelTiles = GetComponent<Tilemap>();
        GenerateWalls(tileSeed);
    }

    private void Shuffle<T>(ref List<List<T>> walls, int rSeed)
    {
        Random.InitState(rSeed);

        for (int i = 0; i < walls[0].Count; ++i)
        {
            int r = Random.Range(i, walls.Count);

            for (int j = 0; j < walls.Count; ++j)
            {
                T temp = walls[j][i];
                walls[j][i] = walls[j][r];
                walls[j][r] = temp;
            }
        }
    }

    public void GenerateWalls(int seed)
    {
        List<List<int>> tilemapAreas = new List<List<int>> { startX, startY };
        Shuffle(ref tilemapAreas, seed);

        for (int i = 0; i < wallCount; ++i)
        {
            Vector3Int loc = new Vector3Int(startX[i], startY[i], 0);
            levelTiles.BoxFill(loc, tile, startX[i], startY[i], startX[i] + 3, startY[i] + 3);
        }
    }

    public void ResetWalls()
    {
        levelTiles.ClearAllTiles();
    }
}
