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

    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start ()
    {
        this.td = this.transform.parent.parent.gameObject.GetComponent<TileData>();
        this.gridPos = td.gridPosition;
        this.gridPos.X -= gridX;
        this.gridPos.Y -= gridY;
        this.attackIsActive = true;
        Debug.Log(this.gridPos.X + " " + this.gridPos.Y);
        LevelGenerator.Instance.tiles[this.gridPos].UnitOnTile = true;
    }

	void Update ()
    {
        if(LevelGenerator.Instance.tiles.ContainsKey(this.gridPos))
        {
            if (!LevelGenerator.Instance.tiles[this.gridPos].IsTowerPlaced)
                this.transform.position = Vector2.MoveTowards(this.transform.position, LevelGenerator.Instance.tiles[this.gridPos].centreOfTile, 2.0f * Time.deltaTime);
            else if(LevelGenerator.Instance.tiles[this.gridPos].IsTowerPlaced)
                spriteRenderer.enabled = false;
        }

        Attack();
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
