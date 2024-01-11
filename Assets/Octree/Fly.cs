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

    private void Start()
    {
        Invoke("Navigate", 3);
    }

    private void LateUpdate()
    {
        if (graph == null)
            return;

        if (graph.GetPathLength() == 0 || currentWP == graph.GetPathLength())
        {
            GetRandomDestination();
            return;
        }

        if(Vector3.Distance(graph.GetPathPoint(currentWP).nodeBounds.center, this.transform.position) <= accuracy)
        {
            currentWP++;
        }

        if(currentWP < graph.GetPathLength())
        {
            goal = graph.GetPathPoint(currentWP).nodeBounds.center;
            currentNode = graph.GetPathPoint(currentWP);

            Vector3 lookAtGoal = new Vector3(goal.x, goal.y, goal.z);
            Vector3 direction = lookAtGoal - this.transform.position;

            this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);
            this.transform.Translate(0, 0, speed * Time.deltaTime);
        }
        else
        {
            GetRandomDestination();
            if(graph.GetPathLength() == 0)
            {
                Debug.Log("No path");
            }
        }
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
        graph.AStar(graph.nodes[currentWP].octreeNode, graph.nodes[randnode].octreeNode);
        currentWP = 0;
    }
}