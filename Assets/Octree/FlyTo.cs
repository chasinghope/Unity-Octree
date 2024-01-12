using System.Collections.Generic;
using UnityEngine;

public class FlyTo : MonoBehaviour
{
    float speed = 1f;
    float accuracy = 0.1f;
    float rotSpeed = 15f;

    public int currentWP = 0;
    Vector3 goal;

    public GameObject octree;
    Graph graph;
    Octree ot;
    public List<Node> pathList = new List<Node>();

    public GameObject goalPosition;

    private void Start()
    {
        Invoke("Navigate", 1);
    }

    private void Update()
    {
        if (ot == null) return;
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 pos = goalPosition.transform.position;
            int i = ot.AddDesitination(pos);
            if(i == -1)
            {
                Debug.Log($"Destination not found in Octree");
                return;
            }
            Node finalGoal = new Node(new OctreeNode(new Bounds(pos, new Vector3(0.1f, 0.1f, 0.1f))));
            NaviagteTo(i, finalGoal);
            Debug.Log(finalGoal.octreeNode.nodeBounds.center);
        }

    }

    private void LateUpdate()
    {
        if(graph == null ) return;
        if(GetPathLength() == 0 || currentWP == GetPathLength())
        {
            return;
        }
        
        if(Vector3.Distance(GetPathPoint(currentWP).nodeBounds.center, 
            transform.position) <= accuracy)
        {
            currentWP++;
        }

        if(currentWP < GetPathLength())
        {
            goal = GetPathPoint(currentWP).nodeBounds.center;
            Vector3 lootAtGoal = new Vector3(goal.x, goal.y, goal.z);
            Vector3 direction = lootAtGoal - this.transform.position;
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);
            this.transform.Translate(0, 0, speed * Time.deltaTime);
        }
        else
        {
            if(GetPathLength() == 0 )
            {
                return;
            }
        }
    }

    void Navigate()
    {
        graph = octree.GetComponent<CreateOctree>().wayPoints;
        ot = octree.GetComponent<CreateOctree>().ot;

    }


    void NaviagteTo(int destination, Node finalGoal)
    {
        Node desinationNode = graph.FindNode(destination);
        graph.AStar(graph.nodes[currentWP].octreeNode, desinationNode.octreeNode, pathList);
        currentWP = 0;
        pathList.Add(finalGoal);
        Debug.Log("path Count " + pathList.Count);
    }

    int GetPathLength()
    {
        return pathList.Count;
    }

    OctreeNode GetPathPoint(int index)
    {
        return pathList[index].octreeNode;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1);
        if(goalPosition != null)    
            Gizmos.DrawWireCube(goalPosition.transform.position, Vector3.one * 0.1f);
    }

}