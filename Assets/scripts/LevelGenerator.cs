using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : Singleton<LevelGenerator>
{
    [SerializeField]
    private List<GameObject> tilePrefabs;

    [SerializeField]
    private CameraInput cameraInput;

    [SerializeField]
    private GameObject startPos;

    [SerializeField]
    private GameObject endPos;

    [SerializeField]
    private Vector2 spawnPoint;

    [SerializeField]
    private Vector2 destroyPoint;

    [SerializeField]
    private GameObject mapTiles;

    private GridPos spawnPos;
    private GridPos destinationPos;
    public Dictionary<GridPos, TileData> tiles { get; set; }
    private float tileSizeX;
    private float tileSizeY;

    private void Awake()
    {
        //get tile width and height
        this.tileSizeX = this.tilePrefabs[0].GetComponent<SpriteRenderer>().bounds.size.x;
        this.tileSizeY = this.tilePrefabs[0].GetComponent<SpriteRenderer>().bounds.size.y;

        //initialize dictionary 
        this.tiles = new Dictionary<GridPos, TileData>();

        //set spawn and end point of creeps
        this.spawnPos = new GridPos((int)this.spawnPoint.x, (int)this.spawnPoint.y);
        this.destinationPos = new GridPos((int)this.destroyPoint.x, (int)this.destroyPoint.y);
    }

    private void Start ()
    {
        SpawnLevel();
        //PathData.CalcPath(startPoint);
	}

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
            PathData.CalcPath(this.spawnPos, this.destinationPos);
    }
    /// <summary>
    /// generates the map
    /// </summary>
    private void SpawnLevel()
    {
        //store map data 
        string[] mapResource = ReadMapFile();

        //get number of characters in each row
        int mapX = mapResource[0].ToCharArray().Length;

        //get number of rows
        int mapY = mapResource.Length;

        Vector3 maxTile = Vector3.zero;

        //get camera top left corner in world space
        Vector3 topLeftWorld = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.main.nearClipPlane));

        //iterate through each row and column of the map to spawn tiles
        for (int y = 0; y < mapY; y++)
        {
            char[] tileTypes = mapResource[y].ToCharArray();
            for (int x = 0; x < mapX; x++)
            {
                SpawnTile(tileTypes[x].ToString(), x, y, topLeftWorld);
            }
        }

        //get the last tile in n-1 row and n-1 column
        maxTile = tiles[new GridPos(mapX - 1, mapY - 1)].transform.position;

        //restrict camera to bounds of the map
        this.cameraInput.RestrictCamera(new Vector3(this.tileSizeX + maxTile.x, maxTile.y - this.tileSizeY));

        SpawnPoints();
    }

    /// <summary>
    /// spawns each tile with a new grid position
    /// </summary>
    /// <param name="tileType"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="topLeftWorld"></param>
    private void SpawnTile(string tileType, int x, int y, Vector3 topLeftWorld)
    {
        //store tile type as an int
        int tileIndex = int.Parse(tileType);

        //create the tile
        TileData tmpTile = Instantiate(tilePrefabs[tileIndex]).GetComponent<TileData>();

        //set the grid position and world position of tile
        tmpTile.SetTile(new GridPos(x, y), new Vector3(topLeftWorld.x + (this.tileSizeX * x), topLeftWorld.y - (this.tileSizeY * y), 0), this.mapTiles);
    }

    /// <summary>
    /// loads the map text file
    /// </summary>
    /// <returns> each row in the map as a string</returns>
    private string[] ReadMapFile()
    {
        //loads the map text file as a TextAsset
        TextAsset getData = Resources.Load("map") as TextAsset;

        //replaces ',' with empty string
        string finalData = getData.text.Replace(Environment.NewLine, string.Empty);

        //return each row in the map as a string
        return finalData.Split(',');
    }

    /// <summary>
    /// spawns start and end point sprites on the centre of tile
    /// </summary>
    private void SpawnPoints()
    {
        //create the starting point of creeps
        Instantiate(this.startPos, this.tiles[this.spawnPos].transform.GetComponent<TileData>().centreOfTile, Quaternion.identity);

        //create the end point of creeps
        Instantiate(this.endPos, this.tiles[this.destinationPos].transform.GetComponent<TileData>().centreOfTile, Quaternion.identity);
    }
}
