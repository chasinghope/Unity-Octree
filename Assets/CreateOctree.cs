using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOctree : MonoBehaviour
{
    public GameObject[] worldObjects;
    public int nodeMinSize = 5;
    Octree ot;

    private void Start()
    {
        ot = new Octree(worldObjects, nodeMinSize);
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying)
        {
            Gizmos.color = new Color(0, 1, 0);
            ot.rootNode.Draw();
        }
    }
}
