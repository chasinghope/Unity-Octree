using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Vector3 sizeVector = new Vector3(maxSize, maxSize, maxSize) * 0.5f;
        bounds.SetMinMax(bounds.center - sizeVector, bounds.center + sizeVector);

        rootNode = new OctreeNode(bounds, minNodeSize);
        AddObejct(worldObjects);
        GetEmptyLeaves(rootNode);
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
            }
        }
    }
}
