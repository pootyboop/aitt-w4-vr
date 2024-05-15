using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    float originalScale;
    public float growTime = 0.4f;

    private void Start() {
        originalScale = transform.localScale.x;

        StartCoroutine(Grow());
    }

    IEnumerator Grow() {

        float time = 0.0f;
        while (time < growTime) {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(0f, originalScale, Mathf.SmoothStep(0.0f, 1.0f, time/growTime));
            transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }

        transform.localScale = new Vector3(originalScale, originalScale, originalScale);
    }
}