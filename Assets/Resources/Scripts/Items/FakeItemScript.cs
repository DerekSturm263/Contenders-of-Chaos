using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeItemScript : MonoBehaviour {
    // Start is called before the first frame update
    private Rigidbody2D rb2;
    private ItemAction itemAction;
    private Float floatScript;
    private bool isTrapActive;

void Start() {
        itemAction = GetComponent<ItemAction>();
        floatScript = GetComponent<Float>();
        isTrapActive = false;
    }

    // Update is called once per frame
    void Update() {
        if (itemAction.gemState == ItemAction.State.Held) {
            if (Input.GetButton("Fire1") && !isTrapActive) {
                StartCoroutine(throwItem());
            }
            if (isTrapActive) {
                Debug.Log("haha trap go brr");
                Debug.Log(itemAction.pickupPlayer);
                GamePlayerInfo.GetPlayerInfo().Points--;

                itemAction.gemState = ItemAction.State.Floating;
                itemAction.pickupPlayer = null;

                isTrapActive = false;
                transform.position = new Vector2(-1000f, -1000f);
                //TODO: add some cool effects
            }
        }
        //add the ability to throw a gem
        //Make sure the other players can see it (navigator? leader?)
        //when they touch it, make them lose points?
        IEnumerator throwItem() {
            rb2 = gameObject.AddComponent<Rigidbody2D>();
            rb2.freezeRotation = true;
            rb2.AddForce(new Vector2(300, 150));

            itemAction.gemState = ItemAction.State.Floating;
            itemAction.pickupPlayer = null;
            yield return new WaitForSeconds(.1f);

            isTrapActive = true;
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            yield return new WaitForSeconds(.5f);

            rb2.velocity = Vector2.zero;
            Destroy(rb2);
            Destroy(collider);

            floatScript.ResetPosition();
            floatScript.enabled = true;
        }
    }
}
