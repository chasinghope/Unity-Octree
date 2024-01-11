using System;
using System.Collections.Generic;

public class Node
{
    public List<Edge> edgeList = new List<Edge>();
    public Node path = null;
    public OctreeNode octreeNode;
    public float f, g, h;
    public Node comeFrom;

    public Node(OctreeNode n)
    {
        octreeNode = n;
        path = null;
    }

    public OctreeNode GetNode()
    {
        return octreeNode;
    }
}