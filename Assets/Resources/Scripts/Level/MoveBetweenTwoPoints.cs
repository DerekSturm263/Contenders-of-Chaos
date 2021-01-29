using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBetweenTwoPoints : MonoBehaviour
{
    public List<Vector2> positions = new List<Vector2>();

    private int newSpotIndex = 1;

    public float moveSpeed;

    private void Update()
    {
        transform.position = Vector2.Lerp(transform.position, positions[newSpotIndex], Time.deltaTime * moveSpeed / Vector2.Distance(transform.position, positions[newSpotIndex]));
    }

    private void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, positions[newSpotIndex]) < 0.01f)
            ChooseNewSpot();
    }

    private void ChooseNewSpot()
    {
        newSpotIndex++;

        if (newSpotIndex >= positions.Count)
        {
            newSpotIndex = 0;
        }
    }

    public void CopyPosition()
    {
        positions.Add(transform.position);
    }
}
