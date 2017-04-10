using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    private List<GameObject> tileR7;
    [SerializeField]
    private List<GameObject> tileR8;
    [SerializeField]
    private List<GameObject> tileR9;
    [SerializeField]
    private List<GameObject> tileR10;

    private CameraInput cameraInput;
    private GameObject spawnGate;
    private GameObject spawnGate2;
    private GameObject destinationGate;

    [SerializeField]
    private Vector2 spawnPoint;

    [SerializeField]
    private Vector2 destroyPoint;

    [SerializeField]
    private GameObject mapTiles;

    [SerializeField]
    private List<string> mapList;

    private GridPos spawnPos;
    public GridPos SpawnPos
    {
        get { return spawnPos; }
        //set { spawnPos = value; }
    }
    private GridPos spawnPos2;
    public GridPos SpawnPos2
    {
        get { return spawnPos2; }
        //set { spawnPos = value; }
    }
    private GridPos destinationPos;
    public GridPos DestinationPos
    {
        get { return destinationPos; }
        //set { destinationPos = value; }
    }
    public Dictionary<GridPos, TileData> tiles { get; set; }
    public Dictionary<GridPos, TileData> newAgeTiles { get; set; }
    public List<GridPos> spawnPosList { get; set; }
    private float tileSizeX;
    private float tileSizeY;
    private TileData tmpTile;
    public CreepGate creepGate { get; set; }
    public CreepGate creepGate2 { get; set; }
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

    private char[] charArr = { 'a', 'b', 'c' };

    private int mapX;
    public int MapX
    { get { return mapX; } }
    private int mapY;
    public int MapY
    { get { return mapY; } }

    [SerializeField]
    private GameObject visibilitytwr;

    private GameObject spawnHole;

    private bool loadMedievalMap = false;
    public bool LoadMedievalMap
    {
        get
        {
            return loadMedievalMap;
        }
        set
        {
            loadMedievalMap = value;
        }
    }
    private bool once = false;


    private void Awake()
    {
        this.tileR0 = new List<GameObject>();
        this.tileR1 = new List<GameObject>();
        this.tileR2 = new List<GameObject>();
        this.tileR3 = new List<GameObject>();
        this.tileR4 = new List<GameObject>();
        this.tileR5 = new List<GameObject>();
        this.tileR6 = new List<GameObject>();
        this.tileR7 = new List<GameObject>();
        this.tileR8 = new List<GameObject>();
        this.tileR9 = new List<GameObject>();
        this.tileR10 = new List<GameObject>();
        LoadStoneAgeMap();

        //get tile width and height
        this.tileSizeX = this.tileR0[0].GetComponent<SpriteRenderer>().bounds.size.x;
        this.tileSizeY = this.tileR0[0].GetComponent<SpriteRenderer>().bounds.size.y;

        //initialize dictionary 
        this.tiles = new Dictionary<GridPos, TileData>();
        this.newAgeTiles = new Dictionary<GridPos, TileData>();

        //set spawn and end point of creeps
        this.spawnPos = new GridPos((int)this.spawnPoint.x, (int)this.spawnPoint.y);
        this.destinationPos = new GridPos((int)this.destroyPoint.x, (int)this.destroyPoint.y);
        this.spawnPos2 = new GridPos(9, 10);

        this.spawnPosList = new List<GridPos>();

        this.spawnGate = Resources.Load("gates/spawnGate", typeof(GameObject)) as GameObject;
        this.destinationGate = Resources.Load("gates/destinationGate", typeof(GameObject)) as GameObject;
        this.spawnGate2 = Resources.Load("gates/spawnGate2", typeof(GameObject)) as GameObject;

        this.cameraInput = Camera.main.GetComponent<CameraInput>();
        return;
    }

    private void Start ()
    {
        //Debug.Log("setting nodeData start: " + PathData.NodeData);
        //Debug.Log(PathData.NodeData.ContainsKey(new GridPos(0, 0)));
        SpawnLevel(this.mapList[0]);
    }

    private void Update()
    {
        Debug.Log(this.once);
        if(this.loadMedievalMap)
        {
            if(!once)
            {
                TileData[] tileObjs = this.mapTiles.GetComponentsInChildren<TileData>();
                GameObject[] spawnGateObj = GameObject.FindGameObjectsWithTag("SpawnPos");
                foreach(GameObject go in spawnGateObj)
                {
                    Destroy(go);
                }
                foreach (TileData tile in tileObjs)
                {
                    if (tile.gameObject.activeInHierarchy)
                            Destroy(tile.gameObject);
                    //tile.gameObject.SetActive(!tile.gameObject.activeSelf);
                }
                Debug.Log("printing ");
                //startNewAge = true;
                this.tiles.Clear();
                this.tiles = new Dictionary<GridPos, TileData>();
                this.tileR0.Clear();
                this.tileR0 = new List<GameObject>();
                this.tileR1.Clear();
                this.tileR1 = new List<GameObject>();
                this.tileR2.Clear();
                this.tileR2 = new List<GameObject>();
                this.tileR3.Clear();
                this.tileR3 = new List<GameObject>();
                this.tileR4.Clear();
                this.tileR4 = new List<GameObject>();
                this.tileR5.Clear();
                this.tileR5 = new List<GameObject>();
                this.tileR6.Clear();
                this.tileR6 = new List<GameObject>();
                //LoadStoneAgeMap();
                LoadMedievalAgeMap();
                SpawnLevel(this.mapList[1]);
                this.spawnPosList.Add(this.spawnPos);
                this.spawnPosList.Add(this.spawnPos2);
                this.loadMedievalMap = false;
                this.once = true;
            }
        }
    }
    
    /// <summary>
    /// generates the maps
    /// </summary>
    /// <param name="mapType"></param>
    private void SpawnLevel(string mapType)
    {
        Camera.main.transform.position = new Vector3(0, 0, -10.0f);
        Camera.main.orthographicSize = 8;
        Debug.Log("spawn Level : " + mapType);
        //store map data 
        string[] mapResource = ReadMapFile(mapType);

        //get number of characters in each row
        this.mapX = mapResource[0].ToCharArray().Length;

        //get number of rows
        this.mapY = mapResource.Length;

        Vector3 maxTile = Vector3.zero;

        //get camera top left corner in world space
        Vector3 topLeftWorld = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.main.nearClipPlane));

        //iterate through each row and column of the map to spawn tiles
        for (int y = 0; y < mapY; y++)
        {
            char[] tileTypes = mapResource[y].ToCharArray();
            for (int x = 0; x < mapX; x++)
            {
                SpawnTile(tileTypes[x].ToString(), x, y, topLeftWorld, mapType);
            }
        }

        //if(mapType == "stoneAgeMap")
        //{
            //get the last tile in n-1 row and n-1 column
            maxTile = tiles[new GridPos(mapX - 1, mapY - 1)].transform.position;
        //}
        //if(mapType == "stoneAgeMapLarge")
        //{
        //    //get the last tile in n-1 row and n-1 column
        //    maxTile = newAgeTiles[new GridPos(mapX - 1, mapY - 1)].transform.position;
        //}

        //restrict camera to bounds of the map
        this.cameraInput.RestrictCamera(new Vector3(this.tileSizeX + maxTile.x, maxTile.y - this.tileSizeY));

        SpawnPoints(mapType);

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
            for (int y = 3; y < mapY; y++)
            {
                int x = mapX - 1;
                gridPos = new GridPos(x, y);
                this.tiles[gridPos].IsTowerPlaced = true;
            }
            for (int x = 0; x < mapX; x++)
            {
                int y = 0;
                gridPos = new GridPos(x, y);
                this.tiles[gridPos].IsTowerPlaced = true;
            }
            for (int x = 1; x < mapX; x++)
            {
                int y = 1;
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
        else if(mapType == this.mapList[1])
        {
            GridPos gridPos;
            for(int y = 0; y < mapY; y++)
            {
                int x = 0;
                gridPos = new GridPos(x, y);
                this.tiles[gridPos].IsTowerPlaced = true;
            }
            for(int y = 1; y < mapY; y++)
            {
                int x = 10;
                gridPos = new GridPos(x, y);
                this.tiles[gridPos].IsTowerPlaced = true;
            }
            for(int x = 0; x < mapX; x++)
            {
                int y = 7;
                gridPos = new GridPos(x, y);
                this.tiles[gridPos].IsTowerPlaced = true;
            }
            for (int x = 0; x < mapX; x++)
            {
                int y = 6;
                gridPos = new GridPos(x, y);
                this.tiles[gridPos].IsTowerPlaced = true;
            }
            for(int x = 2; x < mapX - 2; x++)
            {
                int y = 10;
                gridPos = new GridPos(x, y);
                this.tiles[gridPos].IsTowerPlaced = true;
            }
            for(int x = 1; x < mapX; x++)
            {
                int y = 0;
                gridPos = new GridPos(x, y);
                this.tiles[gridPos].IsTowerPlaced = true;
            }
            for(int y = 5; y < mapY - 1; y++)
            {
                int x = 5;
                gridPos = new GridPos(x, y);
                this.tiles[gridPos].IsTowerPlaced = false;
                this.tiles[gridPos].SpecialCase = true;
            }
            gridPos = new GridPos(6, 5);
            this.tiles[gridPos].IsTowerPlaced = true;
            gridPos = new GridPos(8, 4);
            this.tiles[gridPos].IsTowerPlaced = true;
            gridPos = new GridPos(4, 1);
            this.tiles[gridPos].IsTowerPlaced = true;
            gridPos = new GridPos(2, 2);
            this.tiles[gridPos].IsTowerPlaced = true;
            this.tiles[this.spawnPos].SpecialCase = true;
            gridPos = new GridPos(this.spawnPos.X, this.spawnPos.Y - 1);
            this.tiles[gridPos].SpecialCase = true;
            this.tiles[this.SpawnPos2].SpecialCase = true;
            gridPos = new GridPos(this.SpawnPos2.X, this.SpawnPos2.Y - 1);
            this.tiles[gridPos].SpecialCase = true;
            this.tiles[this.destinationPos].IsTowerPlaced = false;
            this.tiles[this.destinationPos].SpecialCase = true;
            gridPos = new GridPos(this.destinationPos.X, this.destinationPos.Y + 1);
            this.tiles[gridPos].SpecialCase = true;
        }

        //Instantiate(visibilitytwr, LevelGenerator.Instance.tiles[new GridPos(5, 3)].transform.position, Quaternion.identity);
        return;
    }

    /// <summary>
    /// spawns each tile with a new grid position
    /// </summary>
    /// <param name="tileType"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="topLeftWorld"></param>
    private void SpawnTile(string tileType, int x, int y, Vector3 topLeftWorld, string mapAge)
    {
        if (tileType == this.charArr[0].ToString())
            tileType = "10";
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
        else if (y == 7)
            tmpTile = Instantiate(this.tileR7[tileIndex]).GetComponent<TileData>();
        else if (y == 8)
            tmpTile = Instantiate(this.tileR8[tileIndex]).GetComponent<TileData>();
        else if (y == 9)
            tmpTile = Instantiate(this.tileR9[tileIndex]).GetComponent<TileData>();
        else if (y == 10)
            tmpTile = Instantiate(this.tileR10[tileIndex]).GetComponent<TileData>();
        //set the grid position and world position of tile
        tmpTile.SetTile(new GridPos(x, y), new Vector3(topLeftWorld.x + (this.tileSizeX * x), topLeftWorld.y - (this.tileSizeY * y), 0), this.mapTiles, mapAge);
        return;
    }

    /// <summary>
    /// loads the map text file
    /// </summary>
    /// <returns> each row in the map as a string</returns>
    private string[] ReadMapFile(string mapType)
    {
        string maps = "maps/";
        //loads the map text file as a TextAsset
        TextAsset getData = Resources.Load(maps + mapType) as TextAsset;

        //replaces ',' with empty string
        string finalData = getData.text.Replace(Environment.NewLine, string.Empty);

        //return each row in the map as a string
        return finalData.Split(',');
    }

    /// <summary>
    /// spawns start and end point sprites on the centre of tile
    /// </summary>
    private void SpawnPoints(string mapAge)
    {
        Debug.Log("spawnPoints " + mapAge);
        //if(mapAge == "stoneAgeMap")
        //{
            //create the starting point of creeps
            GameObject tmp = Instantiate(this.spawnGate, this.tiles[this.spawnPos].transform.GetComponent<TileData>().centreOfTile, Quaternion.identity);
            this.creepGate = tmp.GetComponent<CreepGate>();

            //create the end point of creeps
            Instantiate(this.destinationGate, this.tiles[this.destinationPos].transform.GetComponent<TileData>().centreOfTile, Quaternion.identity);
        //}
        if(mapAge == this.mapList[1])
        {
            GameObject sGate = Instantiate(this.spawnGate2, this.tiles[this.spawnPos2].transform.GetComponent<TileData>().centreOfTile, Quaternion.identity);
            this.creepGate2 = sGate.GetComponent<CreepGate>();
        }
        return;
        //if(mapAge == "stoneAgeMapLarge")
        //{
        //    Debug.Log(this.spawnPos.X + " " + this.spawnPos.Y);
        //    //create the starting point of creeps
        //    GameObject tmp = Instantiate(this.spawnGate, this.newAgeTiles[this.spawnPos].transform.GetComponent<TileData>().centreOfTile, Quaternion.identity);
        //    this.creepGate = tmp.GetComponent<CreepGate>();

        //    //create the end point of creeps
        //    //Instantiate(this.destinationGate, this.newAgeTiles[this.destinationPos].transform.GetComponent<TileData>().centreOfTile, Quaternion.identity);
        //    return;
        //}
    }

    public Stack<Node> CreateWayPoints(GridPos currentPos, GridPos destinationPos)
    {
        this.wayPoints = PathData.CalcPath(currentPos, destinationPos);
        return this.wayPoints;
    }

    public void SpawnHoleObject()
    {
        this.spawnGate.SetActive(!this.spawnGate.activeSelf);
    }

    private void LoadTiles(List<GameObject> mapRow, string prefabName)
    {
        if(mapRow != null)
        {
            GameObject tilePrefab = Resources.Load(prefabName, typeof(GameObject)) as GameObject;
            mapRow.Add(tilePrefab);
            return;
        }
    }

    /// <summary>
    /// loads each row of map1
    /// </summary>
    private void LoadStoneAgeMap()
    {
        //row0
        LoadTiles(this.tileR0, "mapPrefabs/stoneAge/r0/0,0");
        LoadTiles(this.tileR0, "mapPrefabs/stoneAge/r0/1,0");
        LoadTiles(this.tileR0, "mapPrefabs/stoneAge/r0/2,0");
        LoadTiles(this.tileR0, "mapPrefabs/stoneAge/r0/3,0");
        LoadTiles(this.tileR0, "mapPrefabs/stoneAge/r0/4,0");
        LoadTiles(this.tileR0, "mapPrefabs/stoneAge/r0/5,0");
        LoadTiles(this.tileR0, "mapPrefabs/stoneAge/r0/6,0");
        LoadTiles(this.tileR0, "mapPrefabs/stoneAge/r0/7,0");
        LoadTiles(this.tileR0, "mapPrefabs/stoneAge/r0/8,0");
        LoadTiles(this.tileR0, "mapPrefabs/stoneAge/r0/9,0");
        LoadTiles(this.tileR0, "mapPrefabs/stoneAge/r0/10,0");

        //row1
        LoadTiles(this.tileR1, "mapPrefabs/stoneAge/r1/0,1");
        LoadTiles(this.tileR1, "mapPrefabs/stoneAge/r1/1,1");
        LoadTiles(this.tileR1, "mapPrefabs/stoneAge/r1/2,1");
        LoadTiles(this.tileR1, "mapPrefabs/stoneAge/r1/3,1");
        LoadTiles(this.tileR1, "mapPrefabs/stoneAge/r1/4,1");
        LoadTiles(this.tileR1, "mapPrefabs/stoneAge/r1/5,1");
        LoadTiles(this.tileR1, "mapPrefabs/stoneAge/r1/6,1");
        LoadTiles(this.tileR1, "mapPrefabs/stoneAge/r1/7,1");
        LoadTiles(this.tileR1, "mapPrefabs/stoneAge/r1/8,1");
        LoadTiles(this.tileR1, "mapPrefabs/stoneAge/r1/9,1");
        LoadTiles(this.tileR1, "mapPrefabs/stoneAge/r1/10,1");

        //row2
        LoadTiles(this.tileR2, "mapPrefabs/stoneAge/r2/0,2");
        LoadTiles(this.tileR2, "mapPrefabs/stoneAge/r2/1,2");
        LoadTiles(this.tileR2, "mapPrefabs/stoneAge/r2/2,2");
        LoadTiles(this.tileR2, "mapPrefabs/stoneAge/r2/3,2");
        LoadTiles(this.tileR2, "mapPrefabs/stoneAge/r2/4,2");
        LoadTiles(this.tileR2, "mapPrefabs/stoneAge/r2/5,2");
        LoadTiles(this.tileR2, "mapPrefabs/stoneAge/r2/6,2");
        LoadTiles(this.tileR2, "mapPrefabs/stoneAge/r2/7,2");
        LoadTiles(this.tileR2, "mapPrefabs/stoneAge/r2/8,2");
        LoadTiles(this.tileR2, "mapPrefabs/stoneAge/r2/9,2");
        LoadTiles(this.tileR2, "mapPrefabs/stoneAge/r2/10,2");

        //row3
        LoadTiles(this.tileR3, "mapPrefabs/stoneAge/r3/0,3");
        LoadTiles(this.tileR3, "mapPrefabs/stoneAge/r3/1,3");
        LoadTiles(this.tileR3, "mapPrefabs/stoneAge/r3/2,3");
        LoadTiles(this.tileR3, "mapPrefabs/stoneAge/r3/3,3");
        LoadTiles(this.tileR3, "mapPrefabs/stoneAge/r3/4,3");
        LoadTiles(this.tileR3, "mapPrefabs/stoneAge/r3/5,3");
        LoadTiles(this.tileR3, "mapPrefabs/stoneAge/r3/6,3");
        LoadTiles(this.tileR3, "mapPrefabs/stoneAge/r3/7,3");
        LoadTiles(this.tileR3, "mapPrefabs/stoneAge/r3/8,3");
        LoadTiles(this.tileR3, "mapPrefabs/stoneAge/r3/9,3");
        LoadTiles(this.tileR3, "mapPrefabs/stoneAge/r3/10,3");

        //row4
        LoadTiles(this.tileR4, "mapPrefabs/stoneAge/r4/0,4");
        LoadTiles(this.tileR4, "mapPrefabs/stoneAge/r4/1,4");
        LoadTiles(this.tileR4, "mapPrefabs/stoneAge/r4/2,4");
        LoadTiles(this.tileR4, "mapPrefabs/stoneAge/r4/3,4");
        LoadTiles(this.tileR4, "mapPrefabs/stoneAge/r4/4,4");
        LoadTiles(this.tileR4, "mapPrefabs/stoneAge/r4/5,4");
        LoadTiles(this.tileR4, "mapPrefabs/stoneAge/r4/6,4");
        LoadTiles(this.tileR4, "mapPrefabs/stoneAge/r4/7,4");
        LoadTiles(this.tileR4, "mapPrefabs/stoneAge/r4/8,4");
        LoadTiles(this.tileR4, "mapPrefabs/stoneAge/r4/9,4");
        LoadTiles(this.tileR4, "mapPrefabs/stoneAge/r4/10,4");

        //row5
        LoadTiles(this.tileR5, "mapPrefabs/stoneAge/r5/0,5");
        LoadTiles(this.tileR5, "mapPrefabs/stoneAge/r5/1,5");
        LoadTiles(this.tileR5, "mapPrefabs/stoneAge/r5/2,5");
        LoadTiles(this.tileR5, "mapPrefabs/stoneAge/r5/3,5");
        LoadTiles(this.tileR5, "mapPrefabs/stoneAge/r5/4,5");
        LoadTiles(this.tileR5, "mapPrefabs/stoneAge/r5/5,5");
        LoadTiles(this.tileR5, "mapPrefabs/stoneAge/r5/6,5");
        LoadTiles(this.tileR5, "mapPrefabs/stoneAge/r5/7,5");
        LoadTiles(this.tileR5, "mapPrefabs/stoneAge/r5/8,5");
        LoadTiles(this.tileR5, "mapPrefabs/stoneAge/r5/9,5");
        LoadTiles(this.tileR5, "mapPrefabs/stoneAge/r5/10,5");

        //row6
        LoadTiles(this.tileR6, "mapPrefabs/stoneAge/r6/0,6");
        LoadTiles(this.tileR6, "mapPrefabs/stoneAge/r6/1,6");
        LoadTiles(this.tileR6, "mapPrefabs/stoneAge/r6/2,6");
        LoadTiles(this.tileR6, "mapPrefabs/stoneAge/r6/3,6");
        LoadTiles(this.tileR6, "mapPrefabs/stoneAge/r6/4,6");
        LoadTiles(this.tileR6, "mapPrefabs/stoneAge/r6/5,6");
        LoadTiles(this.tileR6, "mapPrefabs/stoneAge/r6/6,6");
        LoadTiles(this.tileR6, "mapPrefabs/stoneAge/r6/7,6");
        LoadTiles(this.tileR6, "mapPrefabs/stoneAge/r6/8,6");
        LoadTiles(this.tileR6, "mapPrefabs/stoneAge/r6/9,6");
        LoadTiles(this.tileR6, "mapPrefabs/stoneAge/r6/10,6");
        return;
    }

    private void LoadMedievalAgeMap()
    {
        this.spawnPos = new GridPos(1, 10);
        this.destinationPos = new GridPos(5, 0);
        //row0
        LoadTiles(this.tileR0, "mapPrefabs/medievalAge/r0/0,0");
        LoadTiles(this.tileR0, "mapPrefabs/medievalAge/r0/1,0");
        LoadTiles(this.tileR0, "mapPrefabs/medievalAge/r0/2,0");
        LoadTiles(this.tileR0, "mapPrefabs/medievalAge/r0/3,0");
        LoadTiles(this.tileR0, "mapPrefabs/medievalAge/r0/4,0");
        LoadTiles(this.tileR0, "mapPrefabs/medievalAge/r0/5,0");
        LoadTiles(this.tileR0, "mapPrefabs/medievalAge/r0/6,0");
        LoadTiles(this.tileR0, "mapPrefabs/medievalAge/r0/7,0");
        LoadTiles(this.tileR0, "mapPrefabs/medievalAge/r0/8,0");
        LoadTiles(this.tileR0, "mapPrefabs/medievalAge/r0/9,0");
        LoadTiles(this.tileR0, "mapPrefabs/medievalAge/r0/10,0");

        //row1
        LoadTiles(this.tileR1, "mapPrefabs/medievalAge/r1/0,1");
        LoadTiles(this.tileR1, "mapPrefabs/medievalAge/r1/1,1");
        LoadTiles(this.tileR1, "mapPrefabs/medievalAge/r1/2,1");
        LoadTiles(this.tileR1, "mapPrefabs/medievalAge/r1/3,1");
        LoadTiles(this.tileR1, "mapPrefabs/medievalAge/r1/4,1");
        LoadTiles(this.tileR1, "mapPrefabs/medievalAge/r1/5,1");
        LoadTiles(this.tileR1, "mapPrefabs/medievalAge/r1/6,1");
        LoadTiles(this.tileR1, "mapPrefabs/medievalAge/r1/7,1");
        LoadTiles(this.tileR1, "mapPrefabs/medievalAge/r1/8,1");
        LoadTiles(this.tileR1, "mapPrefabs/medievalAge/r1/9,1");
        LoadTiles(this.tileR1, "mapPrefabs/medievalAge/r1/10,1");

        //row2
        LoadTiles(this.tileR2, "mapPrefabs/medievalAge/r2/0,2");
        LoadTiles(this.tileR2, "mapPrefabs/medievalAge/r2/1,2");
        LoadTiles(this.tileR2, "mapPrefabs/medievalAge/r2/2,2");
        LoadTiles(this.tileR2, "mapPrefabs/medievalAge/r2/3,2");
        LoadTiles(this.tileR2, "mapPrefabs/medievalAge/r2/4,2");
        LoadTiles(this.tileR2, "mapPrefabs/medievalAge/r2/5,2");
        LoadTiles(this.tileR2, "mapPrefabs/medievalAge/r2/6,2");
        LoadTiles(this.tileR2, "mapPrefabs/medievalAge/r2/7,2");
        LoadTiles(this.tileR2, "mapPrefabs/medievalAge/r2/8,2");
        LoadTiles(this.tileR2, "mapPrefabs/medievalAge/r2/9,2");
        LoadTiles(this.tileR2, "mapPrefabs/medievalAge/r2/10,2");

        //row3
        LoadTiles(this.tileR3, "mapPrefabs/medievalAge/r3/0,3");
        LoadTiles(this.tileR3, "mapPrefabs/medievalAge/r3/1,3");
        LoadTiles(this.tileR3, "mapPrefabs/medievalAge/r3/2,3");
        LoadTiles(this.tileR3, "mapPrefabs/medievalAge/r3/3,3");
        LoadTiles(this.tileR3, "mapPrefabs/medievalAge/r3/4,3");
        LoadTiles(this.tileR3, "mapPrefabs/medievalAge/r3/5,3");
        LoadTiles(this.tileR3, "mapPrefabs/medievalAge/r3/6,3");
        LoadTiles(this.tileR3, "mapPrefabs/medievalAge/r3/7,3");
        LoadTiles(this.tileR3, "mapPrefabs/medievalAge/r3/8,3");
        LoadTiles(this.tileR3, "mapPrefabs/medievalAge/r3/9,3");
        LoadTiles(this.tileR3, "mapPrefabs/medievalAge/r3/10,3");

        //row4
        LoadTiles(this.tileR4, "mapPrefabs/medievalAge/r4/0,4");
        LoadTiles(this.tileR4, "mapPrefabs/medievalAge/r4/1,4");
        LoadTiles(this.tileR4, "mapPrefabs/medievalAge/r4/2,4");
        LoadTiles(this.tileR4, "mapPrefabs/medievalAge/r4/3,4");
        LoadTiles(this.tileR4, "mapPrefabs/medievalAge/r4/4,4");
        LoadTiles(this.tileR4, "mapPrefabs/medievalAge/r4/5,4");
        LoadTiles(this.tileR4, "mapPrefabs/medievalAge/r4/6,4");
        LoadTiles(this.tileR4, "mapPrefabs/medievalAge/r4/7,4");
        LoadTiles(this.tileR4, "mapPrefabs/medievalAge/r4/8,4");
        LoadTiles(this.tileR4, "mapPrefabs/medievalAge/r4/9,4");
        LoadTiles(this.tileR4, "mapPrefabs/medievalAge/r4/10,4");

        //row5
        LoadTiles(this.tileR5, "mapPrefabs/medievalAge/r5/0,5");
        LoadTiles(this.tileR5, "mapPrefabs/medievalAge/r5/1,5");
        LoadTiles(this.tileR5, "mapPrefabs/medievalAge/r5/2,5");
        LoadTiles(this.tileR5, "mapPrefabs/medievalAge/r5/3,5");
        LoadTiles(this.tileR5, "mapPrefabs/medievalAge/r5/4,5");
        LoadTiles(this.tileR5, "mapPrefabs/medievalAge/r5/5,5");
        LoadTiles(this.tileR5, "mapPrefabs/medievalAge/r5/6,5");
        LoadTiles(this.tileR5, "mapPrefabs/medievalAge/r5/7,5");
        LoadTiles(this.tileR5, "mapPrefabs/medievalAge/r5/8,5");
        LoadTiles(this.tileR5, "mapPrefabs/medievalAge/r5/9,5");
        LoadTiles(this.tileR5, "mapPrefabs/medievalAge/r5/10,5");

        //row6
        LoadTiles(this.tileR6, "mapPrefabs/medievalAge/r6/0,6");
        LoadTiles(this.tileR6, "mapPrefabs/medievalAge/r6/1,6");
        LoadTiles(this.tileR6, "mapPrefabs/medievalAge/r6/2,6");
        LoadTiles(this.tileR6, "mapPrefabs/medievalAge/r6/3,6");
        LoadTiles(this.tileR6, "mapPrefabs/medievalAge/r6/4,6");
        LoadTiles(this.tileR6, "mapPrefabs/medievalAge/r6/5,6");
        LoadTiles(this.tileR6, "mapPrefabs/medievalAge/r6/6,6");
        LoadTiles(this.tileR6, "mapPrefabs/medievalAge/r6/7,6");
        LoadTiles(this.tileR6, "mapPrefabs/medievalAge/r6/8,6");
        LoadTiles(this.tileR6, "mapPrefabs/medievalAge/r6/9,6");
        LoadTiles(this.tileR6, "mapPrefabs/medievalAge/r6/10,6");

        //row7
        LoadTiles(this.tileR7, "mapPrefabs/medievalAge/r7/0,7");
        LoadTiles(this.tileR7, "mapPrefabs/medievalAge/r7/1,7");
        LoadTiles(this.tileR7, "mapPrefabs/medievalAge/r7/2,7");
        LoadTiles(this.tileR7, "mapPrefabs/medievalAge/r7/3,7");
        LoadTiles(this.tileR7, "mapPrefabs/medievalAge/r7/4,7");
        LoadTiles(this.tileR7, "mapPrefabs/medievalAge/r7/5,7");
        LoadTiles(this.tileR7, "mapPrefabs/medievalAge/r7/6,7");
        LoadTiles(this.tileR7, "mapPrefabs/medievalAge/r7/7,7");
        LoadTiles(this.tileR7, "mapPrefabs/medievalAge/r7/8,7");
        LoadTiles(this.tileR7, "mapPrefabs/medievalAge/r7/9,7");
        LoadTiles(this.tileR7, "mapPrefabs/medievalAge/r7/10,7");

        //row8
        LoadTiles(this.tileR8, "mapPrefabs/medievalAge/r8/0,8");
        LoadTiles(this.tileR8, "mapPrefabs/medievalAge/r8/1,8");
        LoadTiles(this.tileR8, "mapPrefabs/medievalAge/r8/2,8");
        LoadTiles(this.tileR8, "mapPrefabs/medievalAge/r8/3,8");
        LoadTiles(this.tileR8, "mapPrefabs/medievalAge/r8/4,8");
        LoadTiles(this.tileR8, "mapPrefabs/medievalAge/r8/5,8");
        LoadTiles(this.tileR8, "mapPrefabs/medievalAge/r8/6,8");
        LoadTiles(this.tileR8, "mapPrefabs/medievalAge/r8/7,8");
        LoadTiles(this.tileR8, "mapPrefabs/medievalAge/r8/8,8");
        LoadTiles(this.tileR8, "mapPrefabs/medievalAge/r8/9,8");
        LoadTiles(this.tileR8, "mapPrefabs/medievalAge/r8/10,8");

        //row9
        LoadTiles(this.tileR9, "mapPrefabs/medievalAge/r9/0,9");
        LoadTiles(this.tileR9, "mapPrefabs/medievalAge/r9/1,9");
        LoadTiles(this.tileR9, "mapPrefabs/medievalAge/r9/2,9");
        LoadTiles(this.tileR9, "mapPrefabs/medievalAge/r9/3,9");
        LoadTiles(this.tileR9, "mapPrefabs/medievalAge/r9/4,9");
        LoadTiles(this.tileR9, "mapPrefabs/medievalAge/r9/5,9");
        LoadTiles(this.tileR9, "mapPrefabs/medievalAge/r9/6,9");
        LoadTiles(this.tileR9, "mapPrefabs/medievalAge/r9/7,9");
        LoadTiles(this.tileR9, "mapPrefabs/medievalAge/r9/8,9");
        LoadTiles(this.tileR9, "mapPrefabs/medievalAge/r9/9,9");
        LoadTiles(this.tileR9, "mapPrefabs/medievalAge/r9/10,9");

        //row10
        LoadTiles(this.tileR10, "mapPrefabs/medievalAge/r10/0,10");
        LoadTiles(this.tileR10, "mapPrefabs/medievalAge/r10/1,10");
        LoadTiles(this.tileR10, "mapPrefabs/medievalAge/r10/2,10");
        LoadTiles(this.tileR10, "mapPrefabs/medievalAge/r10/3,10");
        LoadTiles(this.tileR10, "mapPrefabs/medievalAge/r10/4,10");
        LoadTiles(this.tileR10, "mapPrefabs/medievalAge/r10/5,10");
        LoadTiles(this.tileR10, "mapPrefabs/medievalAge/r10/6,10");
        LoadTiles(this.tileR10, "mapPrefabs/medievalAge/r10/7,10");
        LoadTiles(this.tileR10, "mapPrefabs/medievalAge/r10/8,10");
        LoadTiles(this.tileR10, "mapPrefabs/medievalAge/r10/9,10");
        LoadTiles(this.tileR10, "mapPrefabs/medievalAge/r10/10,10");
    }
}
