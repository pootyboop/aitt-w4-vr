using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketParticlesFollow : MonoBehaviour
{
    public Transform cam;

    private void LateUpdate()
    {
        transform.position = cam.position;
    }
}
