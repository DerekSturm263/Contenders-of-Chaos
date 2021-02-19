using UnityEngine;

public class GameController : MonoBehaviour
{
    public static PlayerData playerInfo;

    private void Awake()
    {
        SaveController.Load();
        Initialize();
    }

    public static void Initialize()
    {
        if (playerInfo != null)
            return;

        playerInfo = new PlayerData("", UIController.GetActiveController().testAsMobile ? PlayerData.Device_Type.MB : PlayerData.Device_Type.Null);
        Debug.Log("Created new PlayerData with username: " + playerInfo.name + " and deviceType: " + playerInfo.deviceType);
    }
}
