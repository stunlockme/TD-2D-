﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileData : MonoBehaviour
{
    private GameObject towerRef;
    private bool isTowerPlaced;
    private TowerRange towerRange;
    private List<Color32> colorList;

    //position of tile in the grid
    public GridPos gridPosition { get; private set; }

    //centre of tile in world space
    public Vector2 centreOfTile
    {
        get { return new Vector2(this.transform.position.x + (this.transform.GetComponent<SpriteRenderer>().bounds.size.x / 2), this.transform.position.y - this.transform.GetComponent<SpriteRenderer>().bounds.size.y / 2); }
    }

    public Vector2 rightCentreOfTile
    {
        get { return new Vector2(this.transform.position.x + (this.transform.GetComponent<SpriteRenderer>().bounds.size.x ), this.transform.position.y - this.transform.GetComponent<SpriteRenderer>().bounds.size.y / 2); }
    }

    public Vector2 btmCentreOfTile
    {
        get { return new Vector2(this.transform.position.x + (this.transform.GetComponent<SpriteRenderer>().bounds.size.x / 2), this.transform.position.y - this.transform.GetComponent<SpriteRenderer>().bounds.size.y); }
    }
    public SpriteRenderer spriteRenderer { get; set; }

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

    private bool unitOnTile;
    public bool UnitOnTile
    {
        get
        {
            return unitOnTile;
        }
        set
        {
            unitOnTile = value;
        }
    }

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
        this.colorList.Add(new Color32(138, 43, 226, 255));
        this.colorList.Add(new Color32(0, 255, 0, 255));

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
    public void SetTile(GridPos gridPos, Vector3 worldPos, GameObject mapTiles, string mapAge)
    {
        this.gridPosition = gridPos;
        this.transform.position = worldPos;
        this.transform.SetParent(mapTiles.transform);
        this.isTowerPlaced = false;
        this.unitOnTile = false;

        //if(mapAge == "stoneAgeMap")
        //{
            if (!LevelGenerator.Instance.tiles.ContainsKey(gridPos))
            {
                //add tile to dictionary with grid info
                LevelGenerator.Instance.tiles.Add(gridPos, this);
            }
        //}
        //if(mapAge == "stoneAgeMapLarge")
        //{
        //    Debug.Log("large map");
        //    if (!LevelGenerator.Instance.newAgeTiles.ContainsKey(gridPos))
        //    {
        //        //add tile to dictionary with grid info
        //        LevelGenerator.Instance.newAgeTiles.Add(gridPos, this);
        //        //Debug.Log("adding to newAgeTiles");
        //        //Debug.Log(gridPos.X + " " + gridPos.Y);
        //    }
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
            {
                if (this.specialCase)
                    this.spriteRenderer.color = this.colorList[2];
                else
                    this.spriteRenderer.color = this.colorList[0];
            }
            else if (Input.GetMouseButtonDown(0))
                SpawnTower(LevelGenerator.Instance.MapX, LevelGenerator.Instance.MapY);
        }
        else if(!EventSystem.current.IsPointerOverGameObject() && GameHandler.Instance.selectedBtn == null && Input.GetMouseButtonDown(0))
        {
            if(this.towerRange != null)
                GameHandler.Instance.ChooseTowerRange(this.towerRange);
            else
                GameHandler.Instance.RemoveTowerRange();
        }
        if(this.towerRange != null)
            this.towerRange.DisableSellBtn();
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
    private void SpawnTower(int mapX, int mapY)
    {
        this.towerRef = Instantiate(GameHandler.Instance.selectedBtn.TowerPrefab, this.transform.position, Quaternion.identity);
        //Debug.Log(this.gridPosition.X + " " + this.gridPosition.Y);
        this.towerRef.GetComponent<SpriteRenderer>().sortingOrder = this.gridPosition.Y;
        this.towerRef.transform.SetParent(this.transform);
        this.towerRange = this.towerRef.transform.GetChild(0).GetComponent<TowerRange>();
        this.isTowerPlaced = true;
        this.spriteRenderer.color = Color.white;
        LockDiagonalTiles();
        CheckIfCreepHasPathAvailable(mapX, mapY);
        GameHandler.Instance.ResetTower();  
        return;
    }

    /// <summary>
    /// sets diagonal tiles not to build towers
    /// using these rules to not block creep path to destination
    /// </summary>
    private void LockDiagonalTiles()
    {
        LevelGenerator.Instance.tiles[new GridPos(this.gridPosition.X - 1, this.gridPosition.Y - 1)].specialCase = true;
        LevelGenerator.Instance.tiles[new GridPos(this.gridPosition.X + 1, this.gridPosition.Y - 1)].specialCase = true;
        LevelGenerator.Instance.tiles[new GridPos(this.gridPosition.X - 1, this.gridPosition.Y + 1)].specialCase = true;
        LevelGenerator.Instance.tiles[new GridPos(this.gridPosition.X + 1, this.gridPosition.Y + 1)].specialCase = true;
        //GridPos tmp = new GridPos(this.gridPosition.X - 1, this.gridPosition.Y - 1);
        //Debug.Log(tmp.X + " " + tmp.Y);
        //tmp = new GridPos(this.gridPosition.X + 1, this.gridPosition.Y - 1);
        //Debug.Log(tmp.X + " " + tmp.Y);
        //tmp = new GridPos(this.gridPosition.X - 1, this.gridPosition.Y + 1);
        //Debug.Log(tmp.X + " " + tmp.Y);
        //tmp = new GridPos(this.gridPosition.X + 1, this.gridPosition.Y + 1);
        //Debug.Log(tmp.X + " " + tmp.Y);
        return;
    }

    /// <summary>
    /// set tiles to build towers after selling the tower
    /// </summary>
    public void UnLockDiagonalTiles()
    {
        LevelGenerator.Instance.tiles[new GridPos(this.gridPosition.X, this.gridPosition.Y)].IsTowerPlaced = false;
        LevelGenerator.Instance.tiles[new GridPos(this.gridPosition.X - 1, this.gridPosition.Y - 1)].specialCase = false;
        LevelGenerator.Instance.tiles[new GridPos(this.gridPosition.X + 1, this.gridPosition.Y - 1)].specialCase = false;
        LevelGenerator.Instance.tiles[new GridPos(this.gridPosition.X - 1, this.gridPosition.Y + 1)].specialCase = false;
        LevelGenerator.Instance.tiles[new GridPos(this.gridPosition.X + 1, this.gridPosition.Y + 1)].specialCase = false;
        GridPos gridPos = new GridPos(this.gridPosition.X - 1, this.gridPosition.Y + 1);
        if(gridPos.X == LevelGenerator.Instance.SpawnPos.X + 1 && gridPos.Y == LevelGenerator.Instance.SpawnPos.Y)
            LevelGenerator.Instance.tiles[gridPos].specialCase = true;
        return;
    }

    /// <summary>
    /// keeps a tile open for creeps to move to destination
    /// </summary>
    /// <param name="mapX"></param>
    /// <param name="mapY"></param>
    private void CheckIfCreepHasPathAvailable(int mapX, int mapY)
    {

        int countX = 0;
        for (int x = 0; x < mapX; x++)
        {
            int y = this.gridPosition.Y;
            GridPos gridPos = new GridPos(x, y);
            if (LevelGenerator.Instance.tiles[gridPos].IsTowerPlaced || LevelGenerator.Instance.tiles[gridPos].SpecialCase)
            {
                countX += 1;
            }
        }
        if (countX > mapX - 2)
        {
            for (int x = 0; x < mapX; x++)
            {
                int y = this.gridPosition.Y;
                GridPos gridPos = new GridPos(x, y);
                if (!LevelGenerator.Instance.tiles[gridPos].IsTowerPlaced && !LevelGenerator.Instance.tiles[gridPos].SpecialCase)
                {
                    LevelGenerator.Instance.tiles[gridPos].SpecialCase = true;
                    Debug.Log("pathCheck" + gridPos.X + " " + gridPos.Y);
                }
            }
        }

        int countY = 0;
        int towerYCount = 0;
        for (int y = 0; y < mapY; y++)
        {
            int x = this.gridPosition.X;
            Debug.Log("gridX : " + x);
            GridPos gridPos = new GridPos(x, y);
            if (LevelGenerator.Instance.tiles[gridPos].IsTowerPlaced)
            {
                towerYCount += 1;
            }
            if (LevelGenerator.Instance.tiles[gridPos].IsTowerPlaced || LevelGenerator.Instance.tiles[gridPos].SpecialCase)
            {
                Debug.Log("the gridPos is : " + gridPos.X +" "+ gridPos.Y);
                countY += 1;
                Debug.Log("adding 1");
            }
        }

        if(GameHandler.Instance.WaveCount <= 1)
        {
            if (towerYCount >= 6)
            {
                for (int y = 0; y < mapY; y++)
                {
                    int x = this.gridPosition.X;
                    GridPos gridPos = new GridPos(x, y);
                    if (!LevelGenerator.Instance.tiles[gridPos].IsTowerPlaced && !LevelGenerator.Instance.tiles[gridPos].SpecialCase)
                    {
                        LevelGenerator.Instance.tiles[gridPos].SpecialCase = true;
                        Debug.Log("pathCheck" + gridPos.X + " " + gridPos.Y);
                    }
                }
            }
        }
        Debug.Log("count Y : " + countY);
        Debug.Log("mapY : " + mapY);
        if (countY > mapY - 1)
        {
            for (int y = 0; y < mapY; y++)
            {
                int x = this.gridPosition.X;
                GridPos gridPos = new GridPos(x, y);
                if (!LevelGenerator.Instance.tiles[gridPos].IsTowerPlaced && !LevelGenerator.Instance.tiles[gridPos].SpecialCase)
                {
                    LevelGenerator.Instance.tiles[gridPos].SpecialCase = true;
                    Debug.Log("pathCheck" + gridPos.X + " " + gridPos.Y);
                }
            }
        }
        return;
    }
}
