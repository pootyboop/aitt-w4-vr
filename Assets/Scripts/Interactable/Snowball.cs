using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowball : MonoBehaviour
{
    Gravity3D gravity;
    GrabbableObject grabbableObject;
    public float growthAmt = 0.05f;

    public float minDistForGrowth = 0.5f;
    public float maxGrowth = 5f;
    Vector3 lastGrowPos;

    bool canGrow = true;

    void Start()
    {
        gravity = GetComponent<Gravity3D>();
        grabbableObject = GetComponent<GrabbableObject>();
        lastGrowPos = transform.position;
    }

    void FixedUpdate()
    {
        float mvmt = Vector3.Distance(transform.position, lastGrowPos);
        print("dist: " + mvmt);

        if (canGrow && mvmt > minDistForGrowth && transform.lossyScale.x <= maxGrowth)
        {
            ScaleUp();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("PlanetCollision"))
        {
            lastGrowPos = transform.position;
        }
    }

    void ScaleUp()
    {
        float newScale = transform.localScale.x + growthAmt/* * transform.lossyScale.x*/;
        if (newScale > maxGrowth)
        {
            newScale = maxGrowth;
        }

        transform.localScale = new Vector3(
            newScale,
            newScale,
            newScale
            );

        StartCoroutine(GrowCooldown());
    }

    IEnumerator GrowCooldown()
    {
        canGrow = false;

        float time = 0f;
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        lastGrowPos = transform.position;
        canGrow = true;
    }
}
