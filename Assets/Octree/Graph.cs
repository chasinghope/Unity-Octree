using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

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


    public bool AStar(OctreeNode startNode, OctreeNode endNode)
    {
        pathList.Clear();
        Node start = FindNode(startNode.id);
        Node end = FindNode(endNode.id);

        if (start == null || end == null)
        {
            return false;
        }

        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();

        float tentative_g_score = 0;
        bool tentative_is_better;

        start.g = 0;
        start.h = Vector3.SqrMagnitude(startNode.nodeBounds.center - endNode.nodeBounds.center);
        start.f = start.h + start.g;

        open.Add(start);

        while (open.Count > 0)
        {
            int i = LowestF(open);
            Node curNode = open[i];
            if(curNode.octreeNode.id == endNode.id)
            {
                ReconstructPath(start, end);
                return true;
            }

            open.RemoveAt(i);
            closed.Add(curNode);

            Node neighbour;
            foreach (Edge e in curNode.edgeList)
            {
                neighbour = e.endNode;

                if(neighbour.g != 0)
                {
                    neighbour.g = curNode.g + Vector3.SqrMagnitude(curNode.octreeNode.nodeBounds.center -
                                               neighbour.octreeNode.nodeBounds.center);
                }

                if (closed.IndexOf(neighbour) > -1)
                    continue;

                tentative_g_score = curNode.g + Vector3.SqrMagnitude(curNode.octreeNode.nodeBounds.center -
                                                               neighbour.octreeNode.nodeBounds.center);

                if (open.IndexOf(neighbour) == -1)
                {
                    open.Add(neighbour);
                    tentative_is_better = true;
                }
                else if(tentative_g_score < neighbour.g)
                {
                    tentative_is_better = true;
                }
                else
                {
                    tentative_is_better = false;
                }

                if(tentative_is_better)
                {
                    neighbour.comeFrom = curNode;
                    neighbour.g = tentative_g_score;
                    neighbour.h = Vector3.SqrMagnitude(curNode.octreeNode.nodeBounds.center -
                                                               endNode.nodeBounds.center);
                    neighbour.f = neighbour.g + neighbour.h;
                }
            }

        }
        return false;
    }

    public void ReconstructPath(Node startId, Node endId)
    {
        pathList.Clear();
        pathList.Add(endId);
        var p = endId.comeFrom;
        while( p !=startId && p != null)
        {
            pathList.Insert(0, p);
            p = p.comeFrom;
        }

        pathList.Insert(0, startId);
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

    int LowestF(List<Node> l)
    {
        float lowestf = 0;
        int count = 0;
        int iteractorCount = 0;

        for (int i = 0; i < l.Count; i++)
        {
            if( i== 0)
            {
                lowestf = l[i].f;
                iteractorCount = 0;
            }
            else if (l[i].f <= lowestf)
            {
                lowestf = l[i].f;
                iteractorCount = count;
            }
            count++;
        }
        return iteractorCount;
    }
}