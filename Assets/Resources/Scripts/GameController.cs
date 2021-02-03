using UnityEngine;

public class GameController : MonoBehaviour
{
    public static PlayerData playerInfo;

    private void Awake()
    {
        Initialize();
        DontDestroyOnLoad(this);
    }

    public static void Initialize()
    {
        if (GameObject.FindObjectOfType<GameController>() != null)
            return;

        playerInfo = new PlayerData(SystemInfo.deviceName);
    }
}
