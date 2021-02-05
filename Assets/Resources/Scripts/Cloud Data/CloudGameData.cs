﻿using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

public static class CloudGameData
{
    public const string PullURL = "http://vgdapi.basmati.org/gets4.php?groupid=pm36&row=";
    public const string PushURL = "http://vgdapi.basmati.org/mods4.php";

    public static bool isHosting = false;
    public static string inputRoomCode = "000000";
    public static int gameNum;

    public static string roomNum = "000000";

    public static IEnumerator ResetAllCloudData()
    {
        Debug.Log("Erasing all information from the cloud...");

        for (int i = 0; i < 10; ++i)
        {
            WWWForm form = new WWWForm();
            form.AddField("groupid", "pm36");
            form.AddField("grouppw", "N3Km3yJZpM");
            form.AddField("row", i);
            form.AddField("s4", "False");

            using (UnityWebRequest webRequest = UnityWebRequest.Post(PushURL, form))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
                }
            }
        }

        for (int i = 10; i < 20; ++i)
        {
            WWWForm form = new WWWForm();
            form.AddField("groupid", "pm36");
            form.AddField("grouppw", "N3Km3yJZpM");
            form.AddField("row", i);
            form.AddField("s4", "000000");

            using (UnityWebRequest webRequest = UnityWebRequest.Post(PushURL, form))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
                }
            }
        }

        for (int i = 20; i < 118; ++i)
        {
            WWWForm form = new WWWForm();
            form.AddField("groupid", "pm36");
            form.AddField("grouppw", "N3Km3yJZpM");
            form.AddField("row", i);
            form.AddField("s4", "E");

            using (UnityWebRequest webRequest = UnityWebRequest.Post(PushURL, form))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
                }
            }
        }

        Debug.Log("Server reset complete!");
    }
}
