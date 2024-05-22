using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

//manages hands, mainly for holding and grabbing
public class MyHand : MonoBehaviour
{
    public SteamVR_Action_Single grabAction;
    GrabbableObject potentialGrabObject;    //what the hand would grab if it performed a grab right now
    GrabbableObject grabbedObject;  //what the hand is currently grabbing



    void Update()
    {
        UpdateGrab();
    }



    //check if player wants to grab or drop and perform appropriate action
    void UpdateGrab() {
        if (grabAction.axis > 0.9f && grabbedObject == null && potentialGrabObject != null) {
            Grab();
        }

        else if (grabAction.axis < 0.1f && grabbedObject != null) {
            Drop();
        }
    }



    //grab potentialGrabObject
    void Grab() {

        //update refs
        grabbedObject = potentialGrabObject;
        potentialGrabObject = null;

        //let grabbedObject know about the grab
        grabbedObject.SetInteract(true);

        //snap grabbedObject to the hand
        grabbedObject.transform.SetParent(transform);
        grabbedObject.transform.localPosition = Vector3.zero;
        grabbedObject.transform.localEulerAngles = Vector3.zero;
    }



    //drop grabbedObject
    public void Drop() {
        //let grabbedObject know we're dropping/throwing it
        grabbedObject.SetInteract(false);

        //unparent grabbedObject
        grabbedObject.transform.SetParent(null);

        //clear grabbedObject so we know we aren't holding anything
        grabbedObject = null;
    }



    //try to set potentialGrabObject
    private void OnTriggerEnter(Collider other) {
        GrabbableObject maybeGrab = other.gameObject.GetComponent<GrabbableObject>();

        if (maybeGrab == null) {
            return;
        }

        if (maybeGrab.IsInteractable() && grabbedObject == null) {
            potentialGrabObject = maybeGrab;
        }
    }



    //try to clear potentialGrabObject
    private void OnTriggerExit(Collider other) {
        GrabbableObject maybeGrab = other.gameObject.GetComponent<GrabbableObject>();
        
        if (maybeGrab == null) {
            return;
        }
        
        if (maybeGrab == potentialGrabObject && grabbedObject == null) {
            potentialGrabObject = null;
        }
    }



    //check if hand is free
    public bool IsHoldingObject() {
        return (grabbedObject != null);
    }
}
