using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldTower : MonoBehaviour
{
    [SerializeField]
    private int goldToAdd;
    private float timerForGold;
	
	void Start ()
    {
        this.timerForGold = 0;
	}
	
	
	void Update ()
    {
        AddGold();
	}

    private void AddGold()
    {
        this.timerForGold += Time.deltaTime;
        if(this.timerForGold > 5.0f)
        {
            GameHandler.Instance.Gold += this.goldToAdd;
            this.timerForGold = 0;
        }
    }
}
