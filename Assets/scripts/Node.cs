using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public GridPos gridPos { get; set; }
    public TileData tileData { get; set; }
    public Node parent { get; set; }

    /// <summary>
    /// stores each tile with its grid position
    /// </summary>
    /// <param name="tileData"></param>
    public Node(TileData tileData)
    {
        this.tileData = tileData;
        this.gridPos = tileData.gridPosition;
    }

    public void SetParent(Node parent)
    {
        this.parent = parent;
    }
}
