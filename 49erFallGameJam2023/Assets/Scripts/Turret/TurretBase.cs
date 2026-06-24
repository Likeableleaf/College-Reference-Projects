using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBase : MonoBehaviour
{
    private GameObject thisTurret;
    private GameObject barrel;


    // Update is called once per frame
    void Start()
    {
        thisTurret = GetComponent<GameObject>();
        //barrel = thisTurret.transform.GetChild(1).gameObject;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("COLLIDING");
        if(collision.gameObject.tag == "Player")
        {
            GameObject p = collision.gameObject;
            if (Input.GetKeyDown(KeyCode.E))
            {
                Player script = p.GetComponent<Player>();
                script.canControl = false;
            }
        }
    }
}
