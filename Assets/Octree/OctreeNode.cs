using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct OctreeObject
{
    public Bounds bounds;
    public GameObject gameObject;

    public OctreeObject(GameObject go)
    {
        bounds = go.GetComponent<Collider>().bounds;
        gameObject = go;
    }
}

public class OctreeNode
{

    Bounds[] childBounds;
    float minSize;
    public List<OctreeObject> containedObjects = new List<OctreeObject>();

    public Bounds nodeBounds;
    public OctreeNode[] children = null;
    public int id;

    public OctreeNode(Bounds b, float minNodeSize)
    {
        nodeBounds = b;
        minSize = minNodeSize;
        id = IDGenerate.GetId();

        float quarter = nodeBounds.size.y / 4f;
        float childLength = nodeBounds.size.y / 2f;

        Vector3 childSize = new Vector3(childLength, childLength, childLength);
        childBounds = new Bounds[8];
        childBounds[0] = new Bounds(nodeBounds.center + new Vector3(-quarter, quarter, -quarter), childSize);
        childBounds[1] = new Bounds(nodeBounds.center + new Vector3(quarter, quarter, -quarter), childSize);
        childBounds[2] = new Bounds(nodeBounds.center + new Vector3(-quarter, quarter, quarter), childSize);
        childBounds[3] = new Bounds(nodeBounds.center + new Vector3(quarter, quarter, quarter), childSize);
        childBounds[4] = new Bounds(nodeBounds.center + new Vector3(-quarter, -quarter, -quarter), childSize);
        childBounds[5] = new Bounds(nodeBounds.center + new Vector3(quarter, -quarter, -quarter), childSize);
        childBounds[6] = new Bounds(nodeBounds.center + new Vector3(-quarter, -quarter, quarter), childSize);
        childBounds[7] = new Bounds(nodeBounds.center + new Vector3(quarter, -quarter, quarter), childSize);
    }

    public void AddObject(GameObject go)
    {
        DivideAndAdd(go);
    }

    public void DivideAndAdd(GameObject go)
    {
        OctreeObject octObj = new OctreeObject(go);
        if (nodeBounds.size.y <= minSize)
        {
            containedObjects.Add(octObj);
            return;
        }

        if (children == null)
            children = new OctreeNode[8];

        bool dividing = false;

        for (int i = 0; i < 8; i++)
        {
            if (children[i] == null)
                children[i] = new OctreeNode(childBounds[i], minSize);

            //if (childBounds[i].Intersects(octObj.bounds))

            if (childBounds[i].Contains(octObj.bounds.min) && childBounds[i].Contains(octObj.bounds.max))
            {
                dividing = true;
                children[i].DivideAndAdd(go);
            }
        }
        if (dividing == false)
        {
            containedObjects.Add(octObj);
            children = null;
        }

    }

    public void Draw()
    {
        Gizmos.color = new Color(0, 1, 0);
        Gizmos.DrawWireCube(nodeBounds.center, nodeBounds.size);
        Gizmos.color = new Color(1, 0, 0);

        foreach (OctreeObject child in containedObjects)
        {
            Gizmos.DrawCube(child.bounds.center, child.bounds.size);
        }

        if (children != null)
        {
            for (int i = 0; i < 8; i++)
            {
                if (children[i] != null)
                    children[i].Draw();
            }
        }
        else if( containedObjects.Count != 0)
        {
            Gizmos.color = new Color(0, 0, 1, 0.25f);
            Gizmos.DrawCube(nodeBounds.center, nodeBounds.size);
        }
    }


}
