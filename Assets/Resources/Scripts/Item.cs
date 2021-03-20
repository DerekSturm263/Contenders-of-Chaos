using System;

public class Item
{
    public string itemName;
    public bool canCarry;
    public Action itemAction;
    public Action throwAction;

    public Item(string itemName, bool canCarry, Action itemAction, Action throwAction = null)
    {
        this.itemName = itemName;
        this.canCarry = canCarry;
        this.itemAction = itemAction;
        this.throwAction = throwAction;
    }
}
