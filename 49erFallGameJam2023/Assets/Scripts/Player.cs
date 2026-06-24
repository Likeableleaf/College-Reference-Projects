using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Vector2 inputVector;

    Vector2 lastInputVector;

    Rigidbody2D rb;

    public TurretFire tFire;

    public int rCasting;

    float lastInputTime; //time since we last pressed 2 diagonal inputs simultaneously.

    public float diagonalInputTimeFrame = 0.1f; //0.1f in our usecase == 100ms.

    public float pauseSlowDownConstant = 2f; //how much we divide all values by when slowed.

    public float baseMoveSpeed = 10f;

    //the move speed we use directly in calculations and set during pause/unpause.
    private float moveSpeed = 10f;

    //we set the trail falloff time to be this divided by our pause constant if we are paused and just this if we aren't
    public float trailTime = 0.5f;

    public bool isTimeFrozen;
    private int pauseCount = 0;

    public bool canControl = true; //allows the player to move

    [Range(0f, 5f)] public float rayDistance = 1f;

    Animator animator;

    public Image stopWatch;

    public Sprite watchOnSprite;
    public Sprite watchOffSprite;

    public AudioSource footstepAudioSource;

    public AudioSource sfxAudioSource;
    public AudioClip enterQuickTime;
    public AudioClip exitQuickTime;

    public AudioSource musicAudioSource;
    public AudioClip pausedMusicClip;
    public AudioClip realtimeMusicClip;

    // Start is called before the first frame update
    void Start()
    {
        rCasting = 1 << 3;
        rCasting = ~rCasting;//everything but the player layer
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canControl)
        {
            inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //set input
        }
        else
        {
            inputVector = new Vector2(0f, 0f);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            //raycast, rcasting is all layers accept the player layer.
            RaycastHit2D hit = Physics2D.Raycast(this.transform.position, -this.transform.up, rayDistance, rCasting);

            //if we hit a turret, allow us to rotate it.
            if (hit != false && hit.collider.gameObject.CompareTag("Turret"))
            {
                canControl = !canControl;
                tFire.playerControlled = !canControl;
                if (tFire.clickCount < 2)
                tFire.clickCount++;
            }
            
        }

        

        //handles if two buttons are let go at the same time but they aren't let go at EXACTLY
        //the same time we still leave it on the diagonal if they are within 1ms of each other. 
        //
        #region PlayerOrientationHandling 

        if (inputVector.x != 0 && inputVector.y != 0) //this is so that when the player stops giving input the last input 
        {
            lastInputVector = inputVector;
            lastInputTime = 0f; //only reset lastInputTime here so that when the player lets go of two buttons at the same time but they aren't let go at EXACTLY
                                //the same time we still leave it on the diagonal if they are within 1ms of each other. 
        }
        else if (((inputVector.x == 0 && inputVector.y != 0) || (inputVector.x != 0 && inputVector.y == 0)) && lastInputTime >= diagonalInputTimeFrame) //only when we have pressed a single input long enough after two inputs should we set the lastInputVector.
        {
            lastInputVector = inputVector; //set lastInputVector
        }
        lastInputTime += Time.deltaTime; //increment last inputTime

        /*        animator.SetFloat("x", lastInputVector.x); //Sets our animator values for the 2D value that determines our 8 direction orientation.
                animator.SetFloat("y", lastInputVector.y);*/
        if (inputVector.magnitude > 0)
        {
            if (!footstepAudioSource.isPlaying)
            footstepAudioSource.Play();
            if (isTimeFrozen)
            {
                footstepAudioSource.pitch = 1f;
            }
            else
            {
                footstepAudioSource.pitch = 2f;
            }
        }
        else
        {
            if (footstepAudioSource.isPlaying)
                footstepAudioSource.Pause();
        }

        animator.SetFloat("X", lastInputVector.x); //Sets our animator values for the 2D value that determines our 8 direction orientation.
        animator.SetFloat("Y", lastInputVector.y);

        #endregion

        if (Input.GetKeyDown("space") && canControl)
        {
            //only allow it to be done twice (pressing spacebar I mean, you only pause once)
            if (pauseCount < 2)
            {
                isTimeFrozen = !isTimeFrozen;
                if (isTimeFrozen == true)
                {
                    sfxAudioSource.clip = enterQuickTime;
                    sfxAudioSource.Play();
                }
                else
                {
                    sfxAudioSource.clip = exitQuickTime;
                    sfxAudioSource.Play();
                }
                PauseEvent.SetPaused(isTimeFrozen);
                pauseCount++;
            }
            
            if (isTimeFrozen)
            {
                //Time.timeScale = 0.05f;

                moveSpeed = baseMoveSpeed / pauseSlowDownConstant;
                //if we have a trail renderer scale it's time for pause mode.
                if (TryGetComponent<TrailRenderer>(out TrailRenderer t))
                {
                    t.enabled = true;
                    t.time = trailTime;
                    //t.emitting = true;
                }
            }
            else
            {
                // Time.timeScale = 1;

                moveSpeed = baseMoveSpeed;
                //if we have a trail renderer scale it's time for pause mode.
                if (TryGetComponent<TrailRenderer>(out TrailRenderer t))
                {
                    //we divide here instead of when paused because we want the trail to exist longer when paused, because you
                    //move slower
                    t.enabled = false;
                    t.time = trailTime / pauseSlowDownConstant;
                    //t.emitting = false;
                }
            }
        }

        if (isTimeFrozen)
        {
            if (stopWatch != null)
                stopWatch.sprite = watchOnSprite;
            if (musicAudioSource != null)
            {
                /*if (pausedMusicClip != null)
                {
                    if (s.clip != pausedMusicClip)
                    {
                        s.clip = pausedMusicClip;
                        s.Play();
                    }
                    
                }*/
                if (musicAudioSource.isPlaying == false)
                    musicAudioSource.Play();
                musicAudioSource.pitch = 0.7f;
            }
        }
        else
        {
            if (stopWatch != null)
                stopWatch.sprite = watchOffSprite;
            if (musicAudioSource != null)
            {
                musicAudioSource.pitch = 1f;
                if (realtimeMusicClip != null)
                {
                    if (musicAudioSource.clip != realtimeMusicClip)
                    {

                        musicAudioSource.clip = realtimeMusicClip;
                        musicAudioSource.Play();
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + (inputVector.normalized * moveSpeed * Time.deltaTime)); //move rigibody 2D, we need to use velocity if we want the player
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //for some reason this is getting set regardless of if I am colliding with a slowbody or a normal object.
        //if we our player hits a slowbody we need to set it's last collision to be this.
        /*RaycastHit2D hit = Physics2D.CircleCast(transform.position, 1f, -transform.up, 0.5f, rCasting);*/
        Debug.Log(collision.collider.gameObject);
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, -this.transform.up, rayDistance, rCasting);
        //we check this raycayst hit 2D so that we can prevent 
        if (isTimeFrozen && collision.gameObject.TryGetComponent<Slowbody>(out Slowbody sb) && hit && hit.collider.gameObject.Equals(collision.gameObject))
        {
            sb.lastRelativeVelocity = collision.relativeVelocity;
            sb.lastCollision = collision;
            sb.DoWind();
        }

        if (collision.collider.gameObject.GetComponent<BulletRicochet>())
        {
            //do some wierd particle fx
            Debug.Log("PLAYER IS DEAD");
            //set scene to be player death scene.
            GameManager.instance.setScene("GameOver_YouDied");
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, -this.transform.up, rayDistance, rCasting);
        //we check this raycayst hit 2D so that we can prevent 
        if (isTimeFrozen && collision.gameObject.TryGetComponent<Slowbody>(out Slowbody sb) && hit && hit.collider.gameObject.Equals(collision.gameObject))
        {
            //sb.lastRelativeVelocity = collision.relativeVelocity;
            //sb.lastCollision = collision;
            sb.DoWind();
        }
    }

}
