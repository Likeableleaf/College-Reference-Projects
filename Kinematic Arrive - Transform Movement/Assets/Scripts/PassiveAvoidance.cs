using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveAvoidance : MonoBehaviour
{
    [SerializeField] GameObject agent;
    private Vector3 newDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        
        //grab object collided with
        GameObject collided = collision.gameObject;
        if ( !( collided.name == "Ground" || collided.name == "Character" || collided.name == "AI1" || collided.name == "AI2" || collided.name == "AI3") )
        {
            //Save to collidePos the collided object transform
            Vector3 collidePos = collided.transform.position;
            //towardDirection between agent and object collided with
            Vector3 towardCollided = collidePos - agent.transform.position;
            towardCollided = towardCollided.normalized;

            float direct = Vector3.Dot(towardCollided, agent.transform.right.normalized);
            if (direct > 0) //object is to agent's left
            {
                Debug.Log("Direction float is right:" + direct);
                newDirection = Quaternion.Euler(0f, -80f, 0f) * agent.transform.position; //adjust direction left from current direction
            }
            else
            {
                Debug.Log("Direction float is left:" + direct);
                newDirection = Quaternion.Euler(0f, 80f, 0f) * agent.transform.position;
            }
            Debug.Log("my rotation is:" + newDirection);

            //Vector3 newDirection = Quaternion.Euler(0f, direct, 0f) * agent.transform.forward;

            Quaternion newRotation = Quaternion.LookRotation(newDirection);

            agent.transform.rotation = newRotation;

            agent.transform.position = agent.transform.position - towardCollided;
        }
    }
}
