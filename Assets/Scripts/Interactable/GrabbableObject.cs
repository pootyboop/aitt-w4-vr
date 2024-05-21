using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class GrabbableObject : MonoBehaviour, IInteractable
{
    bool isGrabbable = true;

    bool grabbed = false;
    bool preGrabbedKinematic = false;
    bool justThrown = false;
    //float minThrowStrengthToThrow = 0.02f;
    public bool isThrowable = true;
    public float throwStrength = 20000f;
    public float maxThrowStrength = 50000f;
    public Vector3 lastPos = Vector3.zero;
    Vector3[] lastPositions;
    int lastPosIndex = -1;
    int throwFixedFrameHistory = 5;
    Rigidbody rb;
    Collider coll;


    public void SetInteract(bool newInteract)
    {
        TrySetGrabbed(newInteract);
    }

    public bool IsInteractable() {
        return (!grabbed && isGrabbable);
    }

    public void SetHovered(bool newHovered) {

    }


    private void Start() {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();

        lastPositions = new Vector3[throwFixedFrameHistory];

        SetGrabbable(isGrabbable);
    }

    private void FixedUpdate() {
        if (justThrown) {
            Throw();
        }

        UpdateLastPos();
    }

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

    Vector3 GetOldestPos()
    {
        int oldestPosIndex = lastPosIndex - 1;
        if (oldestPosIndex < 0)
        {
            oldestPosIndex = throwFixedFrameHistory - 1;
        }

        return lastPositions[oldestPosIndex];
    }

    void Throw() {
        justThrown = false;

        Vector3 oldestPos = GetOldestPos();

        Vector3 throwDir = transform.position - oldestPos;

        Vector3 throwVector = throwDir * throwStrength;

        if (throwVector.magnitude > maxThrowStrength) {
            throwVector = throwVector.normalized * maxThrowStrength;
        }

        rb.AddForce(throwVector);

        BabyPlanet maybeBabyPlanet = gameObject.GetComponent<BabyPlanet>();
        if (maybeBabyPlanet != null) {
            maybeBabyPlanet.OnThrown();
        }
    }

    bool CanGrab() {
        return !grabbed;
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

    IEnumerator DelayDroppedCollision() {
        yield return new WaitForSeconds(.2f);
        coll.enabled = true;
    }

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
