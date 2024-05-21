using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MyHand : MonoBehaviour
{
    public SteamVR_Action_Single grabAction;
    GrabbableObject grabbedObject;
    GrabbableObject potentialGrabObject;

    void Start()
    {
        
    }

    void Update()
    {
        UpdateGrab();
    }

    void UpdateGrab() {
        if (grabAction.axis > 0.9f && grabbedObject == null && potentialGrabObject != null) {
            Grab();
        }

        else if (grabAction.axis < 0.1f && grabbedObject != null) {
            Drop();
        }
    }

    void Grab() {
        grabbedObject = potentialGrabObject;
        potentialGrabObject = null;

        grabbedObject.SetInteract(true);

        grabbedObject.transform.SetParent(transform);
        grabbedObject.transform.localPosition = Vector3.zero;
        grabbedObject.transform.localEulerAngles = Vector3.zero;
    }

    public void Drop() {
        grabbedObject.SetInteract(false);

        grabbedObject.transform.SetParent(null);

        grabbedObject = null;
    }

    private void OnTriggerEnter(Collider other) {
        GrabbableObject maybeGrab = other.gameObject.GetComponent<GrabbableObject>();

        if (maybeGrab == null) {
            return;
        }

        if (maybeGrab.IsInteractable() && grabbedObject == null) {
            potentialGrabObject = maybeGrab;
        }
    }

    private void OnTriggerExit(Collider other) {
        GrabbableObject maybeGrab = other.gameObject.GetComponent<GrabbableObject>();
        
        if (maybeGrab == null) {
            return;
        }
        
        if (maybeGrab == potentialGrabObject && grabbedObject == null) {
            potentialGrabObject = null;
        }
    }

    public bool IsHoldingObject() {
        return (grabbedObject != null);
    }
}
