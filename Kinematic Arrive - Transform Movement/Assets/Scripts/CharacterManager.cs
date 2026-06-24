using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour {

    private float moveSpeed;
    private float radiusOfSatisfaction;
    private WorldDecomposer worldDecom;
    private int[,] map;
    private int rows;
    private int cols;
    private int currentX;
    private int currentY;
    private int currentZ;
    private Vector3Int currentPosition;
    private int nodeSize;


    private bool keepPlaying;

    [SerializeField] private Transform myTransform;
    [SerializeField] private GameObject targetGameObject;
    [SerializeField] private Transform targetTransform;

    void Start() 
    {
        moveSpeed = 6f;
        radiusOfSatisfaction = 2f;
        worldDecom = new WorldDecomposer();
        rows = worldDecom.getRows();
        cols = worldDecom.cols;
        map = worldDecom.getWorldData();
        nodeSize = worldDecom.getNodeSize();

        keepPlaying = true;

        currentPosition = getCurrentCoord();

        this.transform.position = new Vector3 (currentPosition.x, this.transform.position.y, currentPosition.y);

    }

    void Update() 
    {
        if (Input.GetMouseButton(0))
        {
            //AStar();
            print("number of rows are:" + rows);
        }
        RunKinematicArrive();
    }

    private void RunKinematicArrive () 
    {
        // Create vector from character to target
        Vector3 towardsTarget = targetTransform.position - myTransform.position;

        // Check to see if the character is close enough to the target
        if (towardsTarget.magnitude > radiusOfSatisfaction)
        {



            float magnitude = (towardsTarget.magnitude);

            // Normalize vector so we only use the direction
            towardsTarget = towardsTarget.normalized;

            // Face the target
            Quaternion targetRotation = Quaternion.LookRotation(towardsTarget);
            myTransform.rotation = Quaternion.Lerp(myTransform.rotation, targetRotation, 0.1f);

            // Move along our forward vector (the direction we're facing)
            Vector3 newPosition = myTransform.position;
            newPosition += myTransform.forward * moveSpeed * Time.deltaTime;

            myTransform.position = newPosition;
        }
        this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //close enough


    }

    private Vector3Int getCurrentCoord()
    {
        currentX = (int)(this.transform.position.x);
        currentY = (int)(this.transform.position.y);
        currentZ = (int)(this.transform.position.z);

        if (currentX % 2 != 0)
        {
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
    
    
    private void AStar()
    {
        
        MinHeap openList = new MinHeap(630);

        List<node> closedList = new List<node>();

        targetControl target = new targetControl();

       

        int startCapture = worldDecom.getStartX() + (worldDecom.getStartZ() * 24);

        int goalCapture = worldDecom.getGoalX() + (worldDecom.getGoalZ() * 24);

        bool keepSearching = true;

        bool notPathable = false;

        //******************************Below is the algorithm for finding the best path**************************************

        node startNode = worldDecom.nodeList()[startCapture];

        node goalNode = worldDecom.nodeList()[goalCapture];

        int parentSave = 0;

        worldDecom.setNodeAttributes(startNode, goalNode);

        if (startNode.getIsGoal())
        {
            keepSearching = false;
        }

        node currentNode = startNode;

        openList.insert(currentNode);

        int count = 0;
        //begin searching algorithm loop***********************************************************************************************
        while (keepSearching)
        {
            if(count == 600) //failsafe if the path is never found and infinite loop occures
            {
                keepSearching = false;
                notPathable = true;
                break;
            }

            for (int i = 1; i < openList.size() - 1; i++)//check to find that the item in open list isn't already the goal
            {
                if (openList.get(i).getH() == 0)
                {
                    currentNode = openList.get(i); //set the currentNode to be the goal node
                }
            }

            if (currentNode.getIsGoal())//check if the currentNode is the goal node
            {
                keepSearching = false;
                print("Node was found!------------------------------");
                goalNode = currentNode;
                parentSave = currentNode.getP();
                break;
            }

            if (openList.size() < 1) //if the openlist closes before the goal node is found
            {
                notPathable = true;
                keepSearching = false;
                break;
            }

            currentNode = openList.remove();//pop off from the open list for the currentNode

            print("[" + currentNode.getX() + "," + currentNode.getY() + "], "); //print out the current Node

            for (int i = 1; i < openList.size() - 1; i++) //go through openlist
            {
                if (openList.get(i).getF() == currentNode.getF()) //check if there is the same node already in open list
                {
                    openList.get(i).setF(openList.get(i).getF() + 40); //make the calculated heuristic unrealisticly high
                    break;
                }
            }

            if (currentNode.getIsGoal())//check if current node is the goal node
            {
                keepSearching = false;
                print("Node was found!-----------------------------------------------------------");
                goalNode = currentNode;
                parentSave = currentNode.getP();
                break;
            }


            //grab the currentNode that is currently being checked its surrounding possible node paths
            //Grab node top
            node topNode = new node(0, 0, 0);
            worldDecom.getTopNode(currentNode, topNode);
            //Grab node right
            node rightNode = new node(0, 0, 0);
            worldDecom.getRightNode(currentNode, rightNode);
            //Grab node left
            node leftNode = new node(0, 0, 0);
            worldDecom.getLeftNode(currentNode, leftNode);
            //Grab node bottom
            node bottomNode = new node(0, 0, 0);
            worldDecom.getBottomNode(currentNode, bottomNode);
            //Grab node up-right
            node upRightNode = new node(0, 0, 0);
            worldDecom.getUpRightNode(currentNode, upRightNode);
            //Grab node up-left
            node upLeftNode = new node(0, 0, 0);
            worldDecom.getUpLeftNode(currentNode, upLeftNode);
            //Grab node bottom-right
            node downRightNode = new node(0, 0, 0);
            worldDecom.getDownRightNode(currentNode, downRightNode);
            //Grab node bottom-left
            node downLeftNode = new node(0, 0, 0);
            worldDecom.getDownLeftNode(currentNode, downLeftNode);





            //check validity of topNode and add it to openlist
            if (!(topNode.getNodeNum() == -1) && (worldDecom.checkClosedList(closedList, topNode)))
            {

                worldDecom.setNodeAttributes(topNode, goalNode);

                if (!worldDecom.checkIfNodeStrays(topNode, currentNode, goalNode))
                {
                    openList.insert(topNode);
                }

            }

            //check validity of rightNode and add it to openlist
            //closedList.indexOf(rightNode)<0)
            if (!(rightNode.getNodeNum() == -1) && (worldDecom.checkClosedList(closedList, rightNode)))
            {

                worldDecom.setNodeAttributes(rightNode, goalNode);

                if (!worldDecom.checkIfNodeStrays(rightNode, currentNode, goalNode))
                {
                    openList.insert(rightNode);
                }
            }

            //check validity of leftNode and add it to openlist
            if (!(leftNode.getNodeNum() == -1) && (worldDecom.checkClosedList(closedList, leftNode)))
            {

                worldDecom.setNodeAttributes(leftNode, goalNode);

                if (!worldDecom.checkIfNodeStrays(leftNode, currentNode, goalNode))
                {
                    openList.insert(leftNode);
                }
            }

            //check validity of bottomNode and add it to openlist
            if (!(bottomNode.getNodeNum() == -1) && (worldDecom.checkClosedList(closedList, bottomNode)))
            {

                worldDecom.setNodeAttributes(bottomNode, goalNode);

                if (!worldDecom.checkIfNodeStrays(bottomNode, currentNode, goalNode))
                {
                    openList.insert(bottomNode);
                }
            }

            //check validity of upRightNode and add it to openlist
            if (!(upRightNode.getNodeNum() == -1) && (worldDecom.checkClosedList(closedList, upRightNode)))
            {

                worldDecom.setNodeAttributes(upRightNode, goalNode);

                if (!worldDecom.checkIfNodeStrays(upRightNode, currentNode, goalNode))
                {
                    openList.insert(upRightNode);
                }
            }

            //check validity of upLeftNode and add it to openlist
            if (!(upLeftNode.getNodeNum() == -1) && (worldDecom.checkClosedList(closedList, upLeftNode)))
            {

                worldDecom.setNodeAttributes(upLeftNode, goalNode);

                if (!worldDecom.checkIfNodeStrays(upLeftNode, currentNode, goalNode))
                {
                    openList.insert(upLeftNode);
                }
            }

            //check validity of downRightNode and add it to openlist
            if (!(downRightNode.getNodeNum() == -1) && (worldDecom.checkClosedList(closedList, downRightNode)))
            {

                worldDecom.setNodeAttributes(downRightNode, goalNode);

                if (!worldDecom.checkIfNodeStrays(downRightNode, currentNode, goalNode))
                {
                    openList.insert(downRightNode);
                }
            }

            //check validity of downLeftNode and add it to openlist
            if (!(downLeftNode.getNodeNum() == -1) && (worldDecom.checkClosedList(closedList, downLeftNode)))
            {

                worldDecom.setNodeAttributes(downLeftNode, goalNode);

                if (!worldDecom.checkIfNodeStrays(downLeftNode, currentNode, goalNode))
                {
                    openList.insert(downLeftNode);
                }
            }


            closedList.Add(currentNode); // add to closed List the startNode

            openList.minHeapify(1);


            count++;

        }//end of while loop



        bool tracePath = true;

        node currentParent = goalNode;

        currentParent.setP(parentSave);

        print("...Generating path");
        //Trace the path of the parent nodes back to the start:
        while (tracePath)
        {

            //find the parent node and make it the new currentParent here:
            for (int i = (closedList.Capacity - 1); i > -1; i--)
            {

                if (closedList[i].getNodeNum() == currentParent.getP())
                {
                    currentParent = closedList[i];
                    closedList.RemoveAt(i);
                    i = -1;
                }
            }



            //check if the current parent is back at the starting node
            if (currentParent.getIsStart())
            {
                tracePath = false;
                break;
            }
            //print out the path at current Parent.
            worldDecom.createPath(currentParent);
            print(".");

        }// end of generating path
        print("");
        print("___________________________________________________________________________________________________");

        if (notPathable)
        {
            print("Unable to generate path... Sorry for the inconvenience.");

        }
        else
        {
            
            print("--------------------------Path Traced!-------------------------------");
        }

        //call kinematic arrive to each of the following pathList Nodes;

        List<node> pathList = worldDecom.getPathList();

        for (int i = 0; i < pathList.Capacity; i++){

            bool goToTarget = true;

            node currentTarget = pathList[i];

            Vector3 targetPosition = new Vector3(currentTarget.getX(), targetTransform.position.y, currentTarget.getY());

            targetPosition = targetPosition.normalized;

            while (goToTarget)// bool to check if made it to next node
            {
                // Create vector from character to target
                Vector3 towardsTarget = targetPosition - myTransform.position;

                // Check to see if the character is close enough to the target
                if (towardsTarget.magnitude > radiusOfSatisfaction)
                {
                    //RunKinematicArrive(towardsTarget, 0.001f);

                }
                else
                {
                    goToTarget = false;
                }
            }
            this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //close enough

        }

    }
}
