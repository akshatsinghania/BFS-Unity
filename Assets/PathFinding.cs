using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    Grid gridComponent;
    public bool[,] visited;
    private void Awake()
    {
        gridComponent = GetComponent<Grid>();
    }
 
    public List<Node> FindPath(Vector3 start, Vector3 target)
    {
        Node startNode = gridComponent.NodeFromWorldPoint(start);
        Node targetNode = gridComponent.NodeFromWorldPoint(target);
        Node[,] prev = solve(startNode);
        return ReconstructPath(startNode, targetNode, prev);


    }
    List<Node> ReconstructPath(Node start,Node end,Node[,] prev)
    {
        List<Node> path = new List<Node>();
        for(Node at = end; at != null; at = prev[at.gridX, at.gridY])
        {
            //Debug.Log(new Vector2(at.gridX,at.gridY));
            path.Add(at);
        }
        return path;
    }
    public Node[,] solve(Node start)
    {
        Queue<Node> q = new Queue<Node>();
        q.Enqueue(start);

        visited = new bool[gridComponent.gridSizeX, gridComponent.gridSizeY];
        for(int i = 0; i < gridComponent.gridSizeX; i++)
        {
            for (int j = 0; j < gridComponent.gridSizeY; j++) visited[i, j] = false;
        }
        visited[start.gridX, start.gridY] = true;

        Node[,] prev = new Node[gridComponent.gridSizeX, gridComponent.gridSizeY];

        while (q.Count > 0)
        {
            Node curr = q.Dequeue();
            List<Node> neighbours = gridComponent.GetNeighbours(curr);
            //Debug.Log(q.Count);
            foreach (Node n in neighbours)
            {
                if (!visited[n.gridX, n.gridY])
                {
                    q.Enqueue(n);
                    visited[n.gridX, n.gridY] = true;
                    prev[n.gridX, n.gridY] = curr;
                }
            }
        }

        return prev;
    }

}
