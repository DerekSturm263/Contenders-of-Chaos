using UnityEngine;
using UnityEngine.InputSystem;

public class GamePlayerInfo : MonoBehaviour
{
    public static int playerNum = 0; // Evens are humans, odds are fairies.
    public GameObject[] players = new GameObject[8];

    public PlayerInput input;

    public LayerMask playerMask;
    public LayerMask fairyMask;

    public LayerMask ground;

    private void Awake()
    {
        StopAllCoroutines();

        var cam = Camera.main;
        
        if (playerNum % 2 == 0)
        {
            cam.cullingMask = playerMask;
        }
        else
        {
            cam.cullingMask = fairyMask;
        }
        
        for (int i = 0; i < 8; i++)
        {
            if (TeamsUpdater.teams[i / 2].GetPlayer(i % 2 == 0 ? 0 : 1) != null)
            {
                players[i].SetActive(true);

                if (i == playerNum)
                {
                    if (i % 2 == 0)
                    {
                        PlayerMovement newMove = players[i].AddComponent<PlayerMovement>();

                        input.SwitchCurrentActionMap("PC Player");
                        input.actions["Movement"].performed += ctx => newMove.Movement(ctx);
                        input.actions["Jump"].performed += ctx => newMove.Jump();
                        input.actions["Grab"].performed += ctx => newMove.Grab();

                        newMove.ground = ground;
                    }
                    else
                    {
                        FairyMovement newMove = players[i].AddComponent<FairyMovement>();

                        input.SwitchCurrentActionMap("Mobile Player");
                        input.actions["Tap Start"].performed += ctx => newMove.TapStart(ctx);
                        input.actions["Tap End"].performed += ctx => newMove.Move(ctx);
                    }
                }
                else
                {
                    if (i % 2 == 0)
                    {
                        PlayerCloudMovement cloudMov = players[i].AddComponent<PlayerCloudMovement>();
                        cloudMov.playerNum = i;
                    }
                    else
                    {
                        FairyCloudMovement cloudMov = players[i].AddComponent<FairyCloudMovement>();
                        cloudMov.playerNum = i;
                    }
                }
            }
        }
    }
}
