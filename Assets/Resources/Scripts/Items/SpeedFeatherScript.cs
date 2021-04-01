using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpeedFeatherScript : MonoBehaviour {
    // Start is called before the first frame update
    private ItemAction itemAction;
    private int rowNum;
    public static float coolDown = 5;

    void Start() {
        itemAction = GetComponent<ItemAction>();
        //I NEEED TEAM NUMB PLZ
        rowNum = 550 + (4 * CloudGameData.gameNum);
    }

    // Update is called once per frame
    void Update() {
        if (itemAction.gemState == ItemAction.State.Held) {
            StartCoroutine(SpeedyBoi(itemAction.pickupPlayer));
        }
    }

    IEnumerator SpeedyBoi(GameObject player) {
        StartCoroutine(ChangeBool(true));
        itemAction.gemState = ItemAction.State.Floating;
        itemAction.pickupPlayer = null;
        transform.position = new Vector2(-1000f, -1000f);

        if (player.TryGetComponent(out PlayerMovement playerMovement)) {
            itemAction.inUse = true;

            float walkTemp = playerMovement.walkSpeed;
            float runTemp = playerMovement.runSpeed;

            playerMovement.runSpeed = 1.5f * runTemp;
            playerMovement.walkSpeed = 1.5f * walkTemp;

            yield return new WaitForSeconds(coolDown);

            itemAction.inUse = false;
            playerMovement.runSpeed = runTemp;
            playerMovement.walkSpeed = walkTemp;
            StartCoroutine(ChangeBool(false));

        } else if (player.TryGetComponent(out FairyMovement fairyMovement)) {
                itemAction.inUse = true;

                float temp = fairyMovement.speed;
                fairyMovement.speed = 1.5f * temp;

                yield return new WaitForSeconds(coolDown);
                itemAction.inUse = false;
                fairyMovement.speed = temp;
                StartCoroutine(ChangeBool(false));
        } else {
            yield return null;
        }
    }

    IEnumerator ResetBools() {
        for (int i = 0; i < 10; i++) {
            // Create a form.
            WWWForm form2 = new WWWForm();
            form2.AddField("groupid", "pm36"); // Group ID.
            form2.AddField("grouppw", "N3Km3yJZpM"); // Password.
            form2.AddField("row", 550 + CloudGameData.gameNum + i); // Row you're pushing to.
            form2.AddField("s4", "False"); // Value that you're pushing.

            using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form2)) // Uses the PushUrl.
            {
                yield return webRequest.SendWebRequest();
                Debug.Log("Speed feather cleared.");
                if (webRequest.isNetworkError) {
                    Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
                }
            }
            yield return new WaitForSeconds(.1f);
        }
    }
    IEnumerator ChangeBool(bool value) {
        // Create a form.
        WWWForm form2 = new WWWForm();
        form2.AddField("groupid", "pm36"); // Group ID.
        form2.AddField("grouppw", "N3Km3yJZpM"); // Password.
        form2.AddField("row", rowNum); // Row you're pushing to.
        form2.AddField("s4", "" + value); // Value that you're pushing.

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form2)) // Uses the PushUrl.
        {
            yield return webRequest.SendWebRequest();
            Debug.Log("Fake Gem position cleared.");
            if (webRequest.isNetworkError) {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
        }
    }
    IEnumerator PullBools() {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudGameData.PullURL + rowNum)) {
            yield return webRequest.SendWebRequest();
            //Debug.Log(webRequest.downloadHandler.text); 
            string result = webRequest.downloadHandler.text;
            try {
                if (bool.Parse(result)) {
                    StartCoroutine(SpeedyBoi(null));
                }
            }
            catch (Exception e) {
                Debug.LogWarning(e.Message);
            }
            yield return new WaitForSeconds(1);
            StartCoroutine(PullBools());
            //Debug.Log("New text:" + result);
        }
    }
}
