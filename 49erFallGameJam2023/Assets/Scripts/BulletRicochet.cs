using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class BulletRicochet : MonoBehaviour
{
    [Range(0f, 5f)]
    public float shootSpeed = 5f;

    private Rigidbody2D rb;
    private Vector3 lastVelocity;
    private float bulletSpeed;

    //Paused vars
    private bool isPaused;
    public float pausedMass = 100f;
    public float baseMass = 1f;
    public int durability = 3;
    public bool shouldShoot;

    public bool shouldApplyForces;

    private int rCasting = 1 << 6;

    private Vector3 prevVelocity; //velocity from before we paused.

    public ParticleSystem oilSplash;

    public AudioSource audioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        PauseEvent.OnPause += OnPaused;
        rCasting = ~rCasting;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.B))
        {
            shouldShoot = true;

        }*/

        //pause handling
        if (isPaused)
        {
            rb.mass = pausedMass;
            rb.Sleep();
        }
        else
        {
            rb.mass = baseMass;
            rb.WakeUp();
        }

        /*        if (durability == 0)
                {
                    Debug.Log("BULLET DESTROY");
                    Destroy(this.gameObject);
                }*/
        if (durability == 0)
        {
            Debug.Log("BULLET DESTROY");
            Destroy(this.gameObject);
        }

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.2f, transform.up, 1f, rCasting);
        Debug.DrawRay(transform.position, transform.up, Color.green);
        if (hit)
        {
            Debug.DrawLine(transform.position, hit.point);
            Debug.Log("HIT " + hit.collider.gameObject);
            if (hit.collider.gameObject.CompareTag("Robot"))
            {
                if (hit.distance <= 0.5 && hit.collider.enabled == true)
                {
                    Debug.Log("DESTROYED ROBOT");
                    hit.collider.enabled = false; //disable collider

                    //Set the sprite
                    if (hit.collider.gameObject.TryGetComponent<Slowbody>(out Slowbody s))
                    {
                        s.setBroken();
                    }

                    //Do oil splash based off of normal from contact
                    if (oilSplash != null)
                        Instantiate(oilSplash, hit.point, Quaternion.LookRotation(hit.normal));



                    //Destroy(hit.collider.gameObject);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        lastVelocity = rb.velocity;

        if (shouldShoot)
        {
            rb.AddForce(transform.up * shootSpeed, ForceMode2D.Impulse);
            shouldShoot = false;
        }



        if (shouldApplyForces) //on resume make bullet continue moving.
        {
            rb.WakeUp();
            rb.AddForce(prevVelocity, ForceMode2D.Impulse);
            shouldApplyForces = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            bulletSpeed = lastVelocity.magnitude;
            Vector2 direction = Vector2.Reflect(lastVelocity.normalized, other.contacts[0].normal);
            rb.velocity = direction * bulletSpeed; //Don't care if they say not to.
            audioSource.Play();

            //rotate, it shouldn't hurt the rb.
            Vector2 dir = rb.velocity;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);




            durability--;
            Debug.Log("Durability: " + durability);
        }
    }

    private void OnPaused(bool p)
    {
        if (p == true)
        {
            if (rb != null)
            prevVelocity = rb.velocity;
        }
        if (p == false)
        {
            shouldApplyForces = true;
        }
        isPaused = p;
    }
}
