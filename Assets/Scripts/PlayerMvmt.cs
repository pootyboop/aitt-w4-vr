using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerMvmt : MonoBehaviour
{
    //=========REFS=========
    public static PlayerMvmt instance;
    public SteamVR_Action_Vector2 action;
    public Transform cam;
    Rigidbody rb;
    Gravity3D gravity;
    IEnumerator ungroundedTimer;
    //======================

    //========VALUES========
    public float moveSpeed = 500.0f;
    public float groundedWeight = 5.0f;
    float ungroundedTimerTime = 0.2f;
    public bool useUngroundedTimer = false;
    //======================

    //========STATE=========
    Vector3 desiredMvmtInput;
    bool isGrounded = false;
    //======================



    void Start()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
        gravity = GetComponent<Gravity3D>();
    }


    private void FixedUpdate()
    {
        Move();
    }



    void Move()
    {
        //no air control
        if (!isGrounded) {
            return;
        }

        desiredMvmtInput = new Vector3(action.axis.x, 0f, action.axis.y).normalized;
        Vector3 move = Vector3.zero;

        if (desiredMvmtInput != Vector3.zero) {
            move = (desiredMvmtInput.z * transform.forward + desiredMvmtInput.x * transform.right) * Time.fixedDeltaTime * moveSpeed;
        }
            rb.velocity = move;
    }

    private void OnCollisionEnter(Collision other) {
        if (!isGrounded) {

            GravitySource source = other.gameObject.GetComponent<GravitySource>();
            if (source != null) {
                SetGrounded(true);
            }

        }
    }

    private void OnCollisionExit(Collision other) {
        if (isGrounded) {

            GravitySource source = other.gameObject.GetComponent<GravitySource>();
            if (source != null) {
                StartUngroundedTimer();
            }
            
        }

    }



    void SetGrounded(bool newIsGrounded) {
        if (newIsGrounded && ungroundedTimer != null) {
            StopCoroutine(ungroundedTimer);
            ungroundedTimer = null;
        }

        if (isGrounded == newIsGrounded) {
            return;
        }

        isGrounded = newIsGrounded;
        if (isGrounded) {
            gravity.weight = groundedWeight;
        }

        else {
            gravity.weight = 1.0f;
        }
    }



    void StartUngroundedTimer() {
        if (!useUngroundedTimer) {
            SetGrounded(false);
            return;
        }

        if (ungroundedTimer != null) {
            StopCoroutine(ungroundedTimer);
        }

        ungroundedTimer = UngroundedTimer();
        StartCoroutine(ungroundedTimer);
    }



    IEnumerator UngroundedTimer() {
        float time = 0.0f;
        while (time < ungroundedTimerTime) {
            time += Time.deltaTime;
            yield return null;
        }

        SetGrounded(false);
    }

}
