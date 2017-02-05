using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathData
{
    private static Dictionary<GridPos, Node> nodeData;

    /// <summary>
    /// store each tile as a node
    /// </summary>
    private static void SpawnNode()
    {
        nodeData = new Dictionary<GridPos, Node>();
        foreach (TileData td in LevelGenerator.Instance.tiles.Values)
        {
            if(!nodeData.ContainsKey(td.gridPosition))
                nodeData.Add(td.gridPosition, new Node(td));
        }
    }

    public static void CalcPath(GridPos spawnPoint)
    {
        if (nodeData == null)
            SpawnNode();

        HashSet<Node> nodeIsOpen = new HashSet<Node>();
        HashSet<Node> nodeIsClosed = new HashSet<Node>();
        Node currentNode = nodeData[spawnPoint];
        nodeIsOpen.Add(currentNode);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                GridPos tmp = new GridPos(currentNode.gridPos.X - x, currentNode.gridPos.Y - y);
                if (tmp != currentNode.gridPos)
                {
                    if(nodeData.ContainsKey(tmp) && !LevelGenerator.Instance.tiles[tmp].IsTowerPlaced)
                    {
                        Node possibleNode = nodeData[tmp];
                        if (!nodeIsOpen.Contains(possibleNode))
                            nodeIsOpen.Add(possibleNode);

                        possibleNode.SetParent(currentNode);
                        Debug.Log(possibleNode.parent.gridPos.X);
                        Debug.Log(possibleNode.parent.gridPos.Y);
                    }
                }
            }
        }
        nodeIsOpen.Remove(currentNode);
        nodeIsClosed.Add(currentNode);

        if (nodeIsClosed.Contains(currentNode))
            Debug.Log("current node is closed");
    }
}
