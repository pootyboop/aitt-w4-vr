using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used by planets to grow when spawned, usually from a BabyPlanet
public class Planet : MonoBehaviour
{
    float originalScale;
    public float growTime = 0.4f;   //time it takes to grow to full size



    private void Start() {
        originalScale = transform.localScale.x; //save original scale

        StartCoroutine(Grow());
    }



    //do the actual growing
    IEnumerator Grow() {

        float time = 0.0f;
        while (time < growTime) {
            time += Time.deltaTime;

            //update scale
            float scale = Mathf.Lerp(0f, originalScale, Mathf.SmoothStep(0.0f, 1.0f, time/growTime));
            transform.localScale = new Vector3(scale, scale, scale);

            yield return null;
        }

        //finally, ensure planet scale is set to its original scale
        transform.localScale = new Vector3(originalScale, originalScale, originalScale);
    }
}