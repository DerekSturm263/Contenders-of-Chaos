using UnityEngine;
using System.Linq;

public class ItemAction : MonoBehaviour
{
    public enum State
    {
        Floating, Held
    }
    public State gemState = State.Floating;

    private Float floatScript;

    public bool canCarry;
    public System.Action itemAction;

    public GameObject pickupPlayer;

    public static System.Collections.Generic.List<ItemAction> items = new System.Collections.Generic.List<ItemAction>();

    private void Awake()
    {
        items.Add(this);
    }

    private void Update()
    {
        if (gemState == State.Held)
        {
            transform.position = pickupPlayer.transform.position + new Vector3(0f, 0.5f);
        }
    }

    public void Grab()
    {
        floatScript.enabled = false;
        gemState = State.Held;
    }

    public void Spawn()
    {
        ChoosePosition();
    }

    private Vector2 ChoosePosition()
    {
        System.Collections.Generic.List<int> teamPointsList = GamePlayerInfo.teamPoints.ToList();
        teamPointsList.Sort();
        teamPointsList.Reverse();

        return GamePlayerInfo.playerTransforms[System.Array.IndexOf(GamePlayerInfo.playerTransforms, teamPointsList[0])].position + new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
    }
}
