using UnityEngine;

public class PlayerData
{
    public string name;
    public int teamNum;

    public enum Device_Type
    {
        PC, MB, Null   // PC = Computer, MB = Mobile.
    }
    public Device_Type deviceType;

    public PlayerData(string name, Device_Type deviceType = Device_Type.Null)
    {
        if (deviceType == Device_Type.Null)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    deviceType = Device_Type.MB;
                    break;

                default:
                    deviceType = Device_Type.PC;
                    break;
            }
        }

        this.name = name;
        this.deviceType = deviceType;
    }
}
