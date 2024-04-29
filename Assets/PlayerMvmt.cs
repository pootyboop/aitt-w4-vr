using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerMvmt : MonoBehaviour
{
    public SteamVR_Action_Vector2 action;
    Rigidbody rb;
    Gravity3D gravity;
    public Transform cam;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }



    void Update()
    {
        print(action.axis);
    }



    private void FixedUpdate()
    {
        //Move();
    }



    void Move()
    {
        Vector3 move = Vector3.zero;
        Vector3 input = new Vector3(action.axis.x, 0f, action.axis.y);

        move = new Vector3(input.x * transform.forward.x, transform.forward.y, input.z * transform.forward.z);
        rb.AddForce(move);
    }
}
