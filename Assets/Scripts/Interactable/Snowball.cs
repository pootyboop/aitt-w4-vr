using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a snowball that grows as it rolls around on the ground, "picking up snow"
public class Snowball : MonoBehaviour
{
    Gravity3D gravity;
    GrabbableObject grabbableObject;

    public float growthAmt = 0.05f; //how much to grow each time it grows
    public float minDistForGrowth = 0.5f;   //minimum distance to travel before growing
    public float maxGrowth = 5f;    //maximum size it can grow to

    Vector3 lastGrowPos;    //last position it grew at. can also be where it landed when falling onto the planet or any other default point from which to start tracking growth

    bool canGrow = true;    //whether growing is allowed



    void Start()
    {
        gravity = GetComponent<Gravity3D>();
        grabbableObject = GetComponent<GrabbableObject>();
        lastGrowPos = transform.position;
    }



    //when landing on a planet, use landing position as the new lastGrowPos#
    //prevents snowballs from growing when landing on a planet or leaving the ground
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("PlanetCollision"))
        {
            lastGrowPos = transform.position;
        }
    }



    //check if the snowball should grow
    void FixedUpdate()
    {
        float mvmt = Vector3.Distance(transform.position, lastGrowPos); //check distance from last position it "grew" at

        //grow if able to
        if (
            canGrow &&  //allowed to grow
            mvmt > minDistForGrowth &&  //travelled enough distance
            transform.lossyScale.x <= maxGrowth //hasn't passed max growth size
            )
        {
            ScaleUp();
        }
    }



    //increase the size of the snowball
    void ScaleUp()
    {
        float newScale = transform.localScale.x + growthAmt;
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



    //wait between growths so the act of scaling up doesn't immediately move the snowball, making it grow again, making it move again...
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
