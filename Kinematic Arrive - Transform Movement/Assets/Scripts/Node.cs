using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class node
{

    //values for each of node
    int nodeNum;
    int x; //x coordinate
    int y; //y coordinate
    bool pathable; //if the node is unpathable
    int H; //Heuristic 
    int P; //parent node
    int G; //Distance from goal
    int F;
    bool isStart;
    bool isGoal;


    public node(int nodeNum, int x, int y)
    {
        this.nodeNum = nodeNum;
        this.x = x;
        this.y = y;
    }

    public node(int nodeNum, int x, int y, int F)
    {
        this.nodeNum = nodeNum;
        this.x = x;
        this.y = y;
        this.F = F;
    }

    public node(int nodeNum, int x, int y, bool pathable, int H, int P, int G)
    {
        this.nodeNum = nodeNum;
        this.x = x;
        this.y = y;
        this.pathable = pathable;
        this.H = H;
        this.P = P;
        this.G = G;
        this.F = G + H;

    }

    //********************GETTERS************************* */

    //return the x coord
    public int getX()
    {
        return x;
    }

    //return the y coord
    public int getY()
    {
        return y;
    }

    //return the Node Number
    public int getNodeNum()
    {
        return nodeNum;
    }

    //return whether the node is pathable
    public bool getPath()
    {
        return pathable;
    }

    //return whether the node is the start
    public bool getIsStart()
    {
        return isStart;
    }

    //return whether the node is the goal
    public bool getIsGoal()
    {
        return isGoal;
    }

    //return the Heuristic
    public int getH()
    {
        return H;
    }

    //return the parent node Number
    public int getP()
    {
        return P;
    }

    //return the G number (distance from goal)
    public int getG()
    {
        return G;
    }

    //return the F number (caluculate Heuristic)
    public int getF()
    {
        return F;
    }

    /****************SETTERS******************** */

    public void reset()
    {
        this.H = 0;
        this.P = 0;
        this.G = 0;
        this.F = 0;
        this.isGoal = false;
        this.isStart = false;
    }


    //set x coordinate
    public void setX(int x)
    {
        this.x = x;
    }

    //set the y coord
    public void setY(int y)
    {
        this.y = y;
    }

    //set the Node Number
    public void setNodeNum(int nodeNum)
    {
        this.nodeNum = nodeNum;
    }

    //set whether the node is pathable
    public void setPathable(bool pathable)
    {
        this.pathable = pathable;
    }

    //set whether the node is the start
    public void setIsStart(bool isStart)
    {
        this.isStart = isStart;
    }

    //set whether the node is the Goal
    public void setIsGoal(bool isGoal)
    {
        this.isGoal = isGoal;
    }

    //set the Heuristic
    public void setH(int H)
    {
        this.H = H;
    }

    //set the parent node Number
    public void setP(int P)
    {
        this.P = P;
    }

    //set the G number (distance from goal)
    public void setG(int G)
    {
        this.G = G;
    }

    //set the F number (caluculate Heuristic)
    public void setF(int F)
    {
        this.F = F;
    }


}
