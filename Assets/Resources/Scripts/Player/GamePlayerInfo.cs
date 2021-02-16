using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GamePlayerInfo : MonoBehaviour
{
    public static int timeSet = 300;
    public static int playerNum = 0; // Evens are humans, odds are fairies.

    private int _points;
    public float _timeRemaining;

    public static GameObject player;

    public int Points
    {
        get => _points;
        set
        {
            _points = value;
            UIController.UpdateTextObject(UIController.points, _points + " Points");
        }
    }

    public float TimeRemaining
    {
        get => _timeRemaining;
        set
        {
            _timeRemaining = value;
            UIController.UpdateTextObject(UIController.timeRemaining, (int) _timeRemaining / 60 + ":" + ((int) _timeRemaining % 60).ToString().PadLeft(2, '0'));

            if ((int)_timeRemaining == 0)
            {
                SceneManager.LoadScene("Results");
            }
        }
    }

    private bool countdown;

    public GameObject[] players = new GameObject[8];

    public PlayerInput input;

    public LayerMask playerMask;
    public LayerMask fairyMask;

    public LayerMask ground;

    private void Awake()
    {
        StopAllCoroutines();

        var cam = Camera.main;
        
        if (playerNum % 2 == 0)
        {
            cam.cullingMask = playerMask;
        }
        else
        {
            cam.cullingMask = fairyMask;
        }
        
        for (int i = 0; i < 8; i++)
        {
            if (TeamsUpdater.teams[i / 2].GetPlayer(i % 2 == 0 ? 0 : 1) != null)
            {
                players[i].SetActive(true);

                if (i == playerNum)
                {
                    if (i % 2 == 0)
                    {
                        PlayerMovement newMove = players[i].AddComponent<PlayerMovement>();
                        player = players[i];

                        input.SwitchCurrentActionMap("PC Player");
                        input.actions["Movement"].performed += ctx => newMove.Movement(ctx);
                        input.actions["Jump"].performed += ctx => newMove.Jump();
                        input.actions["Grab"].performed += ctx => newMove.Grab();

                        newMove.ground = ground;
                    }
                    else
                    {
                        FairyMovement newMove = players[i].AddComponent<FairyMovement>();
                        player = players[i];

                        input.SwitchCurrentActionMap("Mobile Player");
                        input.actions["Tap Start"].performed += ctx => newMove.TapStart(ctx);
                        input.actions["Tap End"].performed += ctx => newMove.Move(ctx);
                    }
                }
                else
                {
                    if (i % 2 == 0)
                    {
                        PlayerCloudMovement cloudMov = players[i].AddComponent<PlayerCloudMovement>();
                        cloudMov.playerNum = i;

                        cloudMov.ground = ground;
                    }
                    else
                    {
                        FairyCloudMovement cloudMov = players[i].AddComponent<FairyCloudMovement>();
                        cloudMov.playerNum = i;
                    }
                }
            }
        }

        StartGame();
    }

    private void Update()
    {
        if (!countdown)
            return;

        TimeRemaining -= Time.deltaTime;
    }

    public void StartGame()
    {
        TimeRemaining = timeSet;

        countdown = true;
    }

    public static GamePlayerInfo GetPlayerInfo()
    {
        return GameObject.FindObjectOfType<GamePlayerInfo>();
    }
}
