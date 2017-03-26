using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityTower : MonoBehaviour
{
    [SerializeField]
    private Stat mana;

    private float xScaleMax;
    private float yScaleMax;

    private bool isActive = false;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        mana.Init();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start ()
    {
        this.xScaleMax = 25.0f;
        this.yScaleMax = 25.0f;
        this.spriteRenderer.enabled = false;
	}
	
	
	void Update ()
    {
        if(isActive)
        {
            this.spriteRenderer.enabled = true;
            this.transform.localScale += new Vector3(10.0f, 10.0f, 0) * Time.deltaTime;
            if (this.transform.localScale.x >= this.xScaleMax && this.transform.localScale.y >= this.yScaleMax)
            {
                this.transform.localScale = Vector3.one;
                isActive = false;
            }
        }
        else
        {
            this.spriteRenderer.enabled = false;
        }

        if(mana.CurrentVal < mana.MaxVal)
        {
            mana.CurrentVal += Time.deltaTime * 2.0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Invisible")
        {
            collision.tag = "Visible";
            Debug.Log("hello from tower");
        }
    }

    public void IncreaseScale()
    {
        mana.CurrentVal -= 20.0f;
        isActive = true;
    }
}
