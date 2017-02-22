using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    /// <summary>
    /// calculates the shortest path to destination
    /// </summary>
    /// <param name="spawnPos"></param>
    public static Stack<Node> CalcPath(GridPos spawnPos, GridPos destinationPos)
    {
        //null check the node dictionary
        if (nodeData == null)
            SpawnNode();

        //initialize open, closed container sets
        HashSet<Node> nodeIsOpen = new HashSet<Node>();
        HashSet<Node> nodeIsClosed = new HashSet<Node>();

        Stack<Node> shortestPath = new Stack<Node>();

        //set the start of search to the creep spawn position
        Node currentNode = nodeData[spawnPos];

        //add node to the open set
        nodeIsOpen.Add(currentNode);

        //loop until open set contains a node
        while(nodeIsOpen.Count > 0)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    //get grid position of surrounding nodes
                    GridPos tmp = new GridPos(currentNode.gridPos.X - x, currentNode.gridPos.Y - y);

                    //ignore the current node from the search
                    if (tmp != currentNode.gridPos)
                    {
                        //check if the grid position is valid, check if tower is placed
                        if (nodeData.ContainsKey(tmp) && !LevelGenerator.Instance.tiles[tmp].IsTowerPlaced)
                        {
                            int costToMove = 0;
                            if (Math.Abs(x - y) == 1)
                                costToMove = 10;        //up, down, forward, back
                            else
                            {
                                //if (LevelGenerator.Instance.tilesWithTower.ContainsKey(tmp))
                                //    continue;
                                if (!IgnoreDiagonalPath(currentNode, nodeData[tmp]))
                                    continue;
                                costToMove = 14;        //diagonal
                            }

                            //get the possible node data from the dictionary
                            Node possibleNode = nodeData[tmp];

                            //check if the open set contains the possible node
                            if (nodeIsOpen.Contains(possibleNode))
                            {
                                if (currentNode.costToMove + costToMove < possibleNode.costToMove)
                                {
                                    possibleNode.SetParent(currentNode);
                                    possibleNode.SetCost(costToMove, nodeData[destinationPos]);
                                }
                            }
                            //check if the closed set contains the possible node
                            else if (!nodeIsClosed.Contains(possibleNode))
                            {
                                nodeIsOpen.Add(possibleNode);
                                possibleNode.SetParent(currentNode);
                                possibleNode.SetCost(costToMove, nodeData[destinationPos]);
                            }
                        }
                    }
                }
            }

            //remove the current node from the open set and add it to the closed set after the search is complete
            nodeIsOpen.Remove(currentNode);
            nodeIsClosed.Add(currentNode);

            //set the current node to be the node that contains least final cost
            if (nodeIsOpen.Count > 0)
                currentNode = nodeIsOpen.OrderBy(node => node.finalCost).First();

            if (currentNode == nodeData[destinationPos])
            {
                while (currentNode.gridPos != spawnPos)
                {
                    shortestPath.Push(currentNode);
                    currentNode = currentNode.parent;
                    //Debug.Log(shortestPath.Peek().gridPos.X + " " + shortestPath.Peek().gridPos.Y);
                }
                break;
            }
        }
        return shortestPath;
    }

    private static bool IgnoreDiagonalPath(Node currentNode, Node neighbour)
    {
        GridPos direction = neighbour.gridPos - currentNode.gridPos;
        GridPos a = new GridPos(currentNode.gridPos.X + direction.X, currentNode.gridPos.Y + direction.Y);
        GridPos b = new GridPos(currentNode.gridPos.X, currentNode.gridPos.Y + direction.Y);

        if (nodeData.ContainsKey(a) && !LevelGenerator.Instance.tiles[a].IsTowerPlaced)
            return false;
        if (nodeData.ContainsKey(b) && !LevelGenerator.Instance.tiles[b].IsTowerPlaced)
            return false;
        return true;
    }
}
