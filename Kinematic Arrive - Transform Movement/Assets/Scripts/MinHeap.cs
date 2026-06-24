using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinHeap
{
    private List<node> heap;
    private int Size;
    private int max;
    private node node1;
    private static int FRONT = 1;

    public MinHeap() { }

    public MinHeap(int max)
    {

        this.max = max;
        Size = 0;

        this.heap = new List<node>(this.max + 1);
        node1 = new node(0, 0, 0, 0);
        this.heap.Insert(0, node1);

    }

    //grab the parent node of index pos
    private int parent(int pos) { return pos / 2; }

    private int leftNode(int pos) { return (pos * 2); }

    private int rightNode(int pos) { return ((pos * 2) + 1); }

    // To swap two nodes of the heap
    private void swap(node pos1, node pos2)
    {
        int fpos = heap.IndexOf(pos1);
        int spos = heap.IndexOf(pos2);

        node temp;
        temp = heap[fpos];

        set(fpos, pos2);
        set(spos, temp);
    }

    // node is a leaf node
    private bool isLeaf(int pos)
    {
        // if the position is less than the parent then return true
        if (pos > (Size / 2))
        {
            return true;
        }

        return false;
    }

    // To insert a node into the heap
    public void insert(node element)
    {

        if (Size >= max)
        {
            return;
        }

        heap.Insert(1, element);
        Size++;
        int current = Size;

        //compare F values for the new element to see if its smaller than its parent's F value
        while (heap[current].getF() < heap[parent(current)].getF())
        {
            swap(heap[current], heap[parent(current)]);
            current = parent(current);
        }
    }

    // To heapify the node at pos
    public void minHeapify(int pos)
    {
        if (!isLeaf(pos))
        {
            int swapPos = pos;
            // swap with the minimum of the two children
            // to check if right child exists. Otherwise default value will be '0'
            // and that will be swapped with parent node.
            if (rightNode(pos) <= Size)
            {
                swapPos = heap[leftNode(pos)].getF() < heap[rightNode(pos)].getF() ? leftNode(pos) : rightNode(pos);
            }
            else
            {
                swapPos = leftNode(pos);
            }
            if (heap[pos].getF() > heap[leftNode(pos)].getF() || heap[pos].getF() > heap[rightNode(pos)].getF())
            {
                swap(heap[pos], heap[swapPos]);
                minHeapify(swapPos);
            }

        }
    }

    public int size()
    {
        return this.Size;
    }

    public node get(int i)
    {
        return heap[i];
    }

    public void set(int i, node node)
    {
      
        heap[i] = node;
    }

    // To remove and return the minimum element from the heap
    public node remove()
    {

        node popped = heap[FRONT];
        set(FRONT, heap[Size--]);
        minHeapify(FRONT);
        return popped;
    }


}
