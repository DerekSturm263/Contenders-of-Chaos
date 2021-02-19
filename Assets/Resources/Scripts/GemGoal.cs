using System.Collections.Generic;
using UnityEngine;

public class GemGoal : MonoBehaviour
{
    public static List<GameObject> currentGems = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Gem"))
            return;

        if (col.GetComponent<Gem>().holder == GamePlayerInfo.player.transform)
        {
            currentGems.Add(col.gameObject);
            Debug.Log(col.gameObject + " entered the list.");
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (currentGems.Contains(col.gameObject))
        {
            currentGems.Remove(col.gameObject);
            Debug.Log(col.gameObject + " exited the list.");
        }
    }
}
