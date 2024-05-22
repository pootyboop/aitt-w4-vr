using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//just follows the position of the camera without matching its rotation
//this allows rocket particles to always be below the camera while still aligned with gravity source
public class RocketParticlesFollow : MonoBehaviour
{
    public Transform cam;

    private void LateUpdate()
    {
        transform.position = cam.position;
    }
}
