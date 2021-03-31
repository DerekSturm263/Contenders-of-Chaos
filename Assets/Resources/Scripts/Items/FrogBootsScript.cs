using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogBootsScript : MonoBehaviour {
    // Start is called before the first frame update
    private ItemAction itemAction;
    public static float coolDown = 5;

    void Start() {
        itemAction = GetComponent<ItemAction>();
    }

    // Update is called once per frame
    void Update() {
        if (itemAction.gemState == ItemAction.State.Held) {
            StartCoroutine(JumpingBoi(itemAction.pickupPlayer));
        }
    }

    IEnumerator JumpingBoi(GameObject player) {
        if (player.TryGetComponent(out PlayerMovement playerMovement)) {
            itemAction.gemState = ItemAction.State.Floating;
            itemAction.pickupPlayer = null;
            transform.position = new Vector2(-1000f, -1000f);
            itemAction.inUse = true; 

            float temp = playerMovement.jumpSpeed;
            playerMovement.jumpSpeed = 2 * temp;
            yield return new WaitForSeconds(coolDown);
            itemAction.inUse = false;
            playerMovement.jumpSpeed = temp;

        } else {
            yield return null;
        }
    }
}
