using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShieldScript : MonoBehaviour {
    private ItemAction itemAction;

    // Start is called before the first frame update
    void Start() {
        itemAction = GetComponent<ItemAction>();
    }

    // Update is called once per frame
    void Update() {
        if (itemAction.gemState == ItemAction.State.Held) {
            itemAction.inUse = true;
        }
    }
}