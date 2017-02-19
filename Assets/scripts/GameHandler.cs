using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : Singleton<GameHandler>
{
    public TowerUI selectedBtn { get; set; }

    [SerializeField]
    private Text goldText;

    [SerializeField]
    private int gold;

    private int tmpGold;
    private Texture2D cursorTexture;
    private Vector2 cursorOffset;
    private CursorMode cursorMode;
    [SerializeField]
    private string cursorName;

    [SerializeField]
    private List<GameObject> creepList;

    private string ranger;
    private string puck;

    private string creepType;
    public string CreepType
    {
        get
        {
            return creepType;
        }
        set
        {
            creepType = value;
        }
    }

    public List<Creep> creepObjects;
    private static System.Random random = new System.Random();
    private const string charToGet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    public Dictionary<string, Creep> creepInfo { get; private set; }

    public List<string> creepNames;

    public bool b { get; set; }

    private void Awake()
    {
        //load the cursor icon as a texture2d
        this.cursorTexture = Resources.Load(cursorName) as Texture2D;

        this.ranger = "ranger";
        this.puck = "puck";

        this.creepInfo = new Dictionary<string, Creep>();
    }

    private void Start ()
    {
        //set gold text to the starting value
        this.goldText.text = this.gold.ToString();
        this.tmpGold = this.gold;
        //Cursor.visible = false;

        //set the cursor offset to zero
        this.cursorOffset = Vector2.zero;

        //set the cursor mode to render as hardware cursor
        this.cursorMode = CursorMode.Auto;

        SetCustomCursor();
    }
	
	
	private void Update ()
    {
        HandleKeyboard();
        GoldUsed();

        //if (this.creepNames.Count > 0)
        //{
        //    for (int i = 0; i < this.creepNames.Count; i++)
        //    {
        //        Creep tmp = GameObject.Find(this.creepNames[i]).transform.GetComponent<Creep>();
        //        tmp.FindWayPoints(LevelGenerator.Instance.CreateWayPoints(tmp.GridPos, LevelGenerator.Instance.DestinationPos), this.creepNames[i]);
        //    }
        //}
    }

    /// <summary>
    /// selects tower if player has enough gold
    /// </summary>
    /// <param name="towerUI"></param>
    public void SelectTower(TowerUI towerUI)
    {
        //check if player has enough gold to build tower
        if(this.gold >= towerUI.GoldToBuildTwr)
        {
            this.selectedBtn = towerUI;
            MouseIcon.Instance.GetSprite(towerUI.Sprite);
            //Debug.Log(towerUI.GoldToBuildTwr);
        }
    }
    /// <summary>
    /// takes resources from player
    /// </summary>
    public void ResetTower()
    {
        //take gold from player
        this.gold -= this.selectedBtn.GoldToBuildTwr;

        //disable sprite renderer on the mouse
        MouseIcon.Instance.DisableRenderer();
    }

    /// <summary>
    /// player input to cancel selected tower
    /// </summary>
    private void HandleKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.F))
            MouseIcon.Instance.DisableRenderer();
    }

    /// <summary>
    /// updates gold text only when changed
    /// </summary>
    private void GoldUsed()
    {
        //check if gold has changed and update the text on screen
        if (this.gold != this.tmpGold)
        {
            this.goldText.text = this.gold.ToString();
            this.tmpGold = this.gold;
        }
    }

    /// <summary>
    /// sets the cursor to use custom icon
    /// </summary>
    private void SetCustomCursor()
    {
        Cursor.SetCursor(cursorTexture, this.cursorOffset, this.cursorMode);
        return;
    }

    /// <summary>
    /// spawn creeps when button is clicked
    /// </summary>
    public void SpawnCreeps()
    {
        StartCoroutine(CreateCreep());
    }

    /// <summary>
    /// create creeps of random type
    /// </summary>
    /// <returns></returns>
    private IEnumerator CreateCreep()
    {
        LevelGenerator.Instance.CreateWayPoints(LevelGenerator.Instance.SpawnPos, LevelGenerator.Instance.DestinationPos);
        int creepIndex = Random.Range(0, 2);
        creepType = string.Empty;
        switch(creepIndex)
        {
            case 0:
                creepType = this.ranger;
                break;
            case 1:
                creepType = this.puck;
                break;
        }
        Creep creep = GetType(creepType).GetComponent<Creep>();
        creep.name = RandomString(4);
        creep.Spawn();
        creepNames.Add(creep.name);
        //creepObjects.Add(creep);
        //this.creepInfo.Add(creep.name, creep);
        //Debug.Log(creep.name);

        yield return new WaitForSeconds(1.0f);
    }

    /// <summary>
    /// loop through the creep list to find the creep type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private GameObject GetType(string type)
    {
        for (int i = 0; i < creepList.Count; i++)
        {
            if(creepList[i].name == type)
            {
                GameObject tmpObj = Instantiate(creepList[i]);
                tmpObj.name = type;
                return tmpObj;
            }
        }
        return null;
    }

    private static string RandomString(int length)
    {
        return new string(Enumerable.Repeat(charToGet, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
