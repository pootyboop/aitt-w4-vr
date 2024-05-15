using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class GrabbableObject : MonoBehaviour, IInteractable
{
    bool grabbed = false;
    bool preGrabbedKinematic = false;
    bool justThrown = false;
    //float minThrowStrengthToThrow = 0.02f;
    public bool isThrowable = true;
    public float throwStrength = 20000f;
    public float maxThrowStrength = 50000f;
    Vector3 lastPos = Vector3.zero;
    Rigidbody rb;
    Collider coll;


    public void SetInteract(bool newInteract)
    {
        TrySetGrabbed(newInteract);
    }

    public bool IsInteractable() {
        return !grabbed;
    }

    public void SetHovered(bool newHovered) {

    }


    private void Start() {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
    }

    private void FixedUpdate() {
        if (justThrown) {
            Throw();
        }

        lastPos = transform.position;
    }

    void Throw() {
        justThrown = false;

        Vector3 throwDir = transform.position - lastPos;

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
}
