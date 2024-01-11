using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOctree : MonoBehaviour
{
    public GameObject[] worldObjects;

    Octree ot;

    private void Start()
    {
        ot = new Octree(worldObjects, 1);
    }

    private void OnDrawGizmos()
    {
        
    }
}
