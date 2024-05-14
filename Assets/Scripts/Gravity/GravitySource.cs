using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GravitySource : MonoBehaviour
{
    public float sourceStrength = 9.81f;


    void OnTriggerEnter(Collider other)
    {
        Gravity3D currGravity3D = other.gameObject.GetComponent<Gravity3D>();
        if (currGravity3D != null)
        {
            currGravity3D.EnteredGravitySource(transform, sourceStrength);
        }
    }



    void OnTriggerExit(Collider other)
    {
        
        Gravity3D currGravity3D = other.gameObject.GetComponent<Gravity3D>();
        if (currGravity3D != null) {
            currGravity3D.LeftPlanetGravity();
        }
        
    }



    private void OnCollisionEnter(Collision collision)
    {
        Gravity3D currGravity3D = collision.gameObject.GetComponent<Gravity3D>();
        if (currGravity3D != null)
        {
            currGravity3D.ArrivedAtSource();
        }
    }
}
