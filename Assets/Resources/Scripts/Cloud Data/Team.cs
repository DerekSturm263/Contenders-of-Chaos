using UnityEngine;

public class Team : MonoBehaviour
{
    public PlayerData[] players = new PlayerData[2];

    private GameObject[] teamMembers = new GameObject[2];
    private GameObject emptyObj;

    private TMPro.TMP_Text p1NameObj, p2NameObj;

    private void Awake()
    {
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.CompareTag("UI Panel"))
            {
                teamMembers[0] = t.gameObject;
            }
            else if (t.CompareTag("UI Label"))
            {
                teamMembers[1] = t.gameObject;
            }
            else if (t.CompareTag("UI Empty"))
            {
                emptyObj = t.gameObject;
            }
            else if (t.CompareTag("UI P1 Name"))
            {
                p1NameObj = t.GetComponent<TMPro.TMP_Text>();
            }
            else if (t.CompareTag("UI P2 Name"))
            {
                p2NameObj = t.GetComponent<TMPro.TMP_Text>();
            }
        }
    }

    public void AddPlayer(PlayerData player)
    {
        switch (player.deviceType)
        {
            case PlayerData.Device_Type.PC:
                players[0] = player;
                break;
            case PlayerData.Device_Type.Mobile:
                players[1] = player;
                break;
        }
    }

    public void RemovePlayer(PlayerData player)
    {
        switch (player.deviceType)
        {
            case PlayerData.Device_Type.PC:
                players[0] = null;
                break;
            case PlayerData.Device_Type.Mobile:
                players[1] = null;
                break;
        }
    }

    public void UpdateInfo()
    {
        emptyObj.SetActive(players[0] != null && players[1] != null);

        if (players[0] != null)
        {
            p1NameObj.text = players[0].name;
        }
        else
        {
            p1NameObj.text = "";
        }

        if (players[1] != null)
        {
            p2NameObj.text = players[1].name;
        }
        else
        {
            p2NameObj.text = "";
        }
    }
}
