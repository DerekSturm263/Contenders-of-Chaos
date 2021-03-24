using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedFeatherScript : MonoBehaviour {
    // Start is called before the first frame update
    private ItemAction itemAction;
    private float coolDown = 5;

    void Start() {
        itemAction = GetComponent<ItemAction>();
    }

    // Update is called once per frame
    void Update() {
        if (itemAction.gemState == ItemAction.State.Held) {
            StartCoroutine(SpeedyBoi(itemAction.pickupPlayer));
        }
    }

    IEnumerator SpeedyBoi(GameObject player) {
        itemAction.gemState = ItemAction.State.Floating;
        itemAction.pickupPlayer = null;
        transform.position = new Vector2(-1000f, -1000f);
        itemAction.inUse = true;
        //NOTE: NEED SPEED SET TO PUBLIC PRETTY PLZ
        //float temp = playerSpeed;
        //playerSpeed = 1.5 * temp;
        yield return new WaitForSeconds(coolDown);
        itemAction.inUse = false;
        //playerSpeed = temp;
    }
}
