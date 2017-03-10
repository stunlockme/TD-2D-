using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRange : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private const string creepStr = "Creep";
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

    private void Attack()
    {
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
            this.creepTarget = this.creepQueue.Dequeue();

        if(this.creepTarget != null)
        {
            if(GameHandler.Instance.CreepsInScene.Contains(this.creepTarget.name))
            {
                if(this.attackIsActive)
                {
                    TowerProjectile tp = GameHandler.Instance.GetTowerProjectileType(this.tpType).GetComponent<TowerProjectile>();
                    tp.transform.position = this.transform.position;
                    tp.Init(this);
                    this.attackIsActive = false;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == creepStr)
            this.creepQueue.Enqueue(collision.GetComponent<Creep>());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == creepStr)
            this.creepTarget = null;
    }
}
