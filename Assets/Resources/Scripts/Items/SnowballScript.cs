using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class SnowballScript : MonoBehaviour {
    // Start is called before the first frame update
    private ItemAction itemAction;
    public bool isTrapActive;
    public bool isFrozen;

    private const int coolDown = 10;

    void Start() {
        itemAction = GetComponent<ItemAction>();
        isTrapActive = false;
        isFrozen = false;
        StartCoroutine(ClearSnowballInfo());
        StartCoroutine(PullSnowballInfo());
    }

    // Update is called once per frame
    void Update() {
        if (itemAction.gemState == ItemAction.State.Held) {
            itemAction.inUse = true;
            if (Input.GetButton("Fire1")) {
                StartCoroutine(ThrowSnowball());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (isTrapActive) {
            isTrapActive = false;
            if (TryGetComponent(out Rigidbody2D rb2)) {
                Destroy(rb2);
            }
            itemAction.gemState = ItemAction.State.Floating;
            itemAction.pickupPlayer = null;
            transform.position = new Vector2(-1000f, -1000f);
            itemAction.inUse = false;
            if (collision.tag == "Player") StartCoroutine(FreezePlayer(collision.gameObject));
        }
    }

    IEnumerator ThrowSnowball() {
        Rigidbody2D rb2 = gameObject.AddComponent<Rigidbody2D>();
        rb2.freezeRotation = true;

        SpriteRenderer pickupPlayerSr = itemAction.pickupPlayer.GetComponent<SpriteRenderer>();
        itemAction.gemState = ItemAction.State.Floating;
        itemAction.pickupPlayer = null;

        if (pickupPlayerSr.flipX) {
            rb2.AddForce(new Vector2(-600, 150));
        }
        else {
            rb2.AddForce(new Vector2(600, 150));
        }
        yield return new WaitForSeconds(.1f);
        isTrapActive = true;
    }

    // Example on how to push.
    IEnumerator PushSnowballInfo(int playerNum, bool isFrozen) {
        // Create a form.
        WWWForm form2 = new WWWForm();
        form2.AddField("groupid", "pm36"); // Group ID.
        form2.AddField("grouppw", "N3Km3yJZpM"); // Password.
        form2.AddField("row", 590 + (CloudGameData.gameNum * 8) + playerNum); // Row you're pushing to.
        form2.AddField("s4", "" + isFrozen); // Value that you're pushing.

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form2)) // Uses the PushUrl.
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError) {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
        }
    }

    IEnumerator ClearSnowballInfo() {
        for (int i = 0; i < 8; i++) {
            // Create a form.
            WWWForm form2 = new WWWForm();
            form2.AddField("groupid", "pm36"); // Group ID.
            form2.AddField("grouppw", "N3Km3yJZpM"); // Password.
            form2.AddField("row", 590 + (CloudGameData.gameNum * 8) + i); // Row you're pushing to.
            form2.AddField("s4", "" + false); // Value that you're pushing.

            using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form2)) // Uses the PushUrl.
            {
                yield return webRequest.SendWebRequest();
                Debug.Log("Snowball cleared.");
                if (webRequest.isNetworkError) {
                    Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
                }
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    private IEnumerator FreezePlayer(GameObject player) {
        if (player.TryGetComponent(out PlayerMovement playerMovement)) {
            playerMovement.enabled = false;
            isFrozen = true;
            StartCoroutine(PushSnowballInfo(GamePlayerInfo.playerNum, true));
            yield return new WaitForSeconds(coolDown);
            playerMovement.enabled = true;
            isFrozen = false;
            StartCoroutine(PushSnowballInfo(GamePlayerInfo.playerNum, false));

        } else if (player.TryGetComponent(out FairyMovement fairyMovement)) {
            fairyMovement.enabled = false;
            isFrozen = true;
            StartCoroutine(PushSnowballInfo(GamePlayerInfo.playerNum, true));
            yield return new WaitForSeconds(coolDown);
            fairyMovement.enabled = true;
            isFrozen = false;
            StartCoroutine(PushSnowballInfo(GamePlayerInfo.playerNum, false));

        } else if (player.TryGetComponent(out PlayerCloudMovement playerCloudMovement)) {
            StartCoroutine(PushSnowballInfo(playerCloudMovement.playerNum, true));

        } else if (player.TryGetComponent(out FairyCloudMovement fairyCloudMovement)) {
            StartCoroutine(PushSnowballInfo(fairyCloudMovement.playerNum, true));
        }
    }

    // Example on how to pull.
    IEnumerator PullSnowballInfo() {
        int rowNum = 590 + (CloudGameData.gameNum * 8) + GamePlayerInfo.playerNum;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudGameData.PullURL + rowNum)) {
            yield return webRequest.SendWebRequest();
            //Debug.Log(webRequest.downloadHandler.text); 
            string result = webRequest.downloadHandler.text.Split(',')[1];
            try {
                if (bool.Parse(result) && !isFrozen) {
                    StartCoroutine(FreezePlayer(GamePlayerInfo.player));
                }
            }
            catch (Exception e) {
                Debug.LogWarning(e.Message);
            }
            yield return new WaitForSeconds(1);
            StartCoroutine(PullSnowballInfo());
            //Debug.Log("New text:" + result);
        }
    }
}