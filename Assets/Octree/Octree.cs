using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Octree
{
    public OctreeNode rootNode;
    public List<OctreeNode> emptyLeaves = new List<OctreeNode>();
    public Graph navigationGraph;

    public Octree(GameObject[] worldObjects, float minNodeSize, Graph navGraph)
    {
        Bounds bounds = new Bounds();
        navigationGraph = navGraph;
        foreach (GameObject go in worldObjects)
        {
            bounds.Encapsulate(go.GetComponent<Collider>().bounds);
        }


        float maxSize = Mathf.Max(new float[] { bounds.size.x, bounds.size.y, bounds.size.z});
        //Vector3 sizeVector = new Vector3(maxSize, maxSize, maxSize) * 0.5f;
        Vector3 sizeVector = new Vector3(maxSize, maxSize, maxSize) * 1f;
        bounds.SetMinMax(bounds.center - sizeVector, bounds.center + sizeVector);

        rootNode = new OctreeNode(bounds, minNodeSize, null);
        AddObejct(worldObjects);
        GetEmptyLeaves(rootNode);
        //ProcessExtraConnections();
        ConnectLeafNodeNeighbours();
    }

    public void AddObejct(GameObject[] worldObjects)
    {
        foreach (GameObject go in worldObjects)
        {
            rootNode.AddObject(go);
        }
    }

    public void GetEmptyLeaves(OctreeNode octreeNode)
    {
        if (octreeNode == null)
            return;

        if(octreeNode.children == null)
        {
            if(octreeNode.containedObjects.Count == 0)
            {
                emptyLeaves.Add(octreeNode);
                navigationGraph.AddNode(octreeNode);
            }
        }
        else
        {
            for (int i = 0; i < 8; i++)
            {
                GetEmptyLeaves(octreeNode.children[i]);

                /*
                for (int s = 0; s < 8; s++)
                {
                    if(s != i)
                    {
                        navigationGraph.AddEdge(octreeNode.children[i], octreeNode.children[s]);
                    }
                }
                */
            }
        }
    }

    public int FindBindingNode(OctreeNode node, Vector3 position)
    {
        int found = -1;
        if (node == null) return -1;
        if(node.children == null)
        {
            if(node.nodeBounds.Contains(position) && node.containedObjects.Count == 0)
            {
                return node.id;
            }
        }
        else
        {
            for(int i = 0; i < 8;i++)
            {
                found = FindBindingNode(node.children[i], position);
                if (found != -1)
                    break;
            }
        }
        return found;
    }

    void ProcessExtraConnections()
    {
        Dictionary<int, int> subGraphConnections = new Dictionary<int, int>();
        foreach (OctreeNode i in emptyLeaves)
        {
            foreach (OctreeNode j in emptyLeaves)
            {
                if(i.id != j.id && i.parent.id != j.parent.id)
                {
                    RaycastHit hitInfo;
                    Vector3 direction = j.nodeBounds.center - i.nodeBounds.center;
                    float accuracy = 1;
                    if(!Physics.SphereCast(i.nodeBounds.center, accuracy, direction, out hitInfo))
                    {
                        if(subGraphConnections.TryAdd(i.parent.id, j.parent.id))
                        {
                            navigationGraph.AddEdge(i, j);
                        }

                    }
                }
            }
        }
    } 

    void ConnectLeafNodeNeighbours()
    {
        List<Vector3> rays = new List<Vector3>()
        {
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, -1, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 0, -1),
        };

        for (int i = 0; i < emptyLeaves.Count; i++)
        {
            List<OctreeNode> neighbours = new List<OctreeNode>();
            for (int j = 0; j < emptyLeaves.Count; j++)
            {
                if (i == j)
                    continue;

                for (int r = 0; r < 6; r++)
                {
                    Ray ray = new Ray(emptyLeaves[i].nodeBounds.center, rays[r]);
                    float maxLength = emptyLeaves[i].nodeBounds.size.y / 2.0f + 0.01f;
                    float hitLegnth;
                    if (emptyLeaves[j].nodeBounds.IntersectRay(ray, out hitLegnth))
                    {
                        if(hitLegnth < maxLength)
                        {
                            neighbours.Add(emptyLeaves[j]);
                        }
                    }
                }
            }

            //Debug.Log($"neighboursCout: {neighbours.Count}");
            foreach (OctreeNode n in neighbours)
            {
                navigationGraph.AddEdge(emptyLeaves[i], n);
            }

        }

        
    }

    public int AddDesitination(Vector3 destination)
    {
        return FindBindingNode(rootNode, destination);
    }
}
