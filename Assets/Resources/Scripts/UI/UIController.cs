using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class UIController : MonoBehaviour
{
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

    private bool failedHost = false;

    public bool resetCloudData = false;
    public bool testAsMobile = false;

    // Settings.
    public static float volume = 0.5f;
    public static bool useFullscreen = true;
    public static bool useParticles = true;
    public static bool usePostProcessing = true;

    private void Awake()
    {
        if (resetCloudData)
        {
            Debug.Log("Clearing all cloud data.");

            StartCoroutine(CloudGameData.ClearGameData());
            StartCoroutine(CloudGameData.ClearCodeData());
            StartCoroutine(CloudGameData.ClearPlayerData());
            StartCoroutine(CloudGameData.ClearLocationData());
            StartCoroutine(CloudGameData.ClearStartedGameData());
        }
    }

    private void Start()
    {
        bool isPC = GameController.playerInfo.deviceType == PlayerData.Device_Type.PC;

        if (mobileButton != null)
        {
            mobileButton.SetActive(isPC);
        }

        if (hostGame != null)
        {
            hostGame.SetActive(isPC);
        }

        if (!isPC && joinPrompt != null)
        {
            joinPrompt.SetActive(true);
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
        enterCodePrompt.text = "Please enter the room code of the game you wish to enter.\n\nExample: 123456";
        codeInput.text = "";
    }

    public void HideCodePrompt()
    {
        codePrompt.SetActive(false);
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

    public IEnumerator JoinGame(int gameIndex)
    {
        int teamNum = 0;
        bool cancelJoinTeam = true;

        for (int i = 0; i < 8; ++i)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudGameData.PullURL + (i + (10 * (CloudGameData.gameNum + 1) + 10))))
            {
                yield return webRequest.SendWebRequest();

                string[] pages = i.ToString().Split('/');
                int page = pages.Length - 1;

                if (webRequest.downloadHandler.text.Length < 5)
                {
                    if (GameController.playerInfo.deviceType == PlayerData.Device_Type.PC && i % 2 == 0)
                    {
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

            if (GameController.playerInfo.deviceType == PlayerData.Device_Type.PC)
            {
                int rowIndex = TeamsUpdater.GetIndexOfPlayer(teamNum, (int)GameController.playerInfo.deviceType, CloudGameData.gameNum);
                string addition = (GameController.playerInfo.deviceType == PlayerData.Device_Type.PC) ? "*PC*" : "*MB*";

                WWWForm form = new WWWForm();
                form.AddField("groupid", "pm36");
                form.AddField("grouppw", "N3Km3yJZpM");
                form.AddField("row", rowIndex);
                form.AddField("s4", addition + GameController.playerInfo.name);

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

                        SceneManager.LoadScene("Team Select");
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
        }

        WWWForm form3 = new WWWForm();
        form3.AddField("groupid", "pm36");
        form3.AddField("grouppw", "N3Km3yJZpM");
        form3.AddField("row", CloudGameData.gameNum + 10);
        form3.AddField("s4", "000000");

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form3))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
        }

        for (int i = 0; i < 8; ++i)
        {
            StartCoroutine(ClearPlayerData(i));
        }

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

    public void LeaveToTitle()
    {
        if (codePrompt.activeSelf)
        {
            HideCodePrompt();
        }
        else if (joinInfo.activeSelf && failedHost)
        {
            CloseMenu(joinInfo.gameObject);
        }
        else if (!joinInfo.activeSelf)
        {
            SceneManager.LoadScene("Title");
        }
    }

    public void JoinTeam(int teamNum)
    {
        if (GameController.playerInfo.teamNum != -1 || TeamsUpdater.teams[teamNum].GetPlayer(1) != null)
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
    }

    private void LeaveGame()
    {
        try
        {
            StartCoroutine(LeaveTeam(TeamsUpdater.teams[GameController.playerInfo.teamNum], GameController.playerInfo.teamNum));
        } catch { }
    }

    public IEnumerator LeaveTeam(Team team, int teamNum)
    {
        team.RemovePlayer(GameController.playerInfo);
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
                Debug.Log("Team at index: " + teamNum + (10 * (CloudGameData.gameNum + 1) + 10) + " has been succesfully left.");
            }
        }

        SceneManager.LoadScene("Host or Join Game");
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
        menu.SetActive(false);
    }

    public void Quit()
    {
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

        SceneManager.LoadScene("Main");
    }

    public void ChooseGameMode(int gameModeNum)
    {
        if (CloudGameData.isHosting)
        {
            StartCoroutine(SendGemSeed());
        }
        else
        {
            StartCoroutine(GetGemSeed());
        }
    }

    private IEnumerator SendGemSeed()
    {
        SpawnGems.seed = UnityEngine.Random.Range(0, 99999);

        WWWForm form = new WWWForm();
        form.AddField("groupid", "pm36");
        form.AddField("grouppw", "N3Km3yJZpM");
        form.AddField("row", CloudGameData.gameNum + 230);
        form.AddField("s4", SpawnGems.seed);

        using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudGameData.PushURL))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
            else
            {
                Debug.Log("Seed " + SpawnGems.seed + " succesfully pushed.");
            }
        }
    }

    private IEnumerator GetGemSeed()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudGameData.PullURL + CloudGameData.gameNum + 230))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("An error has occurred while pulling.");
            }
            else
            {
                SpawnGems.seed = int.Parse(webRequest.downloadHandler.text.Split(',')[1]);
            }
        }
    }

    #region Settings

    public void EnterUserName()
    {
        string input = usernameInput.text;

        GameController.playerInfo.name = input;
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
        //Application.wantsToQuit += () => false;
        QuitGame();
        //Application.Quit();
    }
}
