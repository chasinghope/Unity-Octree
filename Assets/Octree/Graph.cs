using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    public List<Edge> edges = new List<Edge>();
    public List<Node> nodes = new List<Node>();

    List<Node> pathList = new List<Node>();

    public Graph()
    {
        
    }


    public void AddNode(OctreeNode otn)
    {
        if(FindNode(otn.id) == null)
        {
            Node node = new Node(otn);
            nodes.Add(node);
        }
    }

    public void AddEdge(OctreeNode fromNode,  OctreeNode toNode)
    {
        Node from = FindNode(fromNode.id);
        Node to = FindNode(toNode.id);
        
        if(from != null && to != null)
        {
            Edge e = new Edge(from, to);
            edges.Add(e);
            from.edgeList.Add(e);
            Edge f = new Edge(to, from);
            edges.Add(f);
            to.edgeList.Add(f);
        }
    }

    public int GetPathLength() => pathList.Count;

    public OctreeNode GetPathPoint(int index)
    {
        return pathList[index].octreeNode;
    }

    public void Draw()
    {
        Gizmos.color = new Color(1, 0, 0, 1);
        for (int i = 0; i < edges.Count; i++)
        {
            Gizmos.DrawLine(edges[i].startNode.octreeNode.nodeBounds.center, edges[i].endNode.octreeNode.nodeBounds.center);
            //Debug.DrawLine(edges[i].startNode.octreeNode.nodeBounds.center, edges[i].endNode.octreeNode.nodeBounds.center, Color.red);
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            Gizmos.color = new Color(1, 1, 0);
            Gizmos.DrawWireSphere(nodes[i].octreeNode.nodeBounds.center, 0.05f);
        }
    }

    Node FindNode(int otn_id)
    {
        foreach (Node n in nodes)
        {
            if (n.GetNode().id == otn_id)
                return n;
        }
        return null;
    }
}