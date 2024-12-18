﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using System.Collections;

public class GamePlayerInfo : MonoBehaviour
{
    public static int timeSet = 300;
    public static int playerNum = 0; // Evens are humans, odds are fairies.

    private int _points;
    private float _timeRemaining;

    public static GameObject player;

    public static int[] teamPoints = new int[8];

    public static bool useRandomChallenge = true;
    public static bool useHazards = true;

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
    public static Transform[] playerTransforms = new Transform[8];

    public PlayerInput input;

    public LayerMask playerMask;
    public LayerMask fairyMask;

    public LayerMask ground;

    private void Awake()
    {
        MusicPlayer.Restart();

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
            if (TeamsUpdater.teams[i / 2].GetPlayers()[i % 2 == 0 ? 0 : 1] != null)
            {
                players[i].SetActive(true);
                playerTransforms[i] = players[i].transform;

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
                        StartCoroutine(PushPoints(playerNum / 2));
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
                        players[i].GetComponent<CapsuleCollider2D>().enabled = false;
                        players[i].GetComponent<Rigidbody2D>().gravityScale = 0f;

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

        if (playerNum % 2 != 0)
        {
            StartCoroutine(PullPoints());
        }
    }

    private void Update()
    {
        if (!countdown)
            return;

        teamPoints[0] = _points;
        TimeRemaining -= Time.deltaTime;
    }

    public void StartGame()
    {
        TimeRemaining = timeSet;

        countdown = true;
    }

    public static GamePlayerInfo GetPlayerInfo()
    {
        return FindObjectOfType<GamePlayerInfo>();
    }

    private IEnumerator PullPoints()
    {
        int rowNum = Gem.PointsRowNum(CloudGameData.gameNum, playerNum / 2);

        using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudGameData.PullURL + rowNum))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = rowNum.ToString().Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pulling.\n" + webRequest.error);
            }
            else
            {
                Points = int.Parse(webRequest.downloadHandler.text.Split(',')[1]);
            }
        }

        StartCoroutine(PullPoints());
    }

    private IEnumerator PushPoints(int num)
    {
        int rowNum = Gem.PointsRowNum(CloudGameData.gameNum, num);

        WWWForm form = new WWWForm();
        form.AddField("groupid", "pm36");
        form.AddField("grouppw", "N3Km3yJZpM");
        form.AddField("row", rowNum);
        form.AddField("s4", 0);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
        }
    }
}
