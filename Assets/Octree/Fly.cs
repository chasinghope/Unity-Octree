using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Fly : MonoBehaviour
{
    float speed = 1f;
    float accuracy = 1.0f;
    float rotSpeed = 5f;

    int currentWP = 1;
    Vector3 goal;
    OctreeNode currentNode;

    public GameObject octree;
    Graph graph;
    List<Node> pathList = new List<Node>();

    private void Start()
    {
        Invoke("Navigate", 3);
    }

    private void LateUpdate()
    {
        if (graph == null)
            return;

        if (GetPathLength() == 0 || currentWP == GetPathLength())
        {
            GetRandomDestination();
            return;
        }

        if(Vector3.Distance(GetPathPoint(currentWP).nodeBounds.center, this.transform.position) <= accuracy)
        {
            currentWP++;
        }

        if(currentWP < GetPathLength())
        {
            goal = GetPathPoint(currentWP).nodeBounds.center;
            currentNode = GetPathPoint(currentWP);

            Vector3 lookAtGoal = new Vector3(goal.x, goal.y, goal.z);
            Vector3 direction = lookAtGoal - this.transform.position;

            this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);
            this.transform.Translate(0, 0, speed * Time.deltaTime);
        }
        else
        {
            GetRandomDestination();
            if(GetPathLength() == 0)
            {
                Debug.Log("No path");
            }
        }
    }

    public int GetPathLength()
    {
        return pathList.Count;
    }
    public OctreeNode GetPathPoint(int index)
    {
        return pathList[index].octreeNode;
    }

    void Navigate()
    {
        graph = octree.GetComponent<CreateOctree>().wayPoints;
        currentNode = graph.nodes[currentWP].octreeNode;
        GetRandomDestination();
    }

    void GetRandomDestination()
    {
        int randnode = Random.Range(0, graph.nodes.Count);
        graph.AStar(graph.nodes[currentWP].octreeNode, graph.nodes[randnode].octreeNode, pathList);
        currentWP = 0;
    }
}