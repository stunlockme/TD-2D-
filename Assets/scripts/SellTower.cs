using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellTower : MonoBehaviour {

    private Canvas parentCanvas;
    private GameObject parentTower;
    private SpriteRenderer parentRenderer;
    private int sellPrice;
    private Text sellText;
    private TileData td;
	
	void Start ()
    {
        this.parentCanvas = this.transform.parent.GetComponent<Canvas>();
        this.parentTower = this.parentCanvas.transform.parent.gameObject;
        this.parentRenderer = this.parentTower.GetComponent<SpriteRenderer>();
        this.sellPrice = this.parentTower.transform.GetChild(0).GetComponent<TowerRange>().TowerPrice;
        this.sellText = this.transform.GetChild(0).GetComponent<Text>();
        this.sellText.text = this.sellPrice.ToString();
	}
	
	private void Update ()
    {
        if(this.td == null)
        {
            GetTileData();
            this.parentCanvas.sortingOrder = this.parentRenderer.sortingOrder + 1;
            return;
        }
	}

    /// <summary>
    /// restores gold to player, unlocks tiles
    /// </summary>
    public void RestoreGold()
    {
        GameHandler.Instance.Gold += this.sellPrice;
        td.UnLockDiagonalTiles();
        Destroy(this.parentTower);
        return;
    }

    private void GetTileData()
    {
        this.td = this.parentTower.transform.parent.GetComponent<TileData>();
    }
}
