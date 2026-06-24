using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretFire : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private float starting_rotation;
    [SerializeField] private float max_rotation;
    [SerializeField] private float min_rotation;
/*    [SerializeField, Range(0f, 5f)] private float rayDistance = 1f;*/
    private GameObject thisTurret;
    private Transform trans;
    private bool active;

    public bool playerControlled;

    private bool rotateRight;
    private bool rotateLeft;

    //used to determine when they have clicked for a second time.
    public int clickCount;

    public bool didShoot;

    private void Start()
    {
        playerControlled = false;
        trans = GetComponent<Transform>();
        thisTurret = GetComponent<GameObject>();
        trans.Rotate(0f, 0f, starting_rotation);
    }


    private void Update()
    {
        if (Input.GetKey("right"))
        {
            rotateRight = true;
        }
        else
        {
            rotateRight = false;
        }

        if (Input.GetKey("left"))
        {
            rotateLeft = true;
        }
        else
        {
            rotateLeft = false;
        }

        //SHOOT THE GOD DAMN BULLET.
        if (clickCount == 2 && !didShoot)
        {
            Debug.Log("CLICK COUNT!");
            bullet.transform.position = this.transform.position - this.trans.up * this.gameObject.GetComponent<Collider2D>().bounds.extents.y;
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, this.trans.rotation.eulerAngles.z + 180);
            bullet.GetComponent<Rigidbody2D>().AddForce(bullet.transform.up * 5f, ForceMode2D.Impulse);
            //Debug.Break(); //pause the editor.
            didShoot = true;
            Debug.Log(bullet.GetComponent<Rigidbody2D>().velocity);
            StartCoroutine(waitThenDisable(0.2f));
            //bullet.GetComponent<BulletRicochet>().shouldShoot = true;
        }
    }

    private void FixedUpdate() { //you cannot check for input in fixed update, it is only called when
                                 //physics updates occur (if a collision happens for example)
        if(playerControlled)
        { 
            if(rotateRight && trans.rotation.z < max_rotation)
            {
                Debug.Log(trans.rotation.z);
                trans.Rotate(0f,0f,3f);
            }
            if (rotateLeft && trans.rotation.z > min_rotation)
            {
                trans.Rotate(0f, 0f, -3f);
            }

 /*           if(Input.GetKeyDown(KeyCode.E))
            {
                playerControlled = false;
            }*/
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("COLLIDING");
        /*if (collision.gameObject.tag == "Player")
        {
            GameObject p = collision.gameObject;
            if (Input.GetKeyDown(KeyCode.E))
            {
                Player script = p.GetComponent<Player>();
                if(!active)
                {
                    script.canControl = false;
                    active = true;
                }else
                {
                    script.canControl = true;
                }
            }
        }*/
    }

    public IEnumerator waitThenDisable(float duration)
    {
        yield return new WaitForSeconds(duration);
        this.gameObject.GetComponentInParent<TurretBase>().gameObject.SetActive(false);
    }
}
