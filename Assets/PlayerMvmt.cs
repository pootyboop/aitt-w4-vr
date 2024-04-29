using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerMvmt : MonoBehaviour
{
    [SerializeField]
    public SteamVR_Action_Vector2 action;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        print(action.axis);
    }
}
