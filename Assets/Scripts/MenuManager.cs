using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    bool inMenu = true;

    Vector3 playerSpawnpoint = Vector3.zero;
    Quaternion playerSpawnRot;

    public GameObject menuPlayer, gamePlayer, menu;

    public SteamVR_Action_Single startLAction, startRAction;

    private void Start()
    {
        instance = this;
        SetInMenu(inMenu);
    }

    private void Update()
    {
        if (inMenu && startLAction.axis > 0.9f && startRAction.axis > 0.9f)
        {
            SetInMenu(false);
        }
    }

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
