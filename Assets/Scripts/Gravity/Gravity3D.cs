using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

/**
* This script includes two lines from a YouTube tutorial.
*
* Author: Bit Galaxis
* Location: https://youtu.be/v4gkheo0dt8?si=iv0_cxPh2iBdbwCP&t=1256
* Accessed: 29/04/2024
*/

//use with rigidbody to 3D-ify gravity using GravitySources
[RequireComponent(typeof(Rigidbody))]
public class Gravity3D : MonoBehaviour
{
    public enum ERotationType {
        NONE,   //do not rotate toward gravity source
        ROTATETOSOURCE, //smoothly rotate to gravity source
        SNAPTOSOURCE    //immediately snap rotation to gravity source
    }

    Rigidbody rb;
    bool useGravity = false;    //toggle for gravity
    bool parentedOnPlanet = false;  //in proximity of planet and parented
    public bool standingOnPlanet = false;   //grounded on the gravity source planet?
    Transform currGravitySource;    //source of gravity this object is bound to
    float currGravityStrength = 1.0f;   //gravity strength of the current source
    public float weight = 1.0f; //gravity multiplier

    public ERotationType rotationType = ERotationType.NONE;
    public float rotateTowardSourceSpeed = 3f;  //speed of smooth rotating if rotationType is ROTATETOSOURCE

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        //set useGravity from the rb
        if (rb.useGravity) {
            rb.useGravity = false;
            useGravity = true;
        }

        if (useGravity)
        {
            //fires OnTriggerEnter events when spawning inside a gravity source
            transform.position = new Vector3 (transform.position.x + 0.001f, transform.position.y, transform.position.z);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        //landed on planet
        if (!parentedOnPlanet && other.gameObject.CompareTag("PlanetCollision"))
        {
            ArrivedAtSource();
        }
    }

    private void OnCollisionExit(Collision other)
    {
        //not on planet's surface, but still in its gravity
        if (standingOnPlanet && other.gameObject.CompareTag("PlanetCollision"))
        {
            LeftFromSource();
        }
    }



    //entered a planet's gravity, hasn't landed yet
    public void EnteredGravitySource(Transform newSource, float newStrength)
    {
        if (!useGravity) {
            return;
        }

        if (transform.parent != null) {
            transform.SetParent(null);
        }

        currGravitySource = newSource;
        currGravityStrength = newStrength;
    }



    //left a planet's gravity, no longer bound to that planet in any way
    public void ExitedGravitySource() {
        if (!parentedOnPlanet) {
            return;
        }

        parentedOnPlanet = false;
        transform.SetParent(null);
    }



    
    //landed on the planet
    public void ArrivedAtSource()
    {
        if (!useGravity) {
            return;
        }
        
        if (parentedOnPlanet)
        {
            return;
        }

        parentedOnPlanet = true;
        standingOnPlanet = true;
        transform.SetParent(currGravitySource);
    }



    //no longer physically on the planet
    void LeftFromSource() {
        standingOnPlanet = false;
    }



    //apply gravity
    private void FixedUpdate()
    {
        if (!rb.isKinematic && useGravity) {  //no need to run these when kinematic
            AddForceTowardSource();
            RotateTowardSource();
        }
    }


    //apply 1 FixedUpdate frame of gravity
    void AddForceTowardSource()
    {
        Vector3 directionToSource = GetDirectionToSource();

        Vector3 force = directionToSource * currGravityStrength * rb.mass * weight;
        rb.AddForce(force);
    }



    //rotate toward gravity source according to rotationType
    public void RotateTowardSource()
    {
        if (rotationType != ERotationType.NONE)
        {
            Vector3 directionToSource = GetDirectionToSource();
            float rotSpeed;

            //smooth rotation
            if (transform.parent == null && rotationType == ERotationType.ROTATETOSOURCE) {
                rotSpeed = rotateTowardSourceSpeed * Time.fixedDeltaTime;
            }

            //insta-rotate when parented to planet
            else {
                rotSpeed = 1.0f;
            }

            //actually rotate via Slerp

            //Bit Galaxis' code begins here ===================================================================================
            Quaternion orientationDirection = Quaternion.FromToRotation(-transform.up, directionToSource) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, orientationDirection, rotSpeed);
            //Bit Galaxis' code ends here =====================================================================================
        }
    }



    //returns direction from this object down towards the source of gravity
    public Vector3 GetDirectionToSource()
    {
        if (currGravitySource != null) {
            return (currGravitySource.position - transform.position).normalized;
        }

        return Vector3.zero;
    }



    //toggle gravity
    public void SetGravityActive(bool newActive) {
        useGravity = newActive;
        if (useGravity) {

            //try to find the nearest gravity
            if (currGravitySource == null) {
                GravitySource bestGuess = BestGuessGravitySource();
                EnteredGravitySource(bestGuess.transform, bestGuess.sourceStrength);
            }

        //leave any currently active gravity source
        } else {
            ExitedGravitySource();
        }
    }

    //EXPENSIVE! Only call when necessary
    //return the closest gravity source
    GravitySource BestGuessGravitySource() {
        GravitySource[] allSources = FindObjectsOfType<GravitySource>();

        if (allSources == null) {
            return null;
        }

        GravitySource bestSource = allSources[0];

        foreach (GravitySource source in allSources) {
            if (
                Vector3.Distance(source.transform.position, transform.position)
                <
                Vector3.Distance(bestSource.transform.position, transform.position)) {
                    bestSource = source;
            }
        }

        return bestSource;
    }
}