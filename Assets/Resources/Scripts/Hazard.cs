using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{

    CharacterController fpsinput;
    public float power;

    private void Awake()
    {
        fpsinput = GameObject.Find("PlayerMovement").GetComponent<CharacterController>();
    }

    private void OnCollisionEnter2D(Collision2D collisionData)
    {
        if (collisionData.gameObject.tag == "Player")
        {
            fpsinput.movement.velocity.y = 0;
            fpsinput.movement.velocity = fpsinput.movement.velocity + transform.up * power;
        }
    }

}
