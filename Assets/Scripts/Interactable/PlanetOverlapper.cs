using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetOverlapper : MonoBehaviour
{
    List<GravitySource> overlappedSources;

    private void Start() {
        overlappedSources = new List<GravitySource>();
    }

    private void OnTriggerEnter(Collider other) {
        GravitySource gravSrc = other.gameObject.GetComponent<GravitySource>();
        if (gravSrc != null) {
            overlappedSources.Add(gravSrc);
        }
    }

    private void OnTriggerExit(Collider other) {
        GravitySource gravSrc = other.gameObject.GetComponent<GravitySource>();
        if (gravSrc != null) {
            if (overlappedSources.Contains(gravSrc)) {
                overlappedSources.Remove(gravSrc);
            }
        }
    }

    public void StartCheckNearbyPlanets() {
        StartCoroutine(CheckNearbyPlanets());
    }

    IEnumerator CheckNearbyPlanets() {
        while (overlappedSources.Count > 0) {
            yield return new WaitForSeconds(1f);
        }

        transform.parent.GetComponent<BabyPlanet>().StartGrow();
        gameObject.SetActive(false);
    }
}
