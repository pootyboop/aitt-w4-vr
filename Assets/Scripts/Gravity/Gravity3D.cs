using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Gravity3D : MonoBehaviour
{
    Rigidbody rb;
    private bool useGravity = false;
    private bool parentedOnPlanet = false;
    Transform currGravitySource;
    float currGravityStrength = 1.0f;
    public float weight = 1.0f;

    public bool rotateTowardSource = false;
    public float rotateTowardSourceSpeed = 3f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb.useGravity) {
            rb.useGravity = false;
            useGravity = true;
        }

        if (useGravity)
        {
            //fires OnTriggerEnter
            transform.position = new Vector3 (transform.position.x + 0.001f, transform.position.y, transform.position.z);
        }
    }



    public void EnteredGravitySource(Transform newSource, float newStrength)
    {
        transform.SetParent(null);
        currGravitySource = newSource;
        currGravityStrength = newStrength;
    }



    public void ArrivedAtSource()
    {
        if (parentedOnPlanet)
        {
            return;
        }

        parentedOnPlanet = true;

        transform.SetParent(currGravitySource);
    }


    private void FixedUpdate()
    {
        AddForceTowardSource();
        RotateTowardSource();
    }


    
    void AddForceTowardSource()
    {
        Vector3 directionToSource = getDirectionToSource();

        Vector3 force = directionToSource * currGravityStrength * rb.mass * weight;
        rb.AddForce(force);
    }



    public void RotateTowardSource()
    {
        if (rotateTowardSource)
        {
            Vector3 directionToSource = getDirectionToSource();

            //
            //https://www.youtube.com/watch?v=v4gkheo0dt8
            //

            Quaternion orientationDirection = Quaternion.FromToRotation(-transform.up, directionToSource) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, orientationDirection, rotateTowardSourceSpeed * Time.fixedDeltaTime);
            //transform.rotation = orientationDirection;
        }
    }


    public Vector3 getDirectionToSource()
    {
        if (currGravitySource != null) {
            return (currGravitySource.position - transform.position).normalized;
        }

        return Vector3.zero;
    }

}