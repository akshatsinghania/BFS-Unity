using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public Node[,] grid;

    float nodeDiameter;
    public int gridSizeX, gridSizeY;

    public Transform player;
    public Transform target;

    public PathFinding pathFinding;
    public List<Node> path;
    

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
        
    }

    
    public void Update()
    {
        path = pathFinding.FindPath(player.position, target.position);
        

    }   
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - (Vector3.right * gridWorldSize.x / 2) - (Vector3.forward * gridWorldSize.y / 2);

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + (Vector3.right * (x * nodeDiameter + nodeRadius)) + (Vector3.forward * (y * nodeDiameter + nodeRadius));
                //Debug.Log(worldPoint);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);

            }
        }
    }
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
       

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        int r = node.gridX;
        int c = node.gridY;
        int[] dr = {-1,1,0,0,-1,1,-1,1};
        int[] dc = {0,0,1,-1,-1,1,1,-1};
        for(int i = 0; i < dr.Length; i++)
        {
            int rr = r + dr[i];
            int cc = c + dc[i];

            if (rr < 0 || cc < 0) continue;
            if (rr >= gridSizeX || cc >= gridSizeY) continue;
            //Debug.Log(new Vector2(rr, cc));
            if (!grid[rr, cc].walkable) continue;
           
            neighbours.Add(grid[rr, cc]);
        }
        return neighbours;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null)
        {
            Node playerNode = NodeFromWorldPoint(player.position);
            Node targetNode = NodeFromWorldPoint(target.position);
            foreach (Node node in grid)
            {
                Gizmos.color = (node.walkable) ? Color.white : Color.red;
                if (path != null)
                {
                    if (path.Contains(node))
                    {
                        Gizmos.color = Color.black;
                    }
                }
                if (node == playerNode)
                {
                    Gizmos.color = Color.cyan;
                }
                if (node == targetNode)
                {
                    Gizmos.color = Color.yellow;
                }
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
}
