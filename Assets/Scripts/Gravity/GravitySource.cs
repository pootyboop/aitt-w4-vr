using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//source of gravity. used by planets
[RequireComponent(typeof(Collider))]
public class GravitySource : MonoBehaviour
{
    public float sourceStrength = 9.81f; //how strong gravity is. earth gravity by default


    //something entered the gravity collider
    void OnTriggerEnter(Collider other)
    {
        Gravity3D currGravity3D = other.gameObject.GetComponent<Gravity3D>();
        if (currGravity3D != null)
        {
            //tell the object it entered this gravity source
            currGravity3D.EnteredGravitySource(transform, sourceStrength);
        }
    }



    //something exited the gravity collider
    void OnTriggerExit(Collider other)
    {
        
        Gravity3D currGravity3D = other.gameObject.GetComponent<Gravity3D>();
        if (currGravity3D != null)
        {
            //tell the object it exited this gravity source
            currGravity3D.LeftPlanetGravity();
        }
        
    }
}
