using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpawnGems : MonoBehaviour
{
    public GameObject gemPrefab;

    public Vector2[] positions = new Vector2[64];

    public int count = 16;
    public static int seed;

    private void Awake()
    {
        LayoutGems(seed);
    }

    private void Shuffle<T>(ref List<T> list, int rSeed)
    {
        Random.InitState(rSeed);

        for (int i = 0; i < list.Count; ++i)
        {
            var nextSpot = i + Random.Range(-1, 2);

            T temp;

            if (nextSpot < 0 || nextSpot >= list.Count)
                nextSpot = i;

            temp = list[nextSpot];
            list[nextSpot] = list[i];
            list[i] = temp;
        }
    }

    public void LayoutGems(int rSeed)
    {
        List<Vector2> positionsList = positions.ToList();
        Shuffle(ref positionsList, rSeed);

        for (int i = 0; i < count; ++i)
        {
            GameObject newGem = Instantiate(gemPrefab, transform);
            newGem.transform.position = positions[i];
        }
    }
}
