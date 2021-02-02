using UnityEngine;

public class PlayerData
{
    public string name;

    public enum Device_Type
    {
        PC, MB, Null   // PC = Computer, MB = Mobile.
    }
    public Device_Type deviceType;

    public PlayerData(string name, Device_Type deviceType = Device_Type.Null)
    {
        if (deviceType == Device_Type.Null)
        {
            #if UNITY_ANDROID
                deviceType = Device_Type.MB;
            #else
                deviceType = Device_Type.PC;
            #endif
        }

        this.name = name;
    }
}
