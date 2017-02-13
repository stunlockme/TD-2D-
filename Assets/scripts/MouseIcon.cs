using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseIcon : Singleton<MouseIcon>
{
    private SpriteRenderer spriteRenderer;
    private Camera cam;
    [SerializeField]
    private GameObject towerButtons;

    private void Awake()
    {
        //initialize the spriteRenderer component
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();

        //initialize the camera
        this.cam = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        GetMousePos();
        DisableUI();
    }

    /// <summary>
    /// sets the selected tower to follow the mouse
    /// </summary>
    private void GetMousePos()
    {
        if(this.spriteRenderer.enabled)
        {
            //set this object position to the mouse position in world space
            this.transform.position = this.cam.ScreenToWorldPoint(Input.mousePosition);
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
        }
    }

    /// <summary>
    /// gets the sprite the player has clicked
    /// </summary>
    /// <param name="sprite"></param>
    public void GetSprite(Sprite sprite)
    {
        //get the sprite from the game handler class
        this.spriteRenderer.sprite = sprite;

        //enable sprite renderer
        this.spriteRenderer.enabled = true;
    }

    /// <summary>
    /// stops drawing
    /// set sprite to null
    /// </summary>
    public void DisableRenderer()
    {
        //disable sprite renderer
        //this.spriteRenderer.enabled = false;

        //set the sprite to null
        this.spriteRenderer.sprite = null;

        //set user interface object to null
        GameHandler.Instance.selectedBtn = null;
    }

    private void DisableUI()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            if (this.spriteRenderer.sprite == null)
                this.towerButtons.SetActive(!this.towerButtons.activeSelf);
        }
    }
}
