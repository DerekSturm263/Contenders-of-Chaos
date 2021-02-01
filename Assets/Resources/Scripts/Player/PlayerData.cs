using UnityEngine;

public class PlayerData
{
    public string name;

    public enum Device_Type
    {
        PC, Mobile
    }
    public Device_Type deviceType;

    public PlayerData(string name)
    {
        this.name = name;
    }
}
