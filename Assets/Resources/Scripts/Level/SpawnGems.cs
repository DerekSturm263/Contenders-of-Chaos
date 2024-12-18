﻿using UnityEngine;
using System.Collections.Generic;

public class SpawnGems : MonoBehaviour
{
    public static GameObject gemPrefab;

    public static int levelNum = 0;
    public static Dictionary<int, List<Vector2>> positions; // Key: Level Num, Value: Gem Position.

    public static int count = 12;
    public static int seed;

    private void Awake()
    {
        gemPrefab = Resources.Load<GameObject>("Prefabs/Gem");

        positions = new Dictionary<int, List<Vector2>>
        {
            // I'm not particularly proud of this.
            { 0, new List<Vector2> { new Vector2(0f, -9f), new Vector2(-11f, -18f), new Vector2(11f, -20f), new Vector2(31f, -11f), new Vector2(-31f, -11f), new Vector2(-43f, -27f), new Vector2(43f, -27f), new Vector2(43f, -11f), new Vector2(-43f, -11f), new Vector2(43f, 5f), new Vector2(-43f, 5f), new Vector2(0f, 31f), new Vector2(0f, 16f), new Vector2(-25.5f, 5f), new Vector2(25.5f, 5f), new Vector2(20f, 14f), new Vector2(-20f, 14f), new Vector2(-38.5f, 25f), new Vector2(38.5f, 25f), new Vector2(-29f, 25f), new Vector2(29f, 25f), new Vector2(-38.5f, -3f), new Vector2(38.5f, -3f), new Vector2(-24.5f, -5.5f), new Vector2(24.5f, -5.5f), new Vector2(-27.5f, 16f), new Vector2(27.5f, 16f), new Vector2(-13f, 33f), new Vector2(13f, 33f), new Vector2(0f, -27f), new Vector2(-22f, -27f), new Vector2(22f, -27f) } },
            { 1, new List<Vector2> { } },
            { 2, new List<Vector2> { } },
            { 3, new List<Vector2> { } }
        };

        LayoutGems(seed);
    }

    public static Transform GetTransform()
    {
        return FindObjectOfType<SpawnGems>().transform;
    }

    public static void Shuffle<T>(ref Dictionary<int, List<T>> gemOrder, int rSeed)
    {
        for (int i = 0; i < gemOrder[levelNum].Count - 1; ++i)
        {
            int r = Random.Range(i, gemOrder[levelNum].Count);
            T temp = gemOrder[levelNum][i];
            gemOrder[levelNum][i] = gemOrder[levelNum][r];
            gemOrder[levelNum][r] = temp;
        }
    }

    public static void LayoutGems(int rSeed)
    {
        Random.InitState(rSeed);
        Shuffle(ref positions, rSeed);

        for (int i = 0; i < count; ++i)
        {
            GameObject newGem = Instantiate(gemPrefab, GetTransform());

            newGem.GetComponent<Gem>().gemNum = i;
            newGem.GetComponent<Gem>().SetWorth(Random.Range(1, 9));

            newGem.transform.position = positions[levelNum][i];
        }
    }

    public void CopyPosition()
    {
        positions[levelNum].Add(transform.position);
        Debug.Log("Added gem at " + transform.position);
    }

    public void ResetPoints()
    {
        if (!positions.ContainsKey(levelNum))
        {
            positions.Add(levelNum, new List<Vector2>());
        }
        else
        {
            positions[levelNum] = new List<Vector2>();
        }
    }
}
