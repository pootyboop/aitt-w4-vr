using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//tracks nearby planets and reports back to BabyPlanet when none are nearby
//radius to check is controlled by sphere collider on gameobject
public class PlanetOverlapper : MonoBehaviour
{
    List<GravitySource> overlappedSources;  //all overlapped GravitySources (planets)



    private void Start() {
        overlappedSources = new List<GravitySource>();
    }


    
    //add overlapped object to overlappedSources if it's a GravitySource
    private void OnTriggerEnter(Collider other) {
        GravitySource gravSrc = other.gameObject.GetComponent<GravitySource>();
        if (gravSrc != null) {
            overlappedSources.Add(gravSrc);
        }
    }



    //remove overlapped object to overlappedSources if it's a GravitySource
    private void OnTriggerExit(Collider other) {
        GravitySource gravSrc = other.gameObject.GetComponent<GravitySource>();
        if (gravSrc != null) {
            if (overlappedSources.Contains(gravSrc)) {
                overlappedSources.Remove(gravSrc);
            }
        }
    }



    //start checking for when no planets are nearby
    public void StartCheckNearbyPlanets() {
        StartCoroutine(CheckNearbyPlanets());
    }



    IEnumerator CheckNearbyPlanets() {
        while (overlappedSources.Count > 0) {   //when 1+ planets are nearby (in radius)
            yield return new WaitForSeconds(1f);    //only checks every second since this doesn't need to be super precise
        }

        //no planets nearby, tell BabyPlanet it's safe to grow
        transform.parent.GetComponent<BabyPlanet>().StartGrow();

        //and stop tracking
        gameObject.SetActive(false);
    }
}