using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileData : MonoBehaviour
{
    //position of tile in the grid
    public GridPos gridPosition { get; set; }

    //centre of tile in world space
    public Vector2 centreOfTile
    {
        get { return new Vector2(this.transform.position.x + (this.transform.GetComponent<SpriteRenderer>().bounds.size.x / 2), this.transform.position.y - this.transform.GetComponent<SpriteRenderer>().bounds.size.y / 2); }
    }
    public SpriteRenderer spriteRenderer { get; set; }

    private GameObject towerRef;
    private bool isTowerPlaced;
    public bool IsTowerPlaced
    {
        get
        {
            return isTowerPlaced;
        }
        set
        {
            isTowerPlaced = value;
        }
    }

    private bool specialCase;
    public bool SpecialCase
    {
        get
        {
            return specialCase;
        }

        set
        {
            specialCase = value;
        }
    }

    private List<Color32> colorList;

    private void Awake()
    {
        //initialize the spriteRenderer component
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    private void Start ()
    {
        //initialize and set the colours in the list
        this.colorList = new List<Color32>();
        this.colorList.Add(new Color32(255, 0, 0, 255));
        this.colorList.Add(new Color32(0, 0, 255, 255));

        this.spriteRenderer.sortingOrder = -1;

    }

	private void Update ()
    {

    }

    /// <summary>
    /// sets the tile position in grid space and world space
    /// </summary>
    /// <param name="gridPos"></param>
    /// <param name="worldPos"></param>
    /// <param name="mapTiles"></param>
    public void SetTile(GridPos gridPos, Vector3 worldPos, GameObject mapTiles)
    {
        this.gridPosition = gridPos;
        this.transform.position = worldPos;
        this.transform.SetParent(mapTiles.transform);
        this.isTowerPlaced = false;

        if(!LevelGenerator.Instance.tiles.ContainsKey(gridPos))
        {
            //add tile to dictionary with grid info
            LevelGenerator.Instance.tiles.Add(gridPos, this);
        }

        /****************** Debug *******************/
        //try
        //{
        //    LevelGenerator.Instance.tiles.Add(gridPos, this);
        //}
        //catch(Exception e)
        //{
        //    throw e;
        //}
    }

    /// <summary>
    /// checks if the cursor is on the current tile
    /// changes colour of tile by checking if tower is place or not
    /// spawns the tower on left mouse button
    /// </summary>
    private void OnMouseOver()
    {
        if(!EventSystem.current.IsPointerOverGameObject() && GameHandler.Instance.selectedBtn != null)
        {
            if (!this.isTowerPlaced)
                this.spriteRenderer.color = this.colorList[1];
            if (this.isTowerPlaced || this.specialCase)
                this.spriteRenderer.color = this.colorList[0];
            else if (Input.GetMouseButtonDown(0))
                SpawnTower();
        }
    }

    /// <summary>
    /// reset tile colour to its original when cursor leaves tile
    /// </summary>
    private void OnMouseExit()
    {
        this.spriteRenderer.color = Color.white;
        return;
    }

    /// <summary>
    /// creates the tower and takes gold from player
    /// </summary>
    private void SpawnTower()
    {
        this.towerRef = Instantiate(GameHandler.Instance.selectedBtn.TowerPrefab, this.transform.position, Quaternion.identity);
        this.towerRef.GetComponent<SpriteRenderer>().sortingOrder = this.gridPosition.Y;
        this.towerRef.transform.SetParent(this.transform);
        this.isTowerPlaced = true;
        this.spriteRenderer.color = Color.white;
        GameHandler.Instance.ResetTower();
        return;
    }
}
