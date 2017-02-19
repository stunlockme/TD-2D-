using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node
{
    public GridPos gridPos { get; set; }
    public TileData tileData { get; set; }
    public Node parent { get; set; }

    public int costToMove { get; private set; }
    public int estimatedCost { get; private set; }
    public int finalCost { get; private set; }
    public Vector2 worldPos { get; set; }

    private int toConvert;

    /// <summary>
    /// stores each tile with its grid position
    /// </summary>
    /// <param name="tileData"></param>
    public Node(TileData tileData)
    {
        this.toConvert = 10;
        this.tileData = tileData;
        this.gridPos = tileData.gridPosition;
        this.worldPos = tileData.centreOfTile;
    }

    /// <summary>
    /// sets the node parent
    /// </summary>
    /// <param name="parent"></param>
    public void SetParent(Node parent)
    {
        this.parent = parent;
    }

    public void SetCost(int costToMove, Node destination)
    {
        this.costToMove = this.parent.costToMove + costToMove;
        this.estimatedCost = ((Math.Abs(gridPos.X - destination.gridPos.X)) + Math.Abs((gridPos.Y - destination.gridPos.Y))) * this.toConvert;
        this.finalCost = this.costToMove + this.estimatedCost;
    }
}
