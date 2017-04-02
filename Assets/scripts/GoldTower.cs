using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldTower : MonoBehaviour {

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
            GameHandler.Instance.Gold += 1;
            this.timerForGold = 0;
        }
    }
}
