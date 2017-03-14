using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellTower : MonoBehaviour {

    private GameObject parentCanvas;
    private GameObject parentTower;
    private int sellPrice;
    private Text sellText;
    private TileData td;
	
	void Start ()
    {
        this.parentCanvas = this.transform.parent.gameObject;
        this.parentTower = this.parentCanvas.transform.parent.gameObject;
        this.sellPrice = this.parentTower.transform.GetChild(0).GetComponent<TowerRange>().TowerPrice;
        this.sellText = this.transform.GetChild(0).GetComponent<Text>();
        this.sellText.text = this.sellPrice.ToString();
	}
	
	private void Update ()
    {
        GetTileData();
	}

    public void RestoreGold()
    {
        GameHandler.Instance.Gold += this.sellPrice;
        td.UnLockDiagonalTiles();
        Destroy(this.parentTower);
    }

    private void GetTileData()
    {
        td = this.parentTower.transform.parent.GetComponent<TileData>();
    }
}
