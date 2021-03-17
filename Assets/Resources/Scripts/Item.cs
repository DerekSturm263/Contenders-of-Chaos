using System;

public class Item
{
    public string itemName;
    public bool canCarry;
    public Action itemAction;

    public Item(string itemName, bool canCarry, Action itemAction)
    {
        this.itemName = itemName;
        this.canCarry = canCarry;
        this.itemAction = itemAction;
    }
}
