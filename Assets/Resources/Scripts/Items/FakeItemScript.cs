using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class FakeItemScript : MonoBehaviour {
    // Start is called before the first frame update
    private Rigidbody2D rb2;
    private ItemAction itemAction;
    private Float floatScript;
    public bool isTrapActive;

void Start() {
        itemAction = GetComponent<ItemAction>();
        floatScript = GetComponent<Float>();
        isTrapActive = false;
        StartCoroutine(ClearFakeGemInfo());
        StartCoroutine(PullFakeGemInfo());
    }

    // Update is called once per frame
    void Update() {
        if (itemAction.gemState == ItemAction.State.Held) {
            itemAction.inUse = true;
            if (Input.GetButton("Fire1") && !isTrapActive) {
                StartCoroutine(ThrowItem());
            }
            if (isTrapActive) {
                GamePlayerInfo.GetPlayerInfo().Points--;
                StartCoroutine(ClearFakeGemInfo());
                //TODO: add some cool effects
            }
        }
    }

    IEnumerator ThrowItem() {
        rb2 = gameObject.AddComponent<Rigidbody2D>();
        rb2.freezeRotation = true;

        SpriteRenderer pickupPlayerSr = itemAction.pickupPlayer.GetComponent<SpriteRenderer>();
        if (pickupPlayerSr.flipX) {
            rb2.AddForce(new Vector2(-300, 150));
        }
        else {
            rb2.AddForce(new Vector2(300, 150));
        }

        itemAction.gemState = ItemAction.State.Floating;
        itemAction.pickupPlayer = null;
        yield return new WaitForSeconds(.1f);

        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        yield return new WaitForSeconds(.5f);

        rb2.velocity = Vector2.zero;
        Destroy(rb2);
        Destroy(collider);
        //send data about transform, in use, and isTrapActive
        StartCoroutine(PushFakeGemInfo());
    }

    // Example on how to push.
    IEnumerator PushFakeGemInfo() {
        // Create a form.
        WWWForm form2 = new WWWForm();
        form2.AddField("groupid", "pm36"); // Group ID.
        form2.AddField("grouppw", "N3Km3yJZpM"); // Password.
        form2.AddField("row", 540 + CloudGameData.gameNum); // Row you're pushing to.

        Vector2 pos = transform.position;
        isTrapActive = true;
        form2.AddField("s4", pos.x + "|" + pos.y + "|" + isTrapActive); // Value that you're pushing.

        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form2)) // Uses the PushUrl.
        {
            yield return webRequest.SendWebRequest();
            
            if (webRequest.isNetworkError) {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
        }
    }

    IEnumerator ClearFakeGemInfo() {
        // Create a form.
        WWWForm form2 = new WWWForm();
        form2.AddField("groupid", "pm36"); // Group ID.
        form2.AddField("grouppw", "N3Km3yJZpM"); // Password.
        form2.AddField("row", 540 + CloudGameData.gameNum); // Row you're pushing to.
        form2.AddField("s4", "-1000|-1000|false|true"); // Value that you're pushing.

        isTrapActive = false;
        floatScript.enabled = false;
        itemAction.inUse = false;
        transform.position = new Vector2(-1000f, -1000f);
        itemAction.inUse = false;
        itemAction.gemState = ItemAction.State.Floating;
        itemAction.pickupPlayer = null;
        
        using (UnityWebRequest webRequest = UnityWebRequest.Post(CloudGameData.PushURL, form2)) // Uses the PushUrl.
        {
            yield return webRequest.SendWebRequest();
            Debug.Log("Fake Gem position cleared.");
            if (webRequest.isNetworkError) {
                Debug.LogError("An error has occurred while pushing.\n" + webRequest.error);
            }
        }
    }

    // Example on how to pull.
    IEnumerator PullFakeGemInfo() {
        yield return new WaitForSeconds(1);
        while (true) {
            yield return new WaitForSeconds(1);
            int rowNum = 540 + CloudGameData.gameNum;

            using (UnityWebRequest webRequest = UnityWebRequest.Get(CloudGameData.PullURL + rowNum)) {
                yield return webRequest.SendWebRequest();

                //Debug.Log(webRequest.downloadHandler.text); 
                string result = webRequest.downloadHandler.text.Split(',')[1];
                try {
                    string[] data = result.Split('|');
                    Vector2 pos = new Vector2(float.Parse(data[0]), float.Parse(data[1]));
                    //Debug.Log(bool.Parse(data[2]));
                    if (bool.Parse(data[2])) {
                        itemAction.gemState = ItemAction.State.Floating;
                        itemAction.pickupPlayer = null;
                        isTrapActive = true;
                        itemAction.inUse = true;
                        transform.position = pos;
                        floatScript.ResetPosition();
                        floatScript.enabled = true;
                    } else if (isTrapActive) {
                        //Someone has already sprung the trap and the game needs to update accordingly
                        Debug.Log("this is a test.");
                        StartCoroutine(ClearFakeGemInfo());
                    }
                } catch (Exception e) {
                    Debug.LogWarning(e.Message);
                }
                //Debug.Log("New text:" + result);
            }
        }
    }
}
