using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    bool inMenu = true;

    Vector3 playerSpawnpoint;
    Quaternion playerSpawnRot;

    public GameObject menuPlayer, gamePlayer;

    private void Start()
    {
        instance = this;
    }

    public void SetInMenu(bool newInMenu)
    {
        inMenu = newInMenu;
        menuPlayer.SetActive(inMenu);
        gamePlayer.SetActive(!inMenu);

        if (!inMenu && playerSpawnpoint == null)
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
