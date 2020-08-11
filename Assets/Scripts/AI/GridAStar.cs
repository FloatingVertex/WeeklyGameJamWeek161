using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAStar : MonoBehaviour
{
    public static GridAStar singleton;

    public float aiRadius = 1f;
    public float gridEdge = 0.5f;
    public int gridXSideCount = 100;
    public int gridYSideCount = 100;
    public LayerMask raycastingMask;

    private AStarNode[,] data;

    private void Awake()
    {
        singleton = this;
        data = new AStarNode[gridXSideCount, gridYSideCount];
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        if (data != null)
        {
            for (int x = 0; x < gridXSideCount; x++)
            {
                for (int y = 0; y < gridXSideCount; y++)
                {
                    if (data[x, y] != null)
                    {
                        foreach (var link in data[x, y].links)
                        {
                            if (link.x >= data[x, y].x && link.y >= data[x, y].y)
                            {
                                Gizmos.DrawLine(NodeToPosition(data[x, y]), NodeToPosition(link));
                            }
                        }
                    }
                }
            }
        }
    }

    public Vector2[] CalculatePath(Vector2 start, Vector2 end)
    {
        var startNode = PositionToNode(start);
        var endNode = PositionToNode(end);
        if (startNode == null || endNode == null)
        {
            return null;
        }
        return CalculatePath(startNode, endNode);
    }

    private static float GetDistance(AStarNode n1, AStarNode n2)
    {
        return new Vector2(n1.x - n2.x, n1.y - n2.y).magnitude;
    }

    private Vector2[] CalculatePath(AStarNode startNode, AStarNode endNode)
    {

        var openList = new PolyNav.Heap<AStarNode>(1000);
        var closedList = new HashSet<AStarNode>();
        var success = false;

        openList.Add(startNode);

        while (openList.Count > 0)
        {

            var currentNode = openList.RemoveFirst();
            if (currentNode == endNode)
            {
                success = true;
                break;
            }

            closedList.Add(currentNode);

            var links = currentNode.links;
            for (var i = 0; i < links.Count; i++)
            {
                var neighbour = links[i];

                if (closedList.Contains(neighbour))
                {
                    continue;
                }

                var costToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (costToNeighbour < neighbour.gCost || !openList.Contains(neighbour))
                {
                    neighbour.gCost = costToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, endNode);
                    neighbour.parent = currentNode;

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                        openList.UpdateItem(neighbour);
                    }
                }
            }
        }

        if (success)
        { //Retrace Path if one exists
            var path = new List<Vector2>();
            var currentNode = endNode;
            while (currentNode != startNode)
            {
                path.Add(NodeToPosition(currentNode));
                currentNode = currentNode.parent;
            }
            path.Add(NodeToPosition(startNode));
            path.Reverse();
            return path.ToArray();
        }

        return null;
    }

    public void ClearData()
    {
        data = new AStarNode[gridXSideCount, gridYSideCount];
    }

    private Vector2 NodeToPosition(AStarNode node)
    {
        return new Vector2(node.x * gridEdge, node.y * gridEdge);
    }

    private Vector2 IndexToPosition(int x, int y)
    {
        return new Vector2(x * gridEdge, y * gridEdge);
    }

    private AStarNode PositionToNode(Vector2 position)
    {
        return data[Mathf.RoundToInt(position.x / gridEdge), Mathf.RoundToInt(position.y / gridEdge)];
    }

    private (int,int) PositionToIndex(Vector2 position)
    {
        return (Mathf.RoundToInt(position.x / gridEdge), Mathf.RoundToInt(position.y / gridEdge));
    }

    public void RegenerateSection(Vector2 minPoint, Vector2 maxPoint)
    {
        (int minX, int minY) = PositionToIndex(minPoint);
        (int maxX, int maxY) = PositionToIndex(maxPoint);
        RegenerateSection(minX, minY, maxX - minX, maxY - minY);
    }

    public void RegenerateSection(int startX = 0, int startY = 0, int xCount = int.MaxValue, int yCount = int.MaxValue)
    {
        if(data == null)
        {
            data = new AStarNode[gridXSideCount, gridYSideCount];
        }
        var endX = startX + xCount;
        var endY = startY + yCount;
        if (endX > gridXSideCount)
        {
            endX = gridXSideCount;
        }
        if (endY > gridYSideCount)
        {
            endY = gridYSideCount;
        }
        if (startX < 0) startX = 0;
        if (startY < 0) startY = 0;
        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                data[x, y]?.ClearConnection();
                if (!Physics2D.OverlapCircle(IndexToPosition(x, y), aiRadius, raycastingMask))
                {
                    data[x, y] = new AStarNode(x, y, new List<AStarNode>());
                }
            }
        }
        // recalculate connections
        startX--;
        startY--;
        if (startX < 0) startX = 0;
        if (startY < 0) startY = 0;
        if (endX >= gridXSideCount) endX--;
        if (endY >= gridYSideCount) endY--;
        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                if (data[x, y] != null)
                {
                    if (data[x + 1, y] != null)
                    {
                        AttemptConnection(data[x, y], data[x + 1, y]);
                    }
                    if (data[x, y + 1] != null)
                    {
                        AttemptConnection(data[x, y], data[x, y + 1]);
                    }
                    if (data[x + 1, y + 1] != null)
                    {
                        AttemptConnection(data[x, y], data[x + 1, y + 1]);
                    }
                    if( x > 0 && data[x - 1, y + 1] != null)
                    {
                        AttemptConnection(data[x, y], data[x - 1, y + 1]);
                    }
                }
            }
        }
    }

    private bool AttemptConnection(AStarNode n1, AStarNode n2)
    {
        var direction = IndexToPosition(n2.x, n2.y) - IndexToPosition(n1.x, n1.y);
        if (!Physics2D.CircleCast(IndexToPosition(n1.x, n1.y), aiRadius, direction, direction.magnitude, raycastingMask))
        {
            AStarNode.AddLink(n1, n2);
            return true;
        }
        return false;
    }
}

public class AStarNode : PolyNav.IHeapItem<AStarNode>
{
    public int x;
    public int y;
    public List<AStarNode> links;
    public float gCost;
    public float hCost;
    public AStarNode parent;
    public float fCost
    {
        get { return gCost + hCost; }
    }
    int PolyNav.IHeapItem<AStarNode>.heapIndex { get; set; }

    public AStarNode(int x, int y, List<AStarNode> links)
    {
        this.x = x;
        this.y = y;
        this.links = links;
        gCost = 0;
        hCost = 0;
        parent = null;
    }


    public void ClearConnection()
    {
        foreach(var link in links)
        {
            link.links.Remove(this);
        }
        links = new List<AStarNode>();
    }

    public static void AddLink(AStarNode n1, AStarNode n2)
    {
        n1.links.Add(n2);
        n2.links.Add(n1);
    }

    public static void RemoveLink(AStarNode n1, AStarNode n2)
    {
        n1.links.Remove(n2);
        n2.links.Remove(n1);
    }

    public int CompareTo(AStarNode other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }
        return -compare;
    }
}