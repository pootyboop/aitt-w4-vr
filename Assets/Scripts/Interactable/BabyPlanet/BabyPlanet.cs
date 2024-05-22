using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//small grabbable planet that "expands" into a real planet when thrown far from other planets
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Gravity3D))]
public class BabyPlanet : MonoBehaviour
{
    public Planet planet;   //the planet this planet will grow into (instantiate before this destroys)

    PlanetOverlapper planetOverlapper;  //tracks nearby planets
    Gravity3D gravity;
    Collider coll;

    private void Start() {
        gravity = GetComponent<Gravity3D>();
        coll = GetComponent<Collider>();
        planetOverlapper = GetComponentInChildren<PlanetOverlapper>();
    }



    //called from Gravity3D when thrown by a player
    public void OnThrown() {
        gravity.SetGravityActive(false);    //disable gravity so this can fly into the sky
        planetOverlapper.StartCheckNearbyPlanets(); //start checking for nearby planets, then report back when nothing's around
    }



    //no nearby planets! start growing into a real planet
    public void StartGrow() {
        coll.enabled = false;   //disable collision on this so it doesn't hit anything on the newly born planet
        Planet newPlanet = Instantiate(planet, transform.position, Random.rotation);    //create the new planet
        StartCoroutine(KillAfterSeconds()); //kill this in a little bit
    }



    //wait a bit then destroy this
    IEnumerator KillAfterSeconds() {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
