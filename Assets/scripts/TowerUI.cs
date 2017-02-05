using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour
{
    [SerializeField]
    private GameObject towerPrefab;
    public GameObject TowerPrefab
    { get { return towerPrefab; } }

    [SerializeField]
    private Sprite sprite;
    public Sprite Sprite
    {
        get { return sprite; }
    }

    [SerializeField]
    private int goldToBuildTwr;
    public int GoldToBuildTwr
    {
        get { return goldToBuildTwr; }
    }

    private Text towerGold;

    private void Awake()
    {
        //initialize the text component
        this.towerGold = this.transform.GetChild(0).GetComponent<Text>();
    }

    private void Start()
    {
        //set the text to the cost of tower
        this.towerGold.text = this.goldToBuildTwr.ToString();
    }
}
