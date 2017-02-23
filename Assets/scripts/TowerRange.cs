using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRange : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
	
	private void Start ()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
	}
	
	
	void Update ()
    {
		
	}

    public void ActivateRange()
    {
        this.spriteRenderer.enabled = !this.spriteRenderer.enabled;
    }
}
