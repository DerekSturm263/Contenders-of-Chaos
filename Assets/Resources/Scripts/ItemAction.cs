﻿using UnityEngine;
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
    public bool inUse = false; //currently not in use and can be effected by spawn()
    public System.Action itemAction;

    public GameObject pickupPlayer;

    public static System.Collections.Generic.List<ItemAction> items = new System.Collections.Generic.List<ItemAction>();

    private void Awake()
    {
        items.Add(this);
        floatScript = GetComponent<Float>();
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

    public void Use()
    {

    }

    public void Spawn()
    {
        transform.position = ChoosePosition();
        floatScript.ResetPosition();
    }

    private Vector2 ChoosePosition()
    {
        return new Vector3(0f, 0f, 1f);

        //System.Collections.Generic.List<int> teamPointsList = GamePlayerInfo.teamPoints.ToList();
        //teamPointsList.Sort();
        //teamPointsList.Reverse();

        //return GamePlayerInfo.playerTransforms[System.Array.IndexOf(GamePlayerInfo.playerTransforms, teamPointsList[0])].position + new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
    }
}
