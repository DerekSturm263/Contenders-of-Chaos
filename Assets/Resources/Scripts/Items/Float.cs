using UnityEngine;

public class Float : MonoBehaviour
{
    private Vector2 downPos, upPos;

    private void Start()
    {
        ResetPosition();
    }

    public void ResetPosition()
    {
        downPos = transform.position - new Vector3(0f, 0.33f);
        upPos = transform.position + new Vector3(0f, 0.33f);
    }

    private void Update()
    {
        transform.position = Vector2.Lerp(downPos, upPos, (Mathf.Sin(Time.realtimeSinceStartup) + 1) / 2);
    }
}
