using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Gravity3D : MonoBehaviour
{
    public enum ERotationType {
        NONE,
        ROTATETOSOURCE,
        SNAPTOSOURCE
    }

    Rigidbody rb;
    bool useGravity = false;
    bool parentedOnPlanet = false;
    public bool standingOnPlanet = false;
    Transform currGravitySource;
    float currGravityStrength = 1.0f;
    public float weight = 1.0f;

    public ERotationType rotationType = ERotationType.NONE;
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
            //fires OnTriggerEnter events when spawning inside a gravity source
            transform.position = new Vector3 (transform.position.x + 0.001f, transform.position.y, transform.position.z);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!parentedOnPlanet && other.gameObject.CompareTag("PlanetCollision"))
        {
            ArrivedAtSource();
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (standingOnPlanet && other.gameObject.CompareTag("PlanetCollision"))
        {
            standingOnPlanet = false;
        }
    }



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



    public void LeftPlanetGravity() {
        if (!parentedOnPlanet) {
            return;
        }

        parentedOnPlanet = false;
        transform.SetParent(null);
    }



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


    private void FixedUpdate()
    {
        if (!rb.isKinematic && useGravity) {  //no need to run these when kinematic
            AddForceTowardSource();
            RotateTowardSource();
        }
    }


    
    void AddForceTowardSource()
    {
        Vector3 directionToSource = GetDirectionToSource();

        Vector3 force = directionToSource * currGravityStrength * rb.mass * weight;
        rb.AddForce(force);
    }



    public void RotateTowardSource()
    {
        if (rotationType != ERotationType.NONE)
        {
            Vector3 directionToSource = GetDirectionToSource();

            //
            //https://www.youtube.com/watch?v=v4gkheo0dt8
            //

            float rotSpeed;

            if (transform.parent == null && rotationType == ERotationType.ROTATETOSOURCE) {
                rotSpeed = rotateTowardSourceSpeed * Time.fixedDeltaTime;
            }

            //insta-rotate when parented to planet
            else {
                rotSpeed = 1.0f;
            }

            Quaternion orientationDirection = Quaternion.FromToRotation(-transform.up, directionToSource) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, orientationDirection, rotSpeed);
            //transform.rotation = orientationDirection;
        }
    }


    public Vector3 GetDirectionToSource()
    {
        if (currGravitySource != null) {
            return (currGravitySource.position - transform.position).normalized;
        }

        return Vector3.zero;
    }



    public void SetGravityActive(bool newActive) {
        useGravity = newActive;
        if (useGravity) {

            if (currGravitySource == null) {
                GravitySource bestGuess = BestGuessGravitySource();
                EnteredGravitySource(bestGuess.transform, bestGuess.sourceStrength);
            }

        } else {
            LeftPlanetGravity();
        }
    }

    //EXPENSIVE! Only call when necessary
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