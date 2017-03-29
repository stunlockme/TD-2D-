using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerRange : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private Creep creepTarget;
    public Creep CreepTarget
    {
        get { return creepTarget; }
    }
    private Queue<Creep> creepQueue = new Queue<Creep>();

    [SerializeField]
    private string tpType;

    [SerializeField]
    private float timeBtwAttack;

    [SerializeField]
    private float projectileSpeed;
    public float ProjectileSpeed
    {
        get { return projectileSpeed; }
    }

    private float timer;
    private bool attackIsActive;

    [SerializeField]
    private GameObject sellCanvas;
    public GameObject SellCanvas
    { get { return sellCanvas; } }

    [SerializeField]
    private int towerPrice;
    public int TowerPrice
    { get { return towerPrice; } }

    private const string barracks = "_barracks(Clone)";

    [SerializeField]
    private Text dmgText;


    private void Start ()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.attackIsActive = true;
	}
	
	
	private void Update ()
    {
        Attack();
    }

    /// <summary>
    /// activate or de-activate the renderer
    /// </summary>
    public void ActivateRange()
    {
        this.spriteRenderer.enabled = !this.spriteRenderer.enabled;
    }

    /// <summary>
    /// attacks each creep based on a time delay
    /// initializes the tower projectile
    /// </summary>
    private void Attack()
    {
        Debug.Log(this.transform.parent.gameObject.name);
        if (this.transform.parent.gameObject.name == barracks)
            return;
        if(!this.attackIsActive)
        {
            this.timer += Time.deltaTime;
            if(this.timer > this.timeBtwAttack)
            {
                this.attackIsActive = true;
                this.timer = 0;
            }
        }

        if (this.creepTarget == null && this.creepQueue.Count > 0)
        {
            this.creepTarget = this.creepQueue.Dequeue();
        }

        if(this.creepTarget != null)
        {
            if(GameHandler.Instance.CreepsInScene.Contains(this.creepTarget.name))
            {
                if(this.attackIsActive)
                {
                    if(this.creepTarget.transform.gameObject.tag == GameHandler.Instance.Visible)
                    {
                        TowerProjectile tp = GameHandler.Instance.GetTowerProjectileType(this.tpType).GetComponent<TowerProjectile>();
                        tp.transform.position = this.transform.position;
                        tp.Init(this);
                        this.attackIsActive = false;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == GameHandler.Instance.Visible)
            this.creepQueue.Enqueue(collision.GetComponent<Creep>());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == GameHandler.Instance.Visible)
            this.creepTarget = null;
    }

    public void DisableSellBtn()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            this.sellCanvas.SetActive(!this.sellCanvas.activeSelf);
            if(tpType == "circle")
                this.dmgText.text = "Dmg: 1\n" + "Dmg: 2\n";
        }
    }
}
