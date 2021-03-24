using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Linq;
using System;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{
    private EventSystem currentEvent;

    public TMPro.TMP_Text roomNumber;

    public GameObject codePrompt;
    public GameObject joinInfo;

    public TMPro.TMP_InputField codeInput;
    public TMPro.TMP_InputField usernameInput;

    public TMPro.TMP_Text enterCodePrompt;
    public GameObject quitPrompt;

    public GameObject hostGame;

    public GameObject mobileButton;
    public GameObject joinPrompt;
    public GameObject startGameButton;

    public GameObject fullscreenButton;
    public GameObject fullscreenTMP;

    public static TMPro.TMP_Text timeRemaining;
    public static TMPro.TMP_Text points;

    private bool failedHost = false;

    public bool resetCloudData = false;
    public bool testAsMobile = false;

    public TMPro.TMP_Text versionNumber;

    public GameObject firstTimeUsernameInput;

    public GameObject hostClosedGamePrompt;

    public TMPro.TMP_Text winnerText;
    public TMPro.TMP_Text[] teamPoints = new TMPro.TMP_Text[4];
    public GameObject[] teams = new GameObject[4];
    private int[] teamPointsArray = new int[4];
    private bool[] doneWithResults = new bool[4];

    public GameObject alertButton;

    public GameObject settingsButton;
    public GameObject gameSettingsBG;
    public GameObject leaveGameBG;

    // Settings.
    public static float volume = 0.5f;
    public static bool useFullscreen = true;
    public static bool useParticles = true;
    public static bool usePostProcessing = true;

    public static bool isPaused = false;

    public static bool readyToQuit = true;

    private void Awake()
    {
        currentEvent = FindObjectOfType<EventSystem>();

        if (resetCloudData)
        {
            Debug.Log("Clearing all cloud data.");

            StartCoroutine(CloudGameData.ClearGameData());
            StartCoroutine(CloudGameData.ClearCodeData());
            StartCoroutine(CloudGameData.ClearPlayerData());
            StartCoroutine(CloudGameData.ClearLocationData());
            StartCoroutine(CloudGameData.ClearStartedGameData());
            StartCoroutine(CloudGameData.ClearGemStates());
        }

        try
        {
            timeRemaining = GameObject.FindGameObjectWithTag("Time").GetComponent<TMPro.TMP_Text>();
            points = GameObject.FindGameObjectWithTag("Points").GetComponent<TMPro.TMP_Text>();
        } catch { }

        if (SceneManager.GetActiveScene().name.Equals("Results"))
        {
            UpdateResultsInfo();
        }
    }

    private void Start()
    {
        bool isPC = GameController.playerInfo.deviceType == PlayerData.Device_Type.PC;

        if (mobileButton != null)
        {
            mobileButton.SetActive(isPC);
            versionNumber.text = "V " + Application.version;
        }

        if (hostGame != null)
        {
            hostGame.SetActive(isPC);
        }

        if (!isPC && joinPrompt != null)
        {
            joinPrompt.SetActive(true);
        }

        if (fullscreenButton != null)
        {
            fullscreenButton.SetActive(isPC);
        }
        if (fullscreenTMP != null)
        {
            fullscreenTMP.SetActive(isPC);
        }
        
        if (usernameInput != null)
        {
            Debug.Log(GameController.playerInfo.name);
            usernameInput.text = GameController.playerInfo.name;
        }

        if (firstTimeUsernameInput != null && GameController.playerInfo.name == "")
        {
            firstTimeUsernameInput.SetActive(true);
            currentEvent.SetSelectedGameObject(firstTimeUsernameInput.GetComponentInChildren<TMPro.TMP_InputField>().gameObject);
        }

        if (alertButton != null)
        {
            alertButton.SetActive(!isPC);
        }

        if (settingsButton != null)
        {
            settingsButton.SetActive(CloudGameData.isHosting);
        }
    }

    public static UIController GetActiveController()
    {
        return GameObject.FindObjectOfType<UIController>();
    }

    public void UpdateRoomNumber(string roomNum)
    {
        roomNumber.text = "Room Code: " + roomNum;
    }

    public void HostGame()
    {
        StartCoroutine(Host());
    }

    public IEnumerator Host()
    {
        bool cancelHost = true;
        failedHost = false;

        joinInfo.gameObject.SetActive(true);
        joinInfo.GetComponentInChildren<TMPro.TMP_Text>().text = "Attempting to create a new room...";

        for (int i = 0; i < 10; ++i)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudGameData.PullURL + i))
            {
                yield return webRequest.SendWebRequest();

                string[] pages = i.ToString().Split('/');
                int page = pages.Length - 1;

                if (webRequest.isNetworkError)
                {
                    Debug.LogError("An error has occurred while pulling.\n" + webRequest.error);
                }
                else
                {
                    if (webRequest.downloadHandler.text.Contains("False"))
                    {
                        CloudGameData.isHosting = true;
                        CloudGameData.gameNum = i;
                        cancelHost = false;
                        break;
                    }
                }
            }
        }

        if (cancelHost)
        {
            failedHost = true;
            joinInfo.GetComponentInChildren<TMPro.TMP_Text>().text = "There aren't enough rooms available to host a new game. Please try again later.";
            yield return null;
        }
        else
        {
            WWWForm form = new WWWForm();
            form.AddField("groupid", "pm36");
            form.AddField("grouppw", "N3Km3yJZpM");
            form.AddField("row", CloudGameData.gameNum);
            form.AddField("s4", "True");

            using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
                }
            }

            WWWForm form3 = new WWWForm();
            form3.AddField("groupid", "pm36");
            form3.AddField("grouppw", "N3Km3yJZpM");
            form3.AddField("row", CloudGameData.gameNum + 220);
            form3.AddField("s4", "False");

            using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form3))
            {
                yield return webRequest.SendWebRequest();
            }

            int[] numbers = new int[6];
            string code = "";

            for (int i = 0; i < numbers.Length; ++i)
            {
                numbers[i] = UnityEngine.Random.Range(0, 10);
                code += numbers[i];
            }

            WWWForm form2 = new WWWForm();
            form2.AddField("groupid", "pm36");
            form2.AddField("grouppw", "N3Km3yJZpM");
            form2.AddField("row", CloudGameData.gameNum + 10);
            form2.AddField("s4", code);

            using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form2))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
                }
                else
                {
                    Debug.Log("New game successfully hosted at index: " + CloudGameData.gameNum + " with code: " + code);
                    CloudGameData.roomNum = code;
                }
            }

            for (int i = 0; i < 8; ++i)
            {
                WWWForm form4 = new WWWForm();
                form4.AddField("groupid", "pm36");
                form4.AddField("grouppw", "N3Km3yJZpM");
                form4.AddField("row", i + 10 * (CloudGameData.gameNum + 1) + 10);
                form4.AddField("s4", "E");

                using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form4))
                {
                    yield return webRequest.SendWebRequest();

                    if (webRequest.isNetworkError)
                    {
                        Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
                    }
                    else
                    {
                        Debug.Log("Player spot at index " + (i + 10 * (CloudGameData.gameNum + 1) + 10) + " has been cleared.");
                    }
                }
            }

            StartCoroutine(JoinGame(CloudGameData.gameNum));
        }
    }

    public void JoinGame()
    {
        DisplayCodePrompt();
    }

    private void DisplayCodePrompt()
    {
        codePrompt.SetActive(true);
        enterCodePrompt.text = "Please enter a room code. The code must be 6 digits in length and only consist of numbers.\n\nExample: 285193";
        codeInput.text = "";
    }

    public void HideCodePrompt()
    {
        codePrompt.GetComponent<Animator>().SetTrigger("Exit");
    }

    public void EnterCode()
    {
        string input = codeInput.text;

        if (input.Length == 6)
        {
            StartCoroutine(TryJoin(input));
        }
        else
        {
            enterCodePrompt.text = "Please enter the room code of the game you wish to enter.\n\nPlease type in a valid room code.";
        }
    }

    public void GoToPlay()
    {
        SceneManager.LoadScene("Host or Join Game");
    }

    private IEnumerator TryJoin(string input)
    {
        joinInfo.gameObject.SetActive(true);
        joinInfo.GetComponentInChildren<TMPro.TMP_Text>().text = "Attempting to join a game...";
        failedHost = false;
        bool cancelJoin = true;
        codePrompt.SetActive(false);

        int gameNum = -1;

        // Checks to see if any of the games matches the input code.
        for (int i = 10; i < 20; ++i)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudGameData.PullURL + i))
            {
                yield return webRequest.SendWebRequest();

                string[] pages = i.ToString().Split('/');
                int page = pages.Length - 1;

                if (webRequest.isNetworkError)
                {
                    Debug.LogError("An error has occurred while pulling.\n" + webRequest.error);
                }
                else
                {
                    if (webRequest.downloadHandler.text.Contains(input))
                    {
                        gameNum = i - 10;
                        break;
                    }
                }
            }
        }

        // Checks to see if the game with the code is open.
        using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudGameData.PullURL + gameNum))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = gameNum.ToString().Split('/');
            int page = pages.Length - 1;

            if (webRequest.downloadHandler.text.Contains("True"))
            {
                cancelJoin = false;
                CloudGameData.isHosting = false;
                StartCoroutine(JoinGame(gameNum));
            }
        }

        CloudGameData.roomNum = input;

        if (cancelJoin)
        {
            failedHost = true;
            joinInfo.GetComponentInChildren<TMPro.TMP_Text>().text = "There are no open rooms with that room code. Please try a different code.";
        }
    }

    public static int GetPlayerSlot(int gameNum, int playerNum)
    {
        return playerNum + 10 * (gameNum + 1) + 10;
    }

    public IEnumerator JoinGame(int gameIndex)
    {
        int teamNum = 0;
        bool cancelJoinTeam = true;

        // Checks through each of the open player slots to see if it's open.
        for (int i = 0; i < 8; ++i)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudGameData.PullURL + GetPlayerSlot(gameIndex, i)))
            {
                yield return webRequest.SendWebRequest();

                string[] pages = i.ToString().Split('/');
                int page = pages.Length - 1;

                // If the length is less than 5, the slot is open. The slot will be joined based on whether the player is on mobile or PC.
                if (webRequest.downloadHandler.text.Length < 5)
                {
                    if (GameController.playerInfo.deviceType == PlayerData.Device_Type.PC && i % 2 == 0)
                    {
                        Debug.Log("Player Index: " + GetPlayerSlot(gameIndex, i));

                        teamNum = i / 2;
                        GameController.playerInfo.teamNum = teamNum;
                        GamePlayerInfo.playerNum = i;
                        cancelJoinTeam = false;
                        break;
                    }
                    else if (GameController.playerInfo.deviceType == PlayerData.Device_Type.MB && i % 2 != 0)
                    {
                        cancelJoinTeam = false;
                        break;
                    }
                }
            }
        }

        if (cancelJoinTeam)
        {
            failedHost = true;
            joinInfo.GetComponentInChildren<TMPro.TMP_Text>().text = "There isn't any space in that game. Please try joining another game.";
            yield return null;
        }
        else
        {
            CloudGameData.gameNum = gameIndex;

            // Adds the player to the proper team and pushes the location the player should start at.
            if (GameController.playerInfo.deviceType == PlayerData.Device_Type.PC)
            {
                int rowIndex = TeamsUpdater.GetIndexOfPlayer(teamNum, 0, CloudGameData.gameNum);

                WWWForm form = new WWWForm();
                form.AddField("groupid", "pm36");
                form.AddField("grouppw", "N3Km3yJZpM");
                form.AddField("row", rowIndex);
                form.AddField("s4", "*PC*" + GameController.playerInfo.name);

                using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form))
                {
                    yield return webRequest.SendWebRequest();

                    if (webRequest.isNetworkError)
                    {
                        Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
                    }
                    else
                    {
                        Debug.Log("Player Index 2: " + rowIndex);

                        SceneManager.LoadScene("Team Select");
                    }
                }

                int locRowNum = TeamsUpdater.GetIndexOfPlayerPosition(teamNum, 0, CloudGameData.gameNum);
                Vector2 location;

                switch (teamNum)
                {
                    case 0:
                        location = new Vector2(-8f, 0f);
                        break;
                    case 1:
                        location = new Vector2(-5f, 0f);
                        break;
                    case 2:
                        location = new Vector2(5f, 0f);
                        break;
                    default:
                        location = new Vector2(8f, 0f);
                        break;
                }

                WWWForm form2 = new WWWForm();
                form2.AddField("groupid", "pm36");
                form2.AddField("grouppw", "N3Km3yJZpM");
                form2.AddField("row", locRowNum);
                form2.AddField("s4", location.x.ToString() + "|" + location.y.ToString());

                using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form2))
                {
                    yield return webRequest.SendWebRequest();

                    if (webRequest.isNetworkError)
                    {
                        Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
                    }
                }
            }
            else
            {
                SceneManager.LoadScene("Team Select");
            }
        }
    }

    public void QuitGame()
    {
        readyToQuit = false;

        if (CloudGameData.isHosting)
        {
            CloseGame();
        }
        else
        {
            LeaveGame();
        }
    }

    private void CloseGame()
    {
        StartCoroutine(CloseGameNetwork());
    }

    private IEnumerator CloseGameNetwork()
    {
        WWWForm form = new WWWForm();
        form.AddField("groupid", "pm36");
        form.AddField("grouppw", "N3Km3yJZpM");
        form.AddField("row", CloudGameData.gameNum);
        form.AddField("s4", "False");

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
            else
            {
                Debug.Log("Game succesfully closed.");
            }
        }

        readyToQuit = true;
        SceneManager.LoadScene("Host or Join Game");
    }

    private IEnumerator ClearPlayerData(int index)
    {
        WWWForm form2 = new WWWForm();
        form2.AddField("groupid", "pm36");
        form2.AddField("grouppw", "N3Km3yJZpM");
        form2.AddField("row", index + (10 * (CloudGameData.gameNum + 1) + 10));
        form2.AddField("s4", "E");

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form2))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
        }
    }

    public void CloseSettingsOrGame()
    {
        if (!gameSettingsBG.activeSelf)
        {
            OpenMenu(leaveGameBG);
        }
        else
        {
            CloseMenu(gameSettingsBG);
        }
    }

    public void LeaveToTitle()
    {
        if (codePrompt)
        {
            if (codePrompt.activeSelf)
            {
                HideCodePrompt();
            }
            else if (joinInfo.activeSelf)
            {
                if (failedHost)
                {
                    CloseMenu(joinInfo.gameObject);
                }
                else
                {
                    StopCoroutine(TryJoin(codeInput.text));
                    CloseMenu(joinInfo.gameObject);
                }
            }
            else if (!joinInfo.activeSelf)
            {
                SceneManager.LoadScene("Title");
            }
        }
        else
        {
            SceneManager.LoadScene("Host Or Join Game");
        }
    }

    public void JoinTeam(int teamNum)
    {
        if (GameController.playerInfo.teamNum != -1 || TeamsUpdater.teams[teamNum].GetPlayers()[1] != null)
            return;

        StartCoroutine(MobileJoinTeam(teamNum));
    }

    private IEnumerator MobileJoinTeam(int teamNum)
    {
        int rowIndex = TeamsUpdater.GetIndexOfPlayer(teamNum, 1, CloudGameData.gameNum);

        WWWForm form = new WWWForm();
        form.AddField("groupid", "pm36");
        form.AddField("grouppw", "N3Km3yJZpM");
        form.AddField("row", rowIndex);
        form.AddField("s4", "*MB*" + GameController.playerInfo.name);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
            else
            {
                Debug.Log("You (" + GameController.playerInfo.name + ") have succesfully joined Team " + teamNum + " at index: " + rowIndex);

                GameController.playerInfo.teamNum = teamNum;
                GamePlayerInfo.playerNum = teamNum * 2 + 1;
            }
        }

        int locRowNum = TeamsUpdater.GetIndexOfPlayerPosition(teamNum, 1, CloudGameData.gameNum);
        Vector2 location;

        switch (teamNum)
        {
            case 0:
                location = new Vector2(-7f, 1.5f);
                break;
            case 1:
                location = new Vector2(-4f, 1.5f);
                break;
            case 2:
                location = new Vector2(6f, 1.5f);
                break;
            default:
                location = new Vector2(9f, 1.5f);
                break;
        }

        WWWForm form2 = new WWWForm();
        form2.AddField("groupid", "pm36");
        form2.AddField("grouppw", "N3Km3yJZpM");
        form2.AddField("row", locRowNum);
        form2.AddField("s4", location.x.ToString() + "|" + location.y.ToString());

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form2))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
        }
    }

    private void LeaveGame()
    {
        try
        {
            StartCoroutine(LeaveTeam(GameController.playerInfo.teamNum));
        } catch { }
    }

    public IEnumerator LeaveTeam(int teamNum)
    {
        TeamsUpdater.teams[teamNum].RemovePlayer(GameController.playerInfo);
        GameController.playerInfo.teamNum = -1;

        WWWForm form = new WWWForm();
        form.AddField("groupid", "pm36");
        form.AddField("grouppw", "N3Km3yJZpM");
        form.AddField("row", TeamsUpdater.GetIndexOfPlayer(teamNum, (int) GameController.playerInfo.deviceType, CloudGameData.gameNum));
        form.AddField("s4", "*L*");

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
            else
            {
                Debug.Log("Team at index: " + (teamNum + (10 * (CloudGameData.gameNum + 1) + 10)) + " has been succesfully left.");
            }
        }

        readyToQuit = true;
        SceneManager.LoadScene("Host or Join Game");
    }

    public static int GetAlertRowNum(int teamNum, int gameNum)
    {
        return 250 + teamNum + gameNum * 10;
    }

    public void AlertTeammate()
    {
        StartCoroutine(PushAlert());
    }

    private IEnumerator PushAlert()
    {
        WWWForm form = new WWWForm();
        form.AddField("groupid", "pm36");
        form.AddField("grouppw", "N3Km3yJZpM");
        form.AddField("row", GetAlertRowNum(GamePlayerInfo.playerNum / 2, CloudGameData.gameNum));
        form.AddField("s4", "1");

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
            else
            {
                Debug.Log("Alert pushed at " + GetAlertRowNum(GamePlayerInfo.playerNum, CloudGameData.gameNum));
            }
        }
    }

    public void GoToSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void SettingsToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void OpenMenu(GameObject menu)
    {
        menu.SetActive(true);
    }

    public void CloseMenu(GameObject menu)
    {
        menu.GetComponent<Animator>().SetTrigger("Exit");
    }

    public void Quit(GameObject menu)
    {
        menu.GetComponent<Animator>().SetTrigger("Exit");
        Application.Quit();
    }

    public void GoToMobileApp()
    {
        SceneManager.LoadScene("Mobile App");
    }

    public void StartGame()
    {
        StartCoroutine(LoadMainScene());
    }

    private IEnumerator LoadMainScene()
    {
        WWWForm form = new WWWForm();
        form.AddField("groupid", "pm36");
        form.AddField("grouppw", "N3Km3yJZpM");
        form.AddField("row", CloudGameData.gameNum + 220);
        form.AddField("s4", "True");

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form))
        {
            yield return webRequest.SendWebRequest();
        }

        ChooseGameMode(CloudGameData.gameNum); // Change to button when you add more gamemodes.
        GamePlayerInfo.timeSet = 300;
        SceneManager.LoadScene("Main");
    }

    public void ChooseGameMode(int gameModeNum)
    {
        if (gameModeNum == 0)
        {
            if (CloudGameData.isHosting)
            {
                StartCoroutine(SendGemSeed());
                StartCoroutine(SendLevelSeed());
            }
            else
            {
                StartCoroutine(GetGemSeed());
                StartCoroutine(GetTileSeed());
            }
        }
        else if (gameModeNum == 1)
        {
            if (CloudGameData.isHosting)
            {

            }
            else
            {

            }
        }
        else
        {
            if (CloudGameData.isHosting)
            {

            }
            else
            {

            }
        }
    }

    public static IEnumerator SendGemSeed()
    {
        SpawnGems.seed = UnityEngine.Random.Range(0, 99999);

        WWWForm form = new WWWForm();
        form.AddField("groupid", "pm36");
        form.AddField("grouppw", "N3Km3yJZpM");
        form.AddField("row", CloudGameData.gameNum + 230);
        form.AddField("s4", SpawnGems.seed);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
            else
            {
                Debug.Log("Gem Seed: " + SpawnGems.seed);
            }
        }
    }

    private IEnumerator SendLevelSeed()
    {
        ProceduralTilemap.tileSeed = UnityEngine.Random.Range(0, 99999);

        WWWForm form = new WWWForm();
        form.AddField("groupid", "pm36");
        form.AddField("grouppw", "N3Km3yJZpM");
        form.AddField("row", CloudGameData.gameNum + 240);
        form.AddField("s4", ProceduralTilemap.tileSeed);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
            else
            {
                Debug.Log("Level Seed: " + ProceduralTilemap.tileSeed);
            }
        }
    }

    public static IEnumerator GetGemSeed()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudGameData.PullURL + (CloudGameData.gameNum + 230)))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("An error has occurred while pulling.");
            }
            else
            {
                SpawnGems.seed = int.Parse(webRequest.downloadHandler.text.Split(',')[1]);
                Debug.Log("Gem Seed: " + SpawnGems.seed);
            }
        }
    }

    private IEnumerator GetTileSeed()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudGameData.PullURL + (CloudGameData.gameNum + 240)))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("An error has occurred while pulling.");
            }
            else
            {
                ProceduralTilemap.tileSeed = int.Parse(webRequest.downloadHandler.text.Split(',')[1]);
                Debug.Log("Seed: " + ProceduralTilemap.tileSeed);
            }
        }
    }

    public static void UpdateTextObject(TMPro.TMP_Text text, string value)
    {
        text.text = value;
    }

    #region Settings

    public void EnterUserName(GameObject panel = null)
    {
        string input = usernameInput.text;

        if (input.Length < 1)
            return;

        GameController.playerInfo.name = input;

        Debug.Log("Username succesfully changed to " + input);
        SaveController.Save(input);

        if (panel != null)
            panel.GetComponent<Animator>().SetTrigger("Exit");
    }

    private void UpdateResultsInfo()
    {
        for (int i = 0; i < 4; ++i)
        {
            teams[i].SetActive(TeamsUpdater.teams[i].GetPlayers()[0] != null);

            if (teams[i].activeSelf)
            {
                StartCoroutine(GetResults(i));
            }
        }

        StartCoroutine(DisplayWinner());
    }

    private IEnumerator GetResults(int teamNum)
    {
        doneWithResults[teamNum] = false;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudGameData.PullURL + Gem.PointsRowNum(CloudGameData.gameNum, teamNum)))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("An error has occurred while pulling.");
            }
            else
            {
                teamPointsArray[teamNum] = int.Parse(webRequest.downloadHandler.text.Split(',')[1]);
                teamPoints[teamNum].text = teamPointsArray[teamNum] + " Points";
            }
        }

        doneWithResults[teamNum] = true;
    }

    private IEnumerator DisplayWinner()
    {
        for (int i = 0; i < Array.FindAll(teams, x => x.activeSelf).Length; ++i)
        {
            yield return new WaitUntil(() => doneWithResults[i]);
        }

        System.Collections.Generic.List<int> teamPointsList = teamPointsArray.ToList();
        teamPointsList.Sort();
        teamPointsList.Reverse();

        if (teamPointsList[0] != teamPointsList[1])
        {
            winnerText.text = "The Winner Is: " + TeamsUpdater.teams[Array.IndexOf(teamPointsArray, teamPointsList[0])].GetPlayers()[0].name + " & " + TeamsUpdater.teams[Array.IndexOf(teamPointsArray, teamPointsList[0])].GetPlayers()[1].name;
        }
        else
        {
            winnerText.text = "It's a Tie!";
        }
    }

    public void BackToTeamSelect()
    {
        SceneManager.LoadScene("Team Select");
    }

    public void AdjustVolume()
    {

    }

    public void ToggleFullscreen()
    {
        useFullscreen = !useFullscreen;
        Screen.fullScreen = useFullscreen;
    }

    public void ToggleParticles()
    {
        useParticles = !useParticles;
    }

    public void TogglePostProcessing()
    {
        usePostProcessing = !usePostProcessing;
    }

    #endregion

    private void OnApplicationQuit()
    {
        QuitGame();
        SaveController.Save(GameController.playerInfo.name);
    }
}
