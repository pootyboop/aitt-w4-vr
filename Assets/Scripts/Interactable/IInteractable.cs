using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public bool IsInteractable();

    public void SetInteract(bool newInteract);
    public void SetHovered(bool newHovered);
}
