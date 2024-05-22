using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Valve.VR;

//player controller used in menu (title screen) and game
public class PlayerMvmt : MonoBehaviour
{
    //=========REFS=========
    public static PlayerMvmt instance;

    //SteamVR actions
    public SteamVR_Action_Vector2 mvmtAction, snapTurnAction;
    public SteamVR_Action_Single rocketLAction, rocketRAction;
    public SteamVR_Action_Boolean restartAction;

    public Transform cam;
    Rigidbody rb;
    Gravity3D gravity;
    IEnumerator ungroundedTimer;
    public SteamVR_Input_Sources handTypeL, handTypeR;
    public MyHand handL, handR;
    public ParticleSystem rocketParticles;
    //======================

    //========VALUES========
    public float moveSpeed = 500.0f;    //speed of locomotion movement
    public float groundedWeight = 5.0f; //weight to use when grounded to keep player stuck to ground
    float ungroundedTimerTime = 0.2f;   //how long to wait after leaving ground to determine player is officially ungrounded (helps with small hills and stuff)
    public bool useUngroundedTimer = false; //whether to use the ungrounded timer or not
    float snapTurnAngle = 45f;          //how far to snap turn
    float snapTurnDeadZone = 0.2f;      //deadzone for snap turn input
    float canTurnEverySeconds = 0.5f;   //delay between snap turns
    public bool airControl = false;     //can player use locomotion in air?
    public float maxVelocity = 500f;    //absolute cap on player move speed. prevents nausea
    public bool canRocket = true;       //is jetpack/rocket enabled?
    public float rocketStrength = 20f;   //strength of rocket boost
    //======================

    //========STATE=========
    Vector3 desiredMvmtInput;   //raw mvmt input from controllers mapped onto Vector3
    bool isGrounded = false;    //currently on ground?
    bool isRocketing = false;   //currently rocketing?
    public static float lastSnapTurnTime = 0f;    //what Time.time the player last snap turned at
    //======================



    void Start()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
        gravity = GetComponent<Gravity3D>();
        rocketParticles.Stop();
    }


    private void FixedUpdate()
    {
        UpdateSnapTurn();
        Move();
        CapSpeed();

        if (restartAction.stateDown)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }



    //snap turn if player is trying to
    void UpdateSnapTurn()
    {
        //check input
        if (Mathf.Abs(snapTurnAction.axis.x) > snapTurnDeadZone)
        {
            //limit snap turns by time
            if (Time.time >= (lastSnapTurnTime + canTurnEverySeconds))
                {
                Turn(snapTurnAction.axis.x > 0.0f);
            }
        }
    }



    //update player-driven movement
    void Move()
    {
        //air control?
        if (!isGrounded && !airControl) {
            return;
        }
        
        //update player input
        desiredMvmtInput = new Vector3(mvmtAction.axis.x, 0f, mvmtAction.axis.y).normalized;

        //by default, don't move
        Vector3 move = Vector3.zero;

        //if player provided input, make it relative to their current position/rotation and take speed/time into account
        if (desiredMvmtInput != Vector3.zero) {
            move = (desiredMvmtInput.z * GetMoveForwardDir() + desiredMvmtInput.x * cam.transform.right).normalized * Time.fixedDeltaTime * moveSpeed;
        }

        //add rocket force if rockets are in use
        //also set rocketing active/not active here
        if (CanRocket()) {
            move += transform.up * rocketStrength;
            if (!isRocketing)
            {
                isRocketing = true;
                rocketParticles.Play();
            }
        }

        else if (isRocketing)
        {
            isRocketing = false;
            rocketParticles.Stop();
        }

        //finally, add the move force
        rb.AddForce(move);
    }



    //check if player can/should use rockets
    bool CanRocket() {
        return (
            rocketLAction.axis > 0.9f &&
            rocketRAction.axis > 0.9f &&
            canRocket
            );
    }



    //project camera forward onto the gravity direction so player doesn't move differently when looking up/down
    Vector3 GetMoveForwardDir() {

        //first get camera forward direction (where player is looking)
        Vector3 camDir = cam.transform.forward.normalized;

        //get direction to gravity (down towards player's feet)
        Vector3 gravDir = gravity.GetDirectionToSource().normalized;

        //project cam direction onto the gravity direction, making it perpendicular to the gravity force direction and ensuring player won't move up/down when looking up/down
        return camDir - Vector3.Project(camDir, gravDir);
    }



    //snap turn
    void Turn(bool isRight)
    {
        float angle = snapTurnAngle;
        if (!isRight)
        {
            angle = -angle;
        }

        transform.Rotate(transform.up, angle, Space.World);

        lastSnapTurnTime = Time.time;
    }



    //make sure speed is below/equal to maxVelocity
    void CapSpeed() {
        if (rb.velocity.magnitude > maxVelocity) {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }
    }



    private void OnCollisionEnter(Collision other) {
        if (!isGrounded) {

            //might have landed on planet?
            GravitySource source = other.gameObject.GetComponent<GravitySource>();
            if (source != null) {
                SetGrounded(true);
            }

        }
    }

    private void OnCollisionExit(Collision other) {
        if (isGrounded) {

            //might have left planet ground?
            GravitySource source = other.gameObject.GetComponent<GravitySource>();
            if (source != null) {
                StartUngroundedTimer();
            }
            
        }

    }



    void SetGrounded(bool newIsGrounded) {
        if (newIsGrounded && ungroundedTimer != null && useUngroundedTimer) {
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



    //use a timer to give the player a second to re-ground before being officially ungrounded
    //this means little bumps and hills that lift the player off the ground momentarily won't count as being ungrounded
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
