using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOctree : MonoBehaviour
{
    public GameObject[] worldObjects;
    public int nodeMinSize = 5;
    Octree ot;
    Graph wayPoints;

    private void Start()
    {
        wayPoints = new Graph();
        ot = new Octree(worldObjects, nodeMinSize, wayPoints);
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying)
        {
            Gizmos.color = new Color(0, 1, 0);
            ot.rootNode.Draw();
            ot.navigationGraph.Draw();
        }
    }
}
