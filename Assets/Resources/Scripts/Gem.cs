using UnityEngine;

public class Gem : MonoBehaviour
{
    public enum State
    {
        Floating, Held
    }
    public State gemState = State.Floating;

    private Float floatScript;
    public Transform holder;

    private void Awake()
    {
        floatScript = GetComponent<Float>();
    }

    private void Update()
    {
        if (gemState == State.Held)
        {
            transform.position = holder.position + new Vector3(0f, 0.5f);
        }
    }

    public void Grab()
    {
        floatScript.enabled = false;
        gemState = State.Held;
    }

    public void Drop()
    {
        floatScript.enabled = true;
        floatScript.ResetPosition();
        gemState = State.Floating;
    }
}
