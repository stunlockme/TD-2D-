using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHandler : Singleton<GameHandler>
{
    public TowerUI selectedBtn { get; set; }
    private Text goldText;
    private const string goldTextName = "GoldText";

    [SerializeField]
    private int gold;
    public int Gold
    {
        get
        {
            return gold;
        }

        set
        {
            gold = value;
        }
    }

    [SerializeField]
    private int livesLeft;
    public int LivesLeft
    {
        get
        {
            return livesLeft;
        }
        set
        {
            livesLeft = value;
        }
    }

    [SerializeField]
    private Text livesLeftText;
    public Text LivesLeftText
    {
        get
        {
            return livesLeftText;
        }
        set
        {
            livesLeftText = value;
        }
    }

    [SerializeField]
    private GameObject pauseText;

    [SerializeField]
    private GameObject loadImgObj;

    [SerializeField]
    private GameObject menuBtn;

    private int waveCount;
    public int WaveCount
    {
        get
        {
            return waveCount;
        }
    }
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
    public List<string> CreepsInScene
    {
        get { return creepsInScene; }
    }
    public bool CreepCountInScene
    {
        get { return creepsInScene.Count > 0; }
    }

    private string caveMan;
    private string bigCaveMan;

    private string creepType;
    private static System.Random random = new System.Random();
    private const string charToGet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private TowerRange towerRange;

    [SerializeField]
    private List<GameObject> towerProjectileList;

    private GameObject barrackUnit;
    public GameObject BarrackUnit
    {
        get { return barrackUnit; }
    }

    private string visible;
    public string Visible
    {
        get
        {
            if(visible == null)
                visible = "Visible";
            return visible;
        }
    }

    private ScreenFade screenFade;

    [SerializeField]
    private GameObject MenuBtn;

    private GameObject spawnGateObj;

    private void Awake()
    {
        //load the cursor icon as a texture2d
        this.cursorTexture = Resources.Load(cursorName) as Texture2D;

        this.caveMan = "caveMan";
        this.bigCaveMan = "bigCaveMan";

        this.waveText = GameObject.Find(waveTextName).GetComponent<Text>();
        this.goldText = GameObject.Find(goldTextName).GetComponent<Text>();

        this.barrackUnit = Resources.Load("ranger_custom", typeof(GameObject)) as GameObject;
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

        this.waveCount = 2;
        this.creepsToSpawn = 0;

        this.livesLeftText.text = this.livesLeft.ToString();
        return;
    }
	
	
	private void Update ()
    {
        HandleKeyboard();
        GoldUsed();
        PauseGame();

        if(this.spawnGateObj == null)
        {
            this.spawnGateObj = GameObject.FindGameObjectWithTag("SpawnPos");
        }

        if (this.livesLeft == 0)
        {
            StartCoroutine(LoadGameOver());
        }
    }

    private void LoadMenu()
    {
        if (this.screenFade != null)
        {
            this.menuBtn.SetActive(false);
            this.pauseText.SetActive(false);
            if (!this.screenFade.sceneStarting)
                SceneManager.LoadScene("menu_screen", LoadSceneMode.Single);
        }
    }

    private IEnumerator LoadGameOver()
    {
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene("gameOver_screen", LoadSceneMode.Single);
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
            this.spawnGateObj.SetActive(true);
            LevelGenerator.Instance.CreateWayPoints(LevelGenerator.Instance.SpawnPos, LevelGenerator.Instance.DestinationPos);
            int creepIndex = Random.Range(0, 2);
            creepType = string.Empty;
            switch (creepIndex)
            {
                case 0:
                    creepType = this.caveMan;
                    break;
                case 1:
                    creepType = this.bigCaveMan;
                    break;
            }
            Creep creep = GetCreepType(creepType).GetComponent<Creep>();
            creep.name = RandomString(4);
            this.creepsInScene.Add(creep.name);
            creep.Spawn();
            yield return new WaitForSeconds(2.0f);
            this.spawnGateObj.SetActive(false);
            yield return new WaitForSeconds(2.0f);
        }
    }

    /// <summary>
    /// loop through the creep list to find the creep type
    /// </summary>
    /// <param name="type"></param>
    /// <returns>creep object if type exists in the creep list</returns>
    private GameObject GetCreepType(string type)
    {
        for (int i = 0; i < creepList.Count; i++)
        {
            if(creepList[i].name == type)
            {
                GameObject creepObj = Instantiate(creepList[i]);
                //creepObj.name = type;
                return creepObj;
            }
        }
        return null;
    }

    public GameObject GetTowerProjectileType(string type)
    {
        for (int i = 0; i < towerProjectileList.Count; i++)
        {
            if(towerProjectileList[i].name == type)
            {
                GameObject tp = Instantiate(towerProjectileList[i]);
                return tp;
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
        if (!this.CreepCountInScene)
            this.spawnCreepBtn.SetActive(true);
    }

    /// <summary>
    /// choose the tower range
    /// </summary>
    /// <param name="towerRange"></param>
    public void ChooseTowerRange(TowerRange towerRange)
    {
        if (this.towerRange != null)
            this.towerRange.ActivateRange();
        this.towerRange = towerRange;
        this.towerRange.ActivateRange();
    }

    /// <summary>
    /// de-selects the tower range
    /// </summary>
    public void RemoveTowerRange()
    {
        if (this.towerRange != null)
            this.towerRange.ActivateRange();

        this.towerRange = null;
    }

    private void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1.0f)
            {
                Time.timeScale = 0;
                this.pauseText.SetActive(true);
                this.menuBtn.SetActive(true);
            }
            else if (Time.timeScale == 0)
            {
                Time.timeScale = 1.0f;
                this.pauseText.SetActive(false);
                this.menuBtn.SetActive(false);
            }
        }
    }

    public void BackToMenu()
    {
        Time.timeScale = 1.0f;
        loadImgObj.SetActive(true);
        this.screenFade = this.loadImgObj.transform.GetComponent<ScreenFade>();
        //this.fadeImg.color = Color.Lerp(this.fadeImg.color, Color.black, this.fadeSpeed * Time.deltaTime);
    }
}
