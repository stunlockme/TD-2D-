using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    private Image fadeImg;
    private float fadeSpeed = 100.0f;
    public bool sceneStarting;
    private float lerpT;

    private bool fadeToMap; 

    private void Awake()
    {
        this.fadeImg = this.transform.GetComponent<Image>();
        this.fadeImg.rectTransform.localScale = new Vector2(Screen.width, Screen.height);
        this.sceneStarting = true;
    }

    void Start ()
    {
		
	}
	
	
	void Update ()
    {
        if (sceneStarting)
            StartScene();
	}

    private void FadeToClear()
    {
        this.fadeImg.color = Color.Lerp(this.fadeImg.color, Color.clear, this.lerpT);
        if(lerpT < 1)
        {
            this.lerpT += Time.deltaTime / 500.0f;
        }
    }

    private void StartScene()
    {
        FadeToClear();

        if(this.fadeImg.color.a <= 0.05f)
        {
            this.fadeImg.color = Color.clear;
            this.fadeImg.enabled = false;
            this.sceneStarting = false;
        }
    }
}
