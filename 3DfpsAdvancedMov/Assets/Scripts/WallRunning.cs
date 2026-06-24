using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("WallRunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft;
    private bool wallRight;

    [Header("References")]
    public Transform orientation;
    public Transform Player;
    private PlayerMovement pm;
    private Rigidbody rb;




    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent <PlayerMovement>();
        rb = GetComponent<Rigidbody>();



    }

    // Update is called once per frame
    void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.wallrunning)
        {
            WallRunningMovement();
        }
       
    }

    private void CheckForWall()
    {
        //Todo flip these rays so that instead of facing out from the player, face in toward the player from the left and right sides.
        //wallLeft = Physics.Raycast(transform.position + (orientation.right * wallCheckDistance), -orientation.right * 20f, out rightWallhit, 2f , whatIsWall) ;
        wallLeft = Physics.Raycast(Player.transform.position, -orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
        wallRight = Physics.Raycast(Player.transform.position, orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);
        //wallRight = Physics.Raycast(transform.position + (-orientation.right * wallCheckDistance), orientation.right, out leftWallhit, 2f, whatIsWall);

        Debug.DrawRay(Player.transform.position, -orientation.right , Color.blue, Mathf.Infinity);
        Debug.DrawRay(Player.transform.position, orientation.right, Color.blue, Mathf.Infinity);


        //Debug.DrawRay(transform.position + (orientation.right * wallCheckDistance), -orientation.right * 20f,Color.blue, Mathf.Infinity);
        //Debug.DrawRay(transform.position + (-orientation.right * wallCheckDistance),   orientation.right, Color.blue, Mathf.Infinity);
        Debug.Log("The hit for the wall is Right: " +wallRight + " left:" + wallLeft);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(new Vector3(Player.transform.position.x, Player.transform.position.y - minJumpHeight, Player.transform.position.z), Vector3.up, 1f, whatIsGround);
    }

    private void StateMachine()
    {

        // Getting Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //State 1 - Wall running
        if((wallRight || wallLeft) && verticalInput > 0 && AboveGround() && rb.velocity.magnitude > 0 && !pm.getGrounded())
        {

            Debug.Log("Invoke Wall run has been Activated! the velocity magnitude is: " + rb.velocity.magnitude);
            //start wall run
            if (!pm.wallrunning)
            {
                StartWallRunning();
            }



        }

        else
        {
            if (pm.wallrunning)
            {
                StopWallRunning();
                Debug.Log("invoke stop running!!!!!!!!!!!!!!!!!!!");
                
            }
            if (rb.velocity.magnitude <= 0.1 && pm.getGrounded())
            {
                Debug.Log("Invoke Wall run stop! the velocity magnitude is: " + rb.velocity.magnitude);
                //rb.AddForce(-orientation.up * 200f, ForceMode.Force );
            }
            
        }



    }


    private void StartWallRunning()
    {
        pm.wallrunning = true;
    }

    private void WallRunningMovement()
    {
        rb.useGravity = false;
        //rb.mass = 0.5f;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
        rb.AddForce(orientation.forward * wallRunForce, ForceMode.Force);
    
    }

    private void StopWallRunning()
    {
        pm.wallrunning = false;
        rb.useGravity = true;
        rb.mass = 1f;
    }


}
