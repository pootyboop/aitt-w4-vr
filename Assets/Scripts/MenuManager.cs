using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

//handles transitioning between title screen and game
public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    bool inMenu = true; //currently in menu?

    Vector3 playerSpawnpoint = Vector3.zero;    //where to spawn gameplay player
    Quaternion playerSpawnRot;                  //rotation for ^

    public GameObject menuPlayer, gamePlayer, menu; //transforms to toggle

    public SteamVR_Action_Single startLAction, startRAction;



    private void Start()
    {
        instance = this;
        SetInMenu(inMenu);
    }



    //check for player trying to start game
    private void Update()
    {
        if (inMenu && startLAction.axis > 0.9f && startRAction.axis > 0.9f)
        {
            SetInMenu(false);
        }
    }



    //set the current inMenu state
    public void SetInMenu(bool newInMenu)
    {
        inMenu = newInMenu;
        menu.SetActive(inMenu);

        if (inMenu)
        {
            menuPlayer.SetActive(true);
            gamePlayer.SetActive(false);
        }

        else
        {
            menuPlayer.SetActive(false);
            gamePlayer.SetActive(true);

            //set gameplay player position, or record it to reuse later
            if (playerSpawnpoint == Vector3.zero)
            {
                playerSpawnpoint = gamePlayer.transform.position;
                playerSpawnRot = gamePlayer.transform.rotation;
            }

            else
            {
                gamePlayer.transform.position = playerSpawnpoint;
                gamePlayer.transform.rotation = playerSpawnRot;
            }
        }
    }
}
