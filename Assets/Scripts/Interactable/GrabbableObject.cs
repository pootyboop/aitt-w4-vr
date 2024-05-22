using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//object that the player can pick up and (optionally) throw
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class GrabbableObject : MonoBehaviour, IInteractable
{
    bool isGrabbable = true;    //can be grabbed
    bool grabbed = false;       //is grabbed
    bool preGrabbedKinematic = false;   //save the previous isKinematic state of the rb before disabling it when grabbed

    bool justThrown = false;    //used to only update throw velocity on FixedUpdate
    public bool isThrowable = true; //can be thrown
    public float throwStrength = 20000f;    //multiplier for throw vector
    public float maxThrowStrength = 50000f; //cap for throw vector magnitude

    public Vector3 lastPos = Vector3.zero;  //object's position last FixedUpdate
    Vector3[] lastPositions;    //history  of object's position on the last throwFixedFrameHistory number of FixedUpdates
    int lastPosIndex = -1;  //current index to overwrite on lastPositions
    int throwFixedFrameHistory = 5; //number of FixedUpdates ago to remember this object's position at

    Rigidbody rb;
    Collider coll;



    //================IInteractable================
    public void SetInteract(bool newInteract)
    {
        TrySetGrabbed(newInteract);
    }

    public bool IsInteractable() {
        return CanGrab();
    }

    public void SetHovered(bool newHovered) {

    }

    //=============================================



    private void Start() {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();

        lastPositions = new Vector3[throwFixedFrameHistory];

        SetGrabbable(isGrabbable);
    }



    private void FixedUpdate() {
        //if player threw this between last FixedUpdate and now, do the throw
        if (justThrown) {
            Throw();
        }

        UpdateLastPos();
    }



    //update last positions history
    void UpdateLastPos()
    {
        lastPos = transform.position;
        lastPosIndex++;

        if (lastPosIndex >= throwFixedFrameHistory)
        {
            lastPosIndex = 0;
        }

        lastPositions[lastPosIndex] = transform.position;
    }


    //return the oldest remembered object position
    Vector3 GetOldestPos()
    {
        int oldestPosIndex = lastPosIndex - 1;
        if (oldestPosIndex < 0)
        {
            oldestPosIndex = throwFixedFrameHistory - 1;
        }

        return lastPositions[oldestPosIndex];
    }



    //throw this object
    void Throw() {
        justThrown = false;

        //figure out the direction and strength to throw based on oldest remembered position and current position
        Vector3 oldestPos = GetOldestPos(); //get oldest pos
        Vector3 throwDir = transform.position - oldestPos;  //get directional vector
        Vector3 throwVector = throwDir * throwStrength; //multiply by throw strength

        //cap throw strength to maxThrowStrength
        if (throwVector.magnitude > maxThrowStrength) {
            throwVector = throwVector.normalized * maxThrowStrength;
        }

        rb.AddForce(throwVector);

        //if this is a baby planet, let it know it got thrown
        BabyPlanet maybeBabyPlanet = gameObject.GetComponent<BabyPlanet>();
        if (maybeBabyPlanet != null) {
            maybeBabyPlanet.OnThrown();
        }
    }



    //can this be grabbed?
    bool CanGrab() {
        return !grabbed && isGrabbable;
    }



    void TrySetGrabbed(bool newGrabbed) {
        if (grabbed == newGrabbed) {
            return;
        }

        grabbed = newGrabbed;

        if (grabbed) {
            preGrabbedKinematic = rb.isKinematic;
            rb.isKinematic = true;
            coll.enabled = false;
        }

        else {
            rb.isKinematic = preGrabbedKinematic;
            if (isThrowable) {
                justThrown = true;
            }

            StartCoroutine(DelayDroppedCollision());
        }
    }


    //briefly disables collision so dropped/thrown grabbables don't collide with the player's hand
    IEnumerator DelayDroppedCollision() {
        yield return new WaitForSeconds(.2f);
        coll.enabled = true;
    }



    //set whether or not this can be grabbed, and drop it if not
    public void SetGrabbable(bool newGrabbable)
    {
        isGrabbable = newGrabbable;
        if (grabbed && !isGrabbable)
        {
            MyHand hand = transform.parent.gameObject.GetComponent<MyHand>();

            if (hand != null)
            {
                hand.Drop();
            }
        }
    }
}
