using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : Singleton<GameHandler>
{
    public TowerUI selectedBtn { get; set; }
    private Text goldText;
    private const string goldTextName = "GoldText";

    [SerializeField]
    private int gold;

    private int waveCount;
    private int creepsToSpawn;
    private Text waveText;
    private const string waveTextName = "WaveText";

    [SerializeField]
    private GameObject spawnCreepBtn;

    private int tmpGold;
    private Texture2D cursorTexture;
    private Vector2 cursorOffset;
    private CursorMode cursorMode;
    [SerializeField]
    private string cursorName;

    [SerializeField]
    private List<GameObject> creepList;

    [SerializeField]
    private List<string> creepsInScene;
    public bool CreepsInScene
    {
        get { return creepsInScene.Count > 0; }
    }

    private string ranger;
    private string puck;

    private string creepType;
    private static System.Random random = new System.Random();
    private const string charToGet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    private void Awake()
    {
        //load the cursor icon as a texture2d
        this.cursorTexture = Resources.Load(cursorName) as Texture2D;

        this.ranger = "ranger";
        this.puck = "puck";

        this.waveText = GameObject.Find(waveTextName).GetComponent<Text>();
        this.goldText = GameObject.Find(goldTextName).GetComponent<Text>();
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

        this.waveCount = 0;
        this.creepsToSpawn = 0;
        return;
    }
	
	
	private void Update ()
    {
        HandleKeyboard();
        GoldUsed();
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
            return;
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
        return;
    }

    /// <summary>
    /// player input to cancel selected tower
    /// </summary>
    private void HandleKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.F))
            MouseIcon.Instance.DisableRenderer();
        return;
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
            return;
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
        this.waveCount++;
        this.creepsToSpawn = this.waveCount * 5;
        this.waveText.text = this.waveCount.ToString();
        StartCoroutine(CreateCreep());
        this.spawnCreepBtn.SetActive(false);
    }

    /// <summary>
    /// create creeps of random type
    /// </summary>
    /// <returns></returns>
    private IEnumerator CreateCreep()
    {
        for (int i = 0; i < creepsToSpawn; i++)
        {
            LevelGenerator.Instance.CreateWayPoints(LevelGenerator.Instance.SpawnPos, LevelGenerator.Instance.DestinationPos);
            int creepIndex = Random.Range(0, 2);
            creepType = string.Empty;
            switch (creepIndex)
            {
                case 0:
                    creepType = this.ranger;
                    break;
                case 1:
                    creepType = this.puck;
                    break;
            }
            Creep creep = GetCreepType(creepType).GetComponent<Creep>();
            creep.name = RandomString(4);
            this.creepsInScene.Add(creep.name);
            creep.Spawn();
            yield return new WaitForSeconds(1.0f);
        }
    }

    /// <summary>
    /// loop through the creep list to find the creep type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private GameObject GetCreepType(string type)
    {
        for (int i = 0; i < creepList.Count; i++)
        {
            if(creepList[i].name == type)
            {
                GameObject creepObj = Instantiate(creepList[i]);
                creepObj.name = type;
                return creepObj;
            }
        }
        return null;
    }

    /// <summary>
    /// creates a unique string 
    /// </summary>
    /// <param name="length"></param>
    /// <returns>random string of specified size</returns>
    private static string RandomString(int length)
    {
        return new string(Enumerable.Repeat(charToGet, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// removes the creep from list
    /// </summary>
    /// <param name="creepName"></param>
    public void RemoveFromScene(string creepName)
    {
        this.creepsInScene.Remove(creepName);
        if (!this.CreepsInScene)
            this.spawnCreepBtn.SetActive(true);
    }
}
