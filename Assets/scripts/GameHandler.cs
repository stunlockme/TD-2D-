using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
        //load the cursor icon as a texture2d
        this.cursorTexture = Resources.Load(cursorName) as Texture2D;

        this.ranger = "ranger";
        this.puck = "puck";
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
        int creepIndex = Random.Range(0, 2);
        string type = string.Empty;
        switch(creepIndex)
        {
            case 0:
                type = this.ranger;
                break;
            case 1:
                type = this.puck;
                break;
        }
        GetType(type);
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
}
