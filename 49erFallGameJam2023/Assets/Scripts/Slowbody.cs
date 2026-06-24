using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slowbody : MonoBehaviour
{
    //Remember to make the slowbody slow down after the player stops interacting

    Rigidbody2D rb;



    Queue<Collision2D> collisions;
    Queue<Vector2> forces;

    public Collision2D lastCollision;
    public Vector2 lastRelativeVelocity;

    public bool isPaused;

    bool shouldApplyForces;

    public float hitForce = 100f;

    public float baseMass = 1f;

    public float pausedMass = 100f;

    public bool shouldStop;

    public Sprite brokenSprite;

    public bool broken;

    public GameObject windEffect;

    private ParticleSystem windSystem;

    public float curWindTime = 0;
    public float windTime = 0.45f; //half of the wind particle systems time.

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PauseEvent.OnPause += OnPaused;
        collisions = new Queue<Collision2D>();
        forces = new Queue<Vector2>();

    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused)
        {
            rb.mass = pausedMass;
        }
        else if (!broken)
        {
            rb.mass = baseMass;
            if (rb.IsSleeping())
            rb.WakeUp();
            //rb.constraints = RigidbodyConstraints2D.None;
        }

        if (!isPaused && windSystem != null || windSystem != null && broken)
        {
            Destroy(windSystem);
        }

        if (lastRelativeVelocity != null)
        {
            Debug.DrawRay(this.transform.position, -1 * lastRelativeVelocity, Color.yellow, 0f);
            //Debug.DrawRay(this.transform.position + (Vector3)lastRelativeVelocity, lastCollision.relativeVelocity, Color.red, 0f);
        }

        if (windSystem != null && windSystem.isPlaying && isPaused && curWindTime >= windTime)
        {
            windSystem.Pause(); //pause the system as we are paused and have finished playing.
            curWindTime = 0;
        }
        else if (windSystem != null && windSystem.isPlaying && isPaused)
        {
            curWindTime += Time.deltaTime;
        }
        
    }

    private void FixedUpdate()
    {

        if (shouldApplyForces && lastRelativeVelocity != null && !broken)
        {
/*            Collision2D collision = lastCollision;
            //Vector2 hitForce = forces.Dequeue();
            foreach (ContactPoint2D c in collision.contacts)
            {
                if (c.rigidbody != null)
                {
                    //Debug.DrawLine(c.point, c.point * 10f, Color.red);
                    //Debug.Break();

                    //wakeup the rb just incase it is still asleep.
                    rb.WakeUp();
                    rb.AddForce(*//*-c.normal * hitForce*/ /*lastForceVector.normalized * hitForce*//*lastRelativeVelocity, ForceMode2D.Impulse); //The normal is the collision normal vector relative to object that we have
                    shouldApplyForces = false; //set shouldApplyForces to false.
                }

            }*/

            //Debug.DrawLine(c.point, c.point * 10f, Color.red);
            //Debug.Break();

            //wakeup the rb just incase it is still asleep.
            rb.WakeUp();
            rb.AddForce(/*-c.normal * hitForce*/ /*lastForceVector.normalized * hitForce*/-1 * lastRelativeVelocity, ForceMode2D.Impulse); //The normal is the collision normal vector relative to object that we have
            shouldApplyForces = false; //set shouldApplyForces to false.
        }

        if (isPaused && shouldStop && !broken)
        {
            //makes the rb stop doing all physics. (rotation and movement)
            //rb.Sleep();
            //rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.Sleep();
            Debug.Log("STOP");
            shouldStop = false;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
/*        //we need there to be another rigidbody if we are going to apply force to this,
        //that way if we hit one object into another and unpause the force still applies.
        if (isPaused && collision.otherRigidbody != null)
        {
            collisions.Enqueue(collision);
            forces.Enqueue(collision.otherRigidbody.velocity);
            //lastCollision = collision;
            //lastForceVector = collision.otherRigidbody.velocity;
            //Debug.Log("Collision set");
            //Debug.DrawRay(this.transform.position, collision.relativeVelocity, Color.red, 1f);
            *//*foreach(ContactPoint2D c in collision.contacts)
            {
                Debug.DrawRay(this.transform.position, c.normal, Color.yellow, 1f);
            }*//*
        }*/
        

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        shouldStop = true;
    }

    private void OnPaused(bool p)
    {
        isPaused = p;
        if (isPaused == false)
        {
            shouldApplyForces = true;
        }
    }

    public void setBroken()
    {
        //slow down time so we get impact frames.
        StartCoroutine(GameManager.instance.slowTime(0.1f, 1f));
        Debug.Log("broken");
        GetComponent<AudioSource>().Play();
        if (brokenSprite != null)
        this.GetComponent<SpriteRenderer>().sprite = brokenSprite;
        broken = true;
        rb.Sleep();
    }

    public void DoWind()
    {
        if (windSystem != null)
        {
            //Destroy(windSystem);
            windSystem.transform.position = this.transform.position;
            windSystem.transform.rotation = Quaternion.LookRotation(lastRelativeVelocity);
        }
        else
        {
            windSystem = GameObject.Instantiate(windEffect, this.transform.position, Quaternion.LookRotation(lastRelativeVelocity)).GetComponent<ParticleSystem>();
        }
        
        windSystem.Play();
        curWindTime = 0; //set wind timer to be zero
        Debug.Log(windSystem);
    }
}
