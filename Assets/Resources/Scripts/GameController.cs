using UnityEngine;

public class GameController : MonoBehaviour
{
    public static PlayerData playerInfo;

    private void Awake()
    {
        SaveController.Load();
        Initialize();

        if (!MusicPlayer.Exists())
        {
            MusicPlayer.Initialize();
            SoundPlayer.Initialize();

            MusicPlayer.Play("Update");
        }

        DontDestroyOnLoad(FindObjectsOfType<AudioSource>()[0]); // Music Player.
        DontDestroyOnLoad(FindObjectsOfType<AudioSource>()[1]); // Sound Player.
    }

    public static void Initialize()
    {
        if (playerInfo != null)
            return;

        playerInfo = new PlayerData("", UIController.GetActiveController().testAsMobile ? PlayerData.Device_Type.MB : PlayerData.Device_Type.Null);
        Debug.Log("Created new PlayerData with username: " + playerInfo.name + " and deviceType: " + playerInfo.deviceType);
    }
}
