using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetControl : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Camera mainCamera;
    private int currentX;
    private int currentY;
    private int currentZ;
    private Vector3Int currentPosition;
    private int xPos;
    private int zPos; 

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        targetPlacement();
    }


    private void targetPlacement()
    {
        if (Input.GetMouseButton(0)) 
        {
           
            Vector2 mouse = Input.mousePosition;
           
            
            RaycastHit hit;
            //take mouse position from camera position
            Ray ray = mainCamera.ScreenPointToRay(mouse);
            //shoot 1000f ray
            if(Physics.Raycast(ray,out hit, 1000f))
            {
                //Debug what object was hit
                Debug.Log(hit.transform.gameObject.name);
                //update target position
                currentPosition = getCurrentCoord(hit.point.x, hit.point.y, hit.point.z);
                //targetTransform.position = new Vector3(hit.point.x, targetTransform.position.y, hit.point.z);
                targetTransform.position = new Vector3(currentPosition.x, this.transform.position.y, currentPosition.z);
                xPos = currentPosition.x;
                zPos = currentPosition.z;            
            }
            xPos = (xPos + 24) / 2;
            zPos = (zPos + 24) / 2;
            print("Goal position on map is: col: " + xPos + "row: " + zPos);
            
        }
    }


    private Vector3Int getCurrentCoord(float posX, float posY, float posZ)
    {
        currentX = (int)(posX);
        currentY = (int)(posY);
        currentZ = (int)(posZ);

        if (currentX % 2 != 0)
        {
            Debug.Log("Current X is " + currentX);
            currentX -= 1;
        }
        if (currentY % 2 != 0)
        {
            currentY -= 1;
        }
        if (currentZ % 2 != 0)
        {
            currentZ -= 1;
        }
        Vector3Int currentPos = new Vector3Int(currentX, currentY, currentZ);
        return currentPos;
    }

    public int getGoalPosX()
    {
        return xPos;
    }

    public int getGoalPosZ()
    {
        return zPos;
    }

}