using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blockPlace : MonoBehaviour
{
    [SerializeField] private GameObject blockade;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject blockadePrefab;
    [SerializeField] private Transform CharTransform;
    private WorldDecomposer Decompose = new WorldDecomposer();
    private bool notDown = true;
    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        blockPlacement(notDown);
        
        
    }


    private void blockPlacement(bool notDown)
    {
        if (Input.GetMouseButtonDown(1) && notDown)
        {

            Vector2 mouse = Input.mousePosition;
            
            RaycastHit hit1;
            //take mouse position from camera position
            Ray ray = mainCamera.ScreenPointToRay(mouse);
            //shoot 1000f ray
            if (Physics.Raycast(ray, out hit1, 1000f))
            {
                //Debug what object was hit
                Debug.Log(hit1.transform.gameObject.name);
                //update target position
                Vector3 blockTransform = new Vector3(hit1.point.x, 0.5f, hit1.point.z);
                Vector3 towardChar = blockTransform - CharTransform.position;
                Quaternion lookRotation = Quaternion.LookRotation(towardChar);
                GameObject blockade = GameObject.Instantiate(blockadePrefab, blockTransform, lookRotation);
                
                blockade.transform.position = new Vector3(hit1.point.x, 0.5f, hit1.point.z);
            }
            
        }
    }
}
