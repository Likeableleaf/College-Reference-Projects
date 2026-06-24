using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDecomposer : MonoBehaviour
{
	private List<node> nodes = new List<node>(625);
	private List<node> pathList = new List<node>(625);
	private int xStart = 0;
	private int zStart = 0;
	private targetControl targetControl = new targetControl();

	private int[,] map;
	private int nodeSize;
	private int checkStart;
	private int checkGoal;

	private int terrainWidth;
	private int terrainLength;

	public int rows;
	public int cols;

	private void Start()
	{
		
		terrainWidth = 50;
		terrainLength = 50;
		
		nodeSize = 2;

		rows = terrainWidth / nodeSize;
		cols = terrainLength / nodeSize;

		map = new int[rows, cols];

		DecomposeWorld();
	}

    private void Update()
    {
		if (Input.GetMouseButton(1))
		{

			DecomposeWorld();
		}
	}

    private void DecomposeWorld()
	{
		int count = 0;
		float startX = -25;
		float startZ = -25;

		float nodeCenterOffset = nodeSize / 2f;


		for (int row = 0; row < rows; row++)
		{

			for (int col = 0; col < cols; col++)
			{
				
				float x = startX + nodeCenterOffset + (nodeSize * col);
				float z = startZ + nodeCenterOffset + (nodeSize * row);

				Vector3 startPos = new Vector3(x, 20f, z);

				node node = new node(count,((int)x), (int)z); //set the node number, its x and y (z)


				// Does our raycast hit anything at this point in the map

				RaycastHit hit;

				// Bit shift the index of the layer (3) to get a bit mask
				int layerMask = 1 << 3;

				// This would cast rays only against colliders in layer 3.
				// But instead we want to collide against everything except layer 3. The ~ operator does this, it inverts a bitmask.
				layerMask = ~layerMask;

				// Does the ray intersect any objects excluding the player layer
				if (Physics.Raycast(startPos, Vector3.down, out hit, Mathf.Infinity, layerMask))
				{

					if (hit.collider.CompareTag("Player")) { 

						print("Hit the character at row: " + row + " col: " + col);
						Debug.DrawRay(startPos, Vector3.down * 20, Color.blue, 50000);
						map[row, col] = 2;
						xStart = row;
						zStart = col;
						node.setPathable(true);
						
                    }
                    else
                    {
						print("Hit something at row: " + row + " col: " + col);
						Debug.DrawRay(startPos, Vector3.down * 20, Color.red, 50000);
						map[row, col] = 1;
						node.setPathable(false);
					}

				}
				else
				{
					Debug.DrawRay(startPos, Vector3.down * 19, Color.green, 50000);
					map[row, col] = 0;
					node.setPathable(true);
				}

				nodes.Insert(count, node);
				count++;
			}
		}

		checkStart = setStart(xStart,zStart);

		checkGoal = setGoal(targetControl.getGoalPosX(), targetControl.getGoalPosZ());


		print("checkGoal : " + checkGoal);
		print("checkStart : " + checkStart);
	}

	public int getCheckStart()
    {
		return checkStart;
    }

	public int getCheckGoal()
    {
		return checkGoal;
    }

	public int getStartX()
    {
		return xStart;
    }
	public int getStartZ()
	{
		return zStart;
	}

	public int getGoalX()
    {
		return targetControl.getGoalPosX();
    }

	public int getGoalZ()
    {
		return targetControl.getGoalPosZ();
    }

	public int[,] getWorldData()
    {
		return map;
    }

	public int getNodeSize()
    {
		return nodeSize;
    }

	public List<node> nodeList()
	{
		return nodes;
	}

	public int getRows()
    {
		return rows;
    }

	public int setStart(int xStart, int zStart)
	{
		
		for (int i = 0; i < 25; i++)
		{
			
			for (int j = 0; j < 25; j++)
			{
				
				
				int nodeCapture = ((j) + (i * 25));
				
				if ((j == xStart) && (i == zStart)  && (nodes[nodeCapture].getPath() == true))
				{
					//map[i][j] = "O"
					//set the parent node to itself
					//int nodeNum = nodes.IndexOf(nodes[nodeCapture]);
					//print("nodeNum is" + nodeNum);
					nodes[nodeCapture].setP(nodes[nodeCapture].getNodeNum());

					//set IsStart boolean to be that it is the starting node

					nodes[nodeCapture].setIsStart(true);

					//returns the map now with the starting node represented by 'O'
					print("Start position was found! it is: col: " + xStart + "and row: " + zStart);
					return 1;
				}

				//if the given x and y are coordinates to an unpathable node
				if ((j == xStart) && (i == zStart) && (nodes[nodeCapture].getPath() == false))
				{
					print("Please try again, start selected is unpathable");
					return 0;
				}

			}


		}
		print("X and/or Y coordinates are not in bounds, try again");
		return -1; // the map is unchanged be cause the x and y are out of bounds.

	}

	public int setGoal(int xGoal, int yGoal)
	{
		for (int i = 0; i < 25; i++)
		{

			for (int j = 0; j < 25; j++)
			{

				int nodeCapture = ((j) + (i * 24));

				if ((j == xGoal) && (i == yGoal) && (nodes[nodeCapture].getPath() == true))
				{
					
					//set IsGoal boolean to be that it is the Goal node
					nodes[nodeCapture].setIsGoal(true);
					//returns the map now with the starting node represented by 'O'
					return 1;
				}

				//if the given x and y are coordinates to an unpathable node
				if ((j == xGoal) && (i == yGoal) && (nodes[nodeCapture].getPath() == false))
				{
					print("Please try again, Goal selected is unpathable");
					return 0;
				}

			}


		}
		print("X and/or Y coordinates are not in bounds, try again");
		return -1; // the map is unchanged be cause the x and y are out of bounds.

	}


	//grabs the top node
	public void getTopNode(node currentNode, node topNode)
	{

		if ((currentNode.getY() > 0) && ((nodes[currentNode.getNodeNum() - 24].getPath())))
		{

			topNode.setNodeNum(currentNode.getNodeNum() - 24);
			topNode.setX(currentNode.getX());
			topNode.setY(currentNode.getY() - 1);

			topNode.setG(7);
			topNode.setPathable(true);
			topNode.setP(currentNode.getNodeNum());
		}
		else
		{
			topNode.setNodeNum(-1);
		}


	}

	//grabs the right node
	public void getRightNode(node currentNode, node rightNode)
	{


		if ((currentNode.getX() < 23) && ((nodes[currentNode.getNodeNum() + 1].getPath())))
		{
			rightNode.setNodeNum(currentNode.getNodeNum() + 1);
			rightNode.setX(currentNode.getX() + 1);
			rightNode.setY(currentNode.getY());

			rightNode.setG(7);
			rightNode.setPathable(true);
			rightNode.setP(currentNode.getNodeNum());
		}
		else
		{
			rightNode.setNodeNum(-1);
		}

	}

	//grabs the left node
	public void getLeftNode(node currentNode, node leftNode)
	{


		if ((currentNode.getX() > 0) && ((nodes[currentNode.getNodeNum() - 1].getPath())))
		{
			leftNode.setNodeNum(currentNode.getNodeNum() - 1);
			leftNode.setX(currentNode.getX() - 1);
			leftNode.setY(currentNode.getY());

			leftNode.setG(7);
			leftNode.setPathable(true);
			leftNode.setP(currentNode.getNodeNum());
		}
		else
		{
			leftNode.setNodeNum(-1);
		}

	}

	//grabs the bottom node
	public void getBottomNode(node currentNode, node bottomNode)
	{

		if ((currentNode.getY() < 23) && ((nodes[currentNode.getNodeNum() + 24].getPath())))
		{

			bottomNode.setNodeNum(currentNode.getNodeNum() + 24);
			bottomNode.setX(currentNode.getX());
			bottomNode.setY(currentNode.getY() + 1);

			bottomNode.setG(7);
			bottomNode.setPathable(true);
			bottomNode.setP(currentNode.getNodeNum());
		}
		else
		{
			bottomNode.setNodeNum(-1);
		}


	}


	//grabs diagonal up right node
	public void getUpRightNode(node currentNode, node upRightNode)
	{

		if ((currentNode.getY() > 0) && (currentNode.getX() < 23) && ((nodes[currentNode.getNodeNum() - 23].getPath())))
		{

			upRightNode.setNodeNum(currentNode.getNodeNum() - 23);
			upRightNode.setX(currentNode.getX() + 1);
			upRightNode.setY(currentNode.getY() - 1);

			upRightNode.setG(14);
			upRightNode.setPathable(true);
			upRightNode.setP(currentNode.getNodeNum());
		}
		else
		{
			upRightNode.setNodeNum(-1);
		}


	}

	//grabs diagonal up left node
	public void getUpLeftNode(node currentNode, node upLeftNode)
	{

		if ((currentNode.getY() > 0) && (currentNode.getX() > 0) && ((nodes[currentNode.getNodeNum() - 25].getPath())))
		{

			upLeftNode.setNodeNum(currentNode.getNodeNum() - 25);
			upLeftNode.setX(currentNode.getX() - 1);
			upLeftNode.setY(currentNode.getY() - 1);

			upLeftNode.setG(14);
			upLeftNode.setPathable(true);
			upLeftNode.setP(currentNode.getNodeNum());
		}
		else
		{
			upLeftNode.setNodeNum(-1);
		}


	}

	public void getDownRightNode(node currentNode, node downRightNode)
	{

		if ((currentNode.getY() < 14) && (currentNode.getX() < 23) && ((nodes[currentNode.getNodeNum() + 25].getPath())))
		{

			downRightNode.setNodeNum(currentNode.getNodeNum() + 25);
			downRightNode.setX(currentNode.getX() + 1);
			downRightNode.setY(currentNode.getY() + 1);

			downRightNode.setG(14);
			downRightNode.setPathable(true);
			downRightNode.setP(currentNode.getNodeNum());
		}
		else
		{
			downRightNode.setNodeNum(-1);
		}


	}

	//grabs diagonal up right node
	public void getDownLeftNode(node currentNode, node downLeftNode)
	{

		if ((currentNode.getY() < 23) && (currentNode.getX() > 0) && ((nodes[currentNode.getNodeNum() + 23].getPath())))
		{

			downLeftNode.setNodeNum(currentNode.getNodeNum() + 23);
			downLeftNode.setX(currentNode.getX() - 1);
			downLeftNode.setY(currentNode.getY() + 1);

			downLeftNode.setG(14);
			downLeftNode.setPathable(true);
			downLeftNode.setP(currentNode.getNodeNum());
		}
		else
		{
			downLeftNode.setNodeNum(-1);
		}


	}

	//sets the attributes of a node in its respective place in the map before adding to openlist
	public void setNodeAttributes(node currentNode, node goal)
	{
		int distance = 0;
		int xDistance = currentNode.getX() - goal.getX();
		if (xDistance < 0) { xDistance = xDistance * -1; }
		int yDistance = currentNode.getY() - goal.getY();
		if (yDistance < 0) { yDistance = yDistance * -1; }

		distance = (yDistance) + xDistance;
		currentNode.setH((distance * 7));
		currentNode.setF(currentNode.getG() + currentNode.getH());
		if (currentNode.getH() == 0)
		{
			currentNode.setIsGoal(true);
		}
	}

	public bool checkIfNodeStrays(node currentNode, node previous, node goalNode)
	{
		if (goalNode.getX() > previous.getX())
		{
			if (currentNode.getX() < previous.getX())
			{
				return true;
			}
		}

		if (goalNode.getX() < previous.getX())
		{
			if (currentNode.getX() > previous.getX())
			{
				return true;
			}
		}

		if (goalNode.getY() > previous.getY())
		{
			if (currentNode.getY() < previous.getY())
			{
				return true;
			}
		}

		if (goalNode.getY() < previous.getY())
		{
			if (currentNode.getY() > previous.getY())
			{
				return true;
			}

		}

		return false;
	}

	//create the path.
	public void createPath(node currentParent)
	{
		int xPath = currentParent.getX();
		int yPath = currentParent.getY();
		bool mapNodeFound = false;
		//print out the path at current Parent.
		for (int i = 0; i < 25; i++)
		{

			for (int j = 0; j <25; j++)
			{

				if ((j == xPath) && (i == yPath))
				{
					//TODO: add in a way to add this node to ANOTHER list that will be taken by the character:
					/* the character will take this list of nodes of path, and follow it sequentially making each node a target
					// each target node it will go to with KinematicArrive till it reaches that node and  then goes to the
					//next node target.*/
					pathList.Add(currentParent);
					mapNodeFound = true;
					break;
				}
			}
			if (mapNodeFound)
			{
				break;
			}
		}

	}

	public bool checkClosedList(List<node> closedList, node currentNode)
	{
		for (int i = 0; i < closedList.Capacity - 1; i++)
		{
			if (closedList[i].getNodeNum() == currentNode.getNodeNum())
			{
				return false;
			}
		}
		return true;
	}

	public List<node> getPathList()
    {
		return pathList;
    }

}
