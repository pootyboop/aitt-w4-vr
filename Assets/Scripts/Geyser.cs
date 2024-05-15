using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geyser : MonoBehaviour
{
    List<Rigidbody> rbs;
    public float geyserStrength = 3000f;
    public float geyserPlayerStrength = 30000f;

    private void Start() {
        rbs = new List<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other) {
        Rigidbody hitRB = other.gameObject.GetComponent<Rigidbody>();
        if (hitRB != null) {
            rbs.Add(hitRB);
        }
            
    }

    private void OnTriggerExit(Collider other) {
        Rigidbody hitRB = other.gameObject.GetComponent<Rigidbody>();
        if (hitRB != null) {
            if (rbs.Contains(hitRB)) {
                rbs.Remove(hitRB);
            }
        }
    }

    private void FixedUpdate() {
        foreach (Rigidbody rb in rbs) {
            ApplyForce(rb);
        }
    }

    void ApplyForce(Rigidbody rb) {
        if (rb.isKinematic) {
            return;
        }

        Vector3 geyserForce = transform.parent.up;

        if (rb.gameObject.CompareTag("Player")) {
            geyserForce *= geyserPlayerStrength;
        }

        else {
            geyserForce *= geyserStrength;
        }

        rb.AddForce(geyserForce);
    }
}