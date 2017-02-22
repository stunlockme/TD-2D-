using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : Singleton<LevelGenerator>
{
    [SerializeField]
    private List<GameObject> tileR0;

    [SerializeField]
    private List<GameObject> tileR1;

    [SerializeField]
    private List<GameObject> tileR2;

    [SerializeField]
    private List<GameObject> tileR3;

    [SerializeField]
    private List<GameObject> tileR4;

    [SerializeField]
    private List<GameObject> tileR5;

    [SerializeField]
    private List<GameObject> tileR6;

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

    [SerializeField]
    private List<string> mapList;

    private GridPos spawnPos;
    private GridPos destinationPos;
    public Dictionary<GridPos, TileData> tiles { get; set; }
    //public Dictionary<GridPos, TileData> tilesWithTower { get; set; }
    private float tileSizeX;
    private float tileSizeY;
    private TileData tmpTile;
    public CreepGate creepGate { get; set; }
    private Stack<Node> wayPoints;
    public Stack<Node> WayPoints
    {
        get
        {
            if(wayPoints == null)
            {
                CreateWayPoints(this.spawnPos, this.destinationPos);
            }
            return new Stack<Node>(new Stack<Node>(wayPoints));
        }
    }

    public GridPos SpawnPos
    {
        get
        {
            return spawnPos;
        }
    }

    public GridPos DestinationPos
    {
        get
        {
            return destinationPos;
        }
    }

    private void Awake()
    {
        //get tile width and height
        this.tileSizeX = this.tileR0[0].GetComponent<SpriteRenderer>().bounds.size.x;
        this.tileSizeY = this.tileR0[0].GetComponent<SpriteRenderer>().bounds.size.y;

        //initialize dictionary 
        this.tiles = new Dictionary<GridPos, TileData>();

        //this.tilesWithTower = new Dictionary<GridPos, TileData>();

        //set spawn and end point of creeps
        this.spawnPos = new GridPos((int)this.spawnPoint.x, (int)this.spawnPoint.y);
        this.destinationPos = new GridPos((int)this.destroyPoint.x, (int)this.destroyPoint.y);
    }

    private void Start ()
    {
        SpawnLevel(this.mapList[0]);
	}

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
            PathData.CalcPath(this.spawnPos, this.destinationPos);
    }
    
    /// <summary>
    /// generates the maps
    /// </summary>
    /// <param name="mapType"></param>
    private void SpawnLevel(string mapType)
    {
        //store map data 
        string[] mapResource = ReadMapFile(mapType);

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
            char[] tmpChar = new char[2];
            tmpChar[0] = tileTypes[mapX - 2];
            tmpChar[1] = tileTypes[mapX - 1];
            string s = new string(tmpChar);

            for (int x = 0; x < mapX; x++)
            {
                if (x < 10)
                    SpawnTile(tileTypes[x].ToString(), x, y, topLeftWorld);
                else
                    SpawnTile(s, x, y, topLeftWorld);
            }
        }

        //get the last tile in n-1 row and n-1 column
        maxTile = tiles[new GridPos(mapX - 1, mapY - 1)].transform.position;

        //restrict camera to bounds of the map
        //this.cameraInput.RestrictCamera(new Vector3(this.tileSizeX + maxTile.x, maxTile.y - this.tileSizeY));

        this.cameraInput.RestrictCamera(new Vector3(maxTile.x, maxTile.y - this.tileSizeY));

        SpawnPoints();

        //disable the last coloumn of tiles
        for (int y = 0; y < 8; y++)
        {
            int x = mapX - 1;
            GridPos lastTile = new GridPos(x, y);

            if(this.tiles.ContainsKey(lastTile))
            {
                this.tiles[lastTile].transform.gameObject.SetActive(false);
                this.tiles.Remove(lastTile);
                //Destroy(this.tiles[lastTile].transform.gameObject);
            }
        }

        //set the map border not able to place towers
        if (mapType == this.mapList[0])
        {
            GridPos gridPos;
            for (int y = 0; y < mapY; y++)
            {
                int x = 0;
                gridPos = new GridPos(x, y);
                this.tiles[gridPos].IsTowerPlaced = true;
            }
            for (int y = 2; y < mapY; y++)
            {
                int x = mapX - 2;
                gridPos = new GridPos(x, y);
                this.tiles[gridPos].IsTowerPlaced = true;
            }
            for (int x = 0; x < mapX - 1; x++)
            {
                int y = 0;
                gridPos = new GridPos(x, y);
                this.tiles[gridPos].IsTowerPlaced = true;
            }
            for (int x = 1; x < mapX - 1; x++)
            {
                int y = mapY - 1;
                gridPos = new GridPos(x, y);
                this.tiles[gridPos].IsTowerPlaced = true;
            }
            gridPos = new GridPos((int)this.spawnPoint.x + 1, (int)this.spawnPoint.y);
            this.tiles[gridPos].SpecialCase = true;
            this.tiles[this.destinationPos].SpecialCase = true;
            this.tiles[this.spawnPos].IsTowerPlaced = false;
            this.tiles[this.spawnPos].SpecialCase = true;
            gridPos = new GridPos((int)this.destroyPoint.x - 1, (int)this.destroyPoint.y);
            this.tiles[gridPos].SpecialCase = true;
        }
        return;
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
        //TileData tmpTile = Instantiate(tilePrefabs[tileIndex]).GetComponent<TileData>();

        //create the tiles
        if (y == 0)
            tmpTile = Instantiate(this.tileR0[tileIndex]).GetComponent<TileData>();
        else if (y == 1)
            tmpTile = Instantiate(this.tileR1[tileIndex]).GetComponent<TileData>();
        else if (y == 2)
            tmpTile = Instantiate(this.tileR2[tileIndex]).GetComponent<TileData>();
        else if (y == 3)
            tmpTile = Instantiate(this.tileR3[tileIndex]).GetComponent<TileData>();
        else if (y == 4)
            tmpTile = Instantiate(this.tileR4[tileIndex]).GetComponent<TileData>();
        else if (y == 5)
            tmpTile = Instantiate(this.tileR5[tileIndex]).GetComponent<TileData>();
        else if (y == 6)
            tmpTile = Instantiate(this.tileR6[tileIndex]).GetComponent<TileData>();

        //set the grid position and world position of tile
        tmpTile.SetTile(new GridPos(x, y), new Vector3(topLeftWorld.x + (this.tileSizeX * x), topLeftWorld.y - (this.tileSizeY * y), 0), this.mapTiles);
        return;
    }

    /// <summary>
    /// loads the map text file
    /// </summary>
    /// <returns> each row in the map as a string</returns>
    private string[] ReadMapFile(string mapType)
    {
        //loads the map text file as a TextAsset
        TextAsset getData = Resources.Load(mapType) as TextAsset;

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
        GameObject tmp = (GameObject)Instantiate(this.startPos, this.tiles[this.spawnPos].transform.GetComponent<TileData>().centreOfTile, Quaternion.identity);
        this.creepGate = tmp.GetComponent<CreepGate>();

        //create the end point of creeps
        Instantiate(this.endPos, this.tiles[this.destinationPos].transform.GetComponent<TileData>().centreOfTile, Quaternion.identity);
        return;
    }

    public Stack<Node> CreateWayPoints(GridPos currentPos, GridPos destinationPos)
    {
        this.wayPoints = PathData.CalcPath(currentPos, destinationPos);
        return this.wayPoints;
    }
}
