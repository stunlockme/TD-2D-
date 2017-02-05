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

    private void Awake()
    {
        //load the cursor icon as a texture2d
        this.cursorTexture = Resources.Load(cursorName) as Texture2D;
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
}
