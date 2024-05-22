using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//interaction interface
//only really used for grabbables
public interface IInteractable
{
    public bool IsInteractable();   //can this be interacted with?

    public void SetInteract(bool newInteract);  //set the interaction state of this object
    public void SetHovered(bool newHovered);    //set the "hovered" state - the object should present itself as interactable
}
