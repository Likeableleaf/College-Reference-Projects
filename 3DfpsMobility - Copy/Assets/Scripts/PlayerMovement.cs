using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    //overall speeds of the player (moveSpeed is assigned by walk or sprint speeds)
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    //how much drag for the ground
    public float groundDrag;

    //Jumping!
    public float jumpForce;
    public float jumpCoolDown;
    public float airMultiplier;
    bool readyToJump;


    [Header("KeyBinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode sneakKey = KeyCode.LeftControl;


    [Header("Sneaking")]
    public float sneakSpeed;
    public float sneakYScale;
    public float startYScale;
    private bool sneakOnce; // allows for the force downward when sneaking to only happen once


    [Header("Slope Check")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;//ensures jumping is allowed on slope

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    
    Rigidbody rb;

    public movementState state;
    public enum movementState{

        walking,
        sprinting,
        sneaking,
        air

        }

    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        Debug.Log("The current Crouch speed is: " + sneakSpeed);
        startYScale = transform.localScale.y;
        sneakOnce = true;
    }

    // Update is called once per frame
    void Update()
    {

        //ground check
        grounded = Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z), Vector3.up, 1f, whatIsGround);
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z), Vector3.up.normalized *1f,Color.blue , 2f);

        MyInput();//handles the inputs for the player movement

        stateHandler();//handles the change in states based on what current inputs the player has
      
        SpeedControl();//clamps the speed for movementSpeed variable when player is inputing input to not go too fast
        

        //handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
            Debug.Log("Drag being initiated!"); 
            Debug.Log("the grounded state is:" + grounded);
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }


    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        
        // when to Jump
        if(Input.GetKey(KeyCode.Space) && readyToJump && grounded)
        {
            readyToJump = false;
            Debug.Log("Space bar has been hit!");

            Jump();
            Invoke(nameof(ResetJump), jumpCoolDown);
        }

        if (Input.GetKey(sneakKey) && sneakOnce)
        {
            transform.localScale = new Vector3(transform.localScale.x, sneakYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5, ForceMode.Impulse);
            sneakOnce = false;
        }
        if (Input.GetKeyUp(sneakKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            sneakOnce = true;
        }

    }

    /*MovePlayer
     * track the movement direction of the player and invoke that as force into the rigidbody of the player
     * void
     */
    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed, ForceMode.Force);
            Debug.Log("Invoking the onslope method");

            //Since rb on slope has no gravity, put force down enough to stop the player from slipping/sliding on slope
            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 6f, ForceMode.Force);
            }
        }


        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        }
        else if(!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        }

        //Turn off gravity when on slope to stop player sliding down slope on accident
        rb.useGravity = !OnSlope();
    }

    /*Speed Control
     * Meant to help limit the speed of the player from going too fast or increasing velocity
     * void
     */
    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //Limit velocity if needed when on slop
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }

        //limit velocity if needed
        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }

        


    }

    private void stateHandler()
    {

        //the player is sneaking
        if (grounded && Input.GetKey(sneakKey))
        {
            state = movementState.sneaking;
            moveSpeed = sneakSpeed;
        }
        //Sprinting state
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = movementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        //Walking state
        else if (grounded)
        {
            state = movementState.walking;
            moveSpeed = walkSpeed;
        }
        //The player must be in the air
        else
        {
            state = movementState.air;

        }
    }

    private void Jump()
    {
        exitingSlope = true;//make sure no force is down while jump from slope

        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        Debug.Log("Jump has been invoked");
         
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        exitingSlope = false; //make sure force is down while on a slope
        readyToJump = true;
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z), Vector3.up,out slopeHit, 1f, whatIsGround))
        {
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z), Vector3.up.normalized * 1f, Color.green, 20f);

            float angle = Vector3.Angle(Vector3.down, slopeHit.normal);//calc angle of slope
            Debug.Log("The current angle im on is going to be: " + angle);

            return angle < maxSlopeAngle && angle != 0; 
        }   
        
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        Debug.DrawRay(Vector3.ProjectOnPlane(moveDirection, slopeHit.normal), Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized * 2f, Color.cyan, 20f);


        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
