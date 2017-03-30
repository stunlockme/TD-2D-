using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrackUnit : MonoBehaviour
{
    private TileData td;
    private GridPos gridPos;
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private int gridX;
    [SerializeField]
    private int gridY;

    private Queue<Creep> creepQueue = new Queue<Creep>();
    private Creep creepTarget;
    public Creep CreepTarget
    {
        get { return creepTarget; }
    }
    [SerializeField]
    private string tpType;

    private bool attackIsActive;
    private float timer;

    [SerializeField]
    private float timeBtwAttack;

    [SerializeField]
    private float projectileSpeed;
    public float ProjectileSpeed
    {
        get { return projectileSpeed; }
    }

    [SerializeField]
    private Stat health;
    public Stat Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
        }
    }

    private bool returnToBase = false;
    private Vector3 basePos;
    private Vector3 attackPos;
    private float criticalHealth;

    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.health.Init();
    }

    void Start ()
    {
        this.td = this.transform.parent.parent.gameObject.GetComponent<TileData>();
        this.basePos = LevelGenerator.Instance.tiles[this.td.gridPosition].centreOfTile;
        this.gridPos = td.gridPosition;
        this.gridPos.X -= gridX;
        this.gridPos.Y -= gridY;
        this.attackPos = LevelGenerator.Instance.tiles[this.gridPos].centreOfTile;
        this.attackIsActive = true;
        //Debug.Log(this.gridPos.X + " " + this.gridPos.Y);
        LevelGenerator.Instance.tiles[this.gridPos].UnitOnTile = true;
        this.criticalHealth = 2.0f;
    }

	void Update ()
    {
        Movement();
        Attack();
    }

    private void Movement()
    {
        if (LevelGenerator.Instance.tiles.ContainsKey(this.gridPos))
        {
            if (!LevelGenerator.Instance.tiles[this.gridPos].IsTowerPlaced && this.health.CurrentVal > this.criticalHealth)
            {
                if (!returnToBase)
                    this.transform.position = Vector2.MoveTowards(this.transform.position, LevelGenerator.Instance.tiles[this.gridPos].centreOfTile, 2.0f * Time.deltaTime);
                if (this.transform.position == this.attackPos)
                    LevelGenerator.Instance.tiles[this.gridPos].UnitOnTile = true;
            }
            else if (LevelGenerator.Instance.tiles[this.gridPos].IsTowerPlaced)
                spriteRenderer.enabled = false;
        }
        if (this.health.CurrentVal <= this.criticalHealth)
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position, LevelGenerator.Instance.tiles[this.td.gridPosition].centreOfTile, 2.0f * Time.deltaTime);
            this.creepTarget = null;
            //spriteRenderer.enabled = false;
            LevelGenerator.Instance.tiles[this.gridPos].UnitOnTile = false;
            this.returnToBase = true;
            //Debug.Log("moving back");
        }

        if (returnToBase && this.transform.position == this.basePos)
        {
            this.health.CurrentVal += 0.01f;
            if (this.health.CurrentVal >= 9.0f)
                returnToBase = false;
        }
        //Debug.Log(td.gridPosition.X + " " + td.gridPosition.Y);
    }

    private void Attack()
    {
        if (!this.attackIsActive)
        {
            this.timer += Time.deltaTime;
            if (this.timer > this.timeBtwAttack)
            {
                this.attackIsActive = true;
                this.timer = 0;
            }
        }

        if (this.creepTarget == null && this.creepQueue.Count > 0)
        {
            this.creepTarget = this.creepQueue.Dequeue();
        }

        if (this.creepTarget != null)
        {
            if (GameHandler.Instance.CreepsInScene.Contains(this.creepTarget.name))
            {
                if (this.attackIsActive)
                {
                    if (this.creepTarget.transform.gameObject.tag == GameHandler.Instance.Visible)
                    {
                        TowerProjectile tp = GameHandler.Instance.GetTowerProjectileType(this.tpType).GetComponent<TowerProjectile>();
                        tp.transform.position = this.transform.position;
                        tp.Init(null, this);
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
}
