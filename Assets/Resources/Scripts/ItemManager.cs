﻿using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class ItemManager : MonoBehaviour
{
    public const string FilePath = "Sprites/";

    public Item beehive = new Item("Beehive", false, new Action(() =>
    {
        Debug.Log("Use Beehive.");
        GetItemManager().StartCoroutine(PullExample());
    }));
    public Item cloak = new Item("Cloak", false, new Action(() =>
    {
        Debug.Log("Pick up cloak.");
    }));
    public Item carryTest = new Item("Beehive", true, new Action(() =>
    {
        Debug.Log("Carry item.");
    }));

    public static ItemManager GetItemManager()
    {
        return FindObjectOfType<ItemManager>();
    }

    private void Awake()
    {
        // Don't forget to make the items after you define them.
        GameObject item1 = NewItem(beehive);
        GameObject item2 = NewItem(cloak);
        GameObject item3 = NewItem(carryTest);

        // Spawns a new item every 30 seconds.
        InvokeRepeating("SpawnItem", 30f, 30f);
    }

    // Example on how to push.
    public static IEnumerator PushExample()
    {
        // Create a form.
        WWWForm form2 = new WWWForm();
        form2.AddField("groupid", "pm36"); // Group ID.
        form2.AddField("grouppw", "N3Km3yJZpM"); // Password.
        form2.AddField("row", 540); // Row you're pushing to.
        form2.AddField("s4", "Testing"); // Value that you're pushing.

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form2)) // Uses the PushUrl.
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
        }
    }

    // Example on how to pull.
    public static IEnumerator PullExample()
    {
        int rowNum = 540;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudGameData.PullURL + rowNum)) // Uses the PullUrl.
        {
            yield return webRequest.SendWebRequest();

            Debug.Log(webRequest.downloadHandler.text); // Returns whatever the pulling thing pulled from the website.
            // Wouldn't return Testing.
            // Would return: 540, Testing.
            
            // I find this really annoying.
            
            // 540, Testing = ["540", "Testing"];
            string result = webRequest.downloadHandler.text.Split(',')[1];

            Debug.Log("New text:" + result);
        }
    }

    public GameObject NewItem(Item newItem)
    {
        GameObject newObj = new GameObject(newItem.itemName);
        newObj.AddComponent<Float>();
        newObj.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(FilePath + newItem.itemName);
        newObj.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
        newObj.AddComponent<ItemAction>().canCarry = newItem.canCarry;
        newObj.GetComponent<ItemAction>().itemAction = newItem.itemAction;
        newObj.AddComponent<BoxCollider2D>().isTrigger = true;
        newObj.tag = "Item";
        newObj.transform.position = new Vector2(-1000f, -1000f);

        return newObj;
    }

    public void SpawnItem()
    {
        int randomNum = UnityEngine.Random.Range(0, ItemAction.items.Count - 1);

        ItemAction.items[randomNum].GetComponent<Float>().enabled = false;
        ItemAction.items[randomNum].Spawn();
        ItemAction.items[randomNum].GetComponent<Float>().enabled = true;
    }
}
