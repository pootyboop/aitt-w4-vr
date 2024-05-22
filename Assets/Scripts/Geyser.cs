using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//boosts rigidbodies away from it
public class Geyser : MonoBehaviour
{
    List<Rigidbody> rbs;    //overlapped rbs
    public float geyserStrength = 3000f;    //boost strength for everything except player
    public float geyserPlayerStrength = 30000f; //boost strength for player only



    private void Start() {
        rbs = new List<Rigidbody>();
    }



    //try to add a new rb
    private void OnTriggerEnter(Collider other) {
        Rigidbody hitRB = other.gameObject.GetComponent<Rigidbody>();
        if (hitRB != null) {
            rbs.Add(hitRB);
        }
            
    }



    //try to remove a rb
    private void OnTriggerExit(Collider other) {
        Rigidbody hitRB = other.gameObject.GetComponent<Rigidbody>();
        if (hitRB != null) {
            if (rbs.Contains(hitRB)) {
                rbs.Remove(hitRB);
            }
        }
    }



    //apply boost to all overlapped rbs
    private void FixedUpdate() {
        foreach (Rigidbody rb in rbs) {
            ApplyForce(rb);
        }
    }



    //apply the force
    void ApplyForce(Rigidbody rb) {

        //no need to affect kinematic rbs
        if (rb.isKinematic) {
            return;
        }

        Vector3 geyserForce = transform.parent.up;

        //different strength for player
        if (rb.gameObject.CompareTag("Player")) {
            geyserForce *= geyserPlayerStrength;
        }

        //regular strength
        else {
            geyserForce *= geyserStrength;
        }

        rb.AddForce(geyserForce);
    }
}