using UnityEngine;
using UnityEngine.UI;

public class Team : MonoBehaviour
{
    private GameObject emptyBG;
    private GameObject emptyLabel;
    private GameObject[] playerBGs = new GameObject[2];
    private GameObject teamNumber;

    private TMPro.TMP_Text p1Name, p2Name;
    private PlayerData[] players = new PlayerData[2];

    private void Awake()
    {
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.CompareTag("UI Label"))
            {
                teamNumber = t.gameObject;
            }
            else if (t.CompareTag("UI Empty"))
            {
                if (emptyBG == null) emptyBG = t.gameObject;
                else if (emptyLabel == null) emptyLabel = t.gameObject;
            }
            else if (t.CompareTag("UI P1 Name"))
            {
                p1Name = t.GetComponent<TMPro.TMP_Text>();
            }
            else if (t.CompareTag("UI P2 Name"))
            {
                p2Name = t.GetComponent<TMPro.TMP_Text>();
            }
            else if (t.CompareTag("UI Player"))
            {
                if (playerBGs[0] == null) playerBGs[0] = t.gameObject;
                else if (playerBGs[1] == null) playerBGs[1] = t.gameObject;
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
            case PlayerData.Device_Type.MB:
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
            case PlayerData.Device_Type.MB:
                players[1] = null;
                break;
        }
    }

    public void UpdateInfo()
    {
        bool isEmpty = players[0] == null && players[1] == null;

        emptyBG.GetComponent<Image>().enabled = isEmpty;
        emptyLabel.SetActive(isEmpty);

        teamNumber.SetActive(!isEmpty);
        playerBGs[0].SetActive(!isEmpty);
        playerBGs[1].SetActive(!isEmpty);

        if (players[0] != null)
        {
            p1Name.text = players[0].name;
        }
        else
        {
            p1Name.text = "";
        }

        if (players[1] != null)
        {
            p2Name.text = players[1].name;
        }
        else
        {
            p2Name.text = "";
        }
    }
}
