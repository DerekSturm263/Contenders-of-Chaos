using UnityEngine;
using System;

public class ItemManager : MonoBehaviour
{
    public const string FilePath = "Sprites/";

    public Item beehive = new Item("Beehive", false, new Action(() =>
    {
        Debug.Log("Use Beehive.");
    }));

    public Item cloak = new Item("Cloak", false, new Action(() =>
    {
        Debug.Log("Pick up cloak.");
    }));

    private void Awake()
    {
        GameObject item1 = NewItem(beehive);
        GameObject item2 = NewItem(cloak);

        InvokeRepeating("SpawnItem", 30f, 30f);
    }

    public GameObject NewItem(Item newItem)
    {
        GameObject newObj = new GameObject(newItem.itemName);
        newObj.AddComponent<Float>();
        newObj.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(FilePath + newItem.itemName);
        newObj.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
        newObj.AddComponent<ItemAction>().canCarry = newItem.canCarry;
        newObj.GetComponent<ItemAction>().itemAction = newItem.itemAction;
        newObj.tag = "Item";
        newObj.transform.position = new Vector2(-1000f, -1000f);

        return newObj;
    }

    public void SpawnItem()
    {
        ItemAction.items[UnityEngine.Random.Range(0, ItemAction.items.Count - 1)].Spawn();
    }
}
