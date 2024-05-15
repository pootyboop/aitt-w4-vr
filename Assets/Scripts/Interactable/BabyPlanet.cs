using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Gravity3D))]
public class BabyPlanet : MonoBehaviour
{
    public Planet planet;

    PlanetOverlapper planetOverlapper;
    Gravity3D gravity;
    Collider coll;

    private void Start() {
        gravity = GetComponent<Gravity3D>();
        coll = GetComponent<Collider>();
        planetOverlapper = GetComponentInChildren<PlanetOverlapper>();
    }

    public void OnThrown() {
        gravity.SetGravityActive(false);
        planetOverlapper.StartCheckNearbyPlanets();
    }

    public void StartGrow() {
        coll.enabled = false;
        Planet newPlanet = Instantiate(planet, transform.position, Quaternion.identity);
        StartCoroutine(KillAfterSeconds());
    }

    IEnumerator KillAfterSeconds() {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
