using UnityEngine;

public class FairyMovement : MonoBehaviour
{
    private void Update()
    {
        transform.position = Input.GetTouch(0).position;
    }
}
