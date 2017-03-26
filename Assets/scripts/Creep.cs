using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creep : MonoBehaviour
{
    private float speed;
    private Vector3 destination;
    private Stack<Node> wayPoints;
    private GridPos gridPos;
    public GridPos GridPos
    {
        get { return gridPos; }
        private set { ;}
    }

    private float timer;
    private const string parentName = "CreepObjects";
    private float timeToDestroy;
    private GameObject parent;
    private bool isDead;
    public bool IsDead
    {
        get { return isDead; }
    }

    [SerializeField]
    private Stat health;

    private Animator animator;
    private const string vertical = "Vertical";
    private const string horizontal = "Horizontal";

    private void Awake()
    {
        this.animator = this.transform.GetComponent<Animator>();
        this.animator.enabled = true;
        health.Init();
        this.gridPos = LevelGenerator.Instance.SpawnPos;
    }

    private void Start()
    {
        this.speed = 1.0f;
        this.timer = 0;
        this.timeToDestroy = 2.5f;
        this.parent = GameObject.FindGameObjectWithTag(parentName);
        this.transform.SetParent(this.parent.transform);
        this.isDead = false;
    }

    private void Update()
    {
        if (this.wayPoints != null)
            MoveToDestination();

        if(this.gameObject.tag == "Visible")
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }

        if(this.health.CurrentVal == 0)
        {
            StartCoroutine(DestroyObj(0));
        }

        WaitToCalWayPoints(2.0f);
    }

    /// <summary>
    /// moves the creep to the destroy gate
    /// </summary>
    private void MoveToDestination()
    {
        this.transform.position = Vector2.MoveTowards(this.transform.position, this.destination, this.speed * Time.deltaTime);

        if (this.transform.position == this.destination)
        {
            if (this.wayPoints != null && this.wayPoints.Count > 0)
            {
                SetAnimTrigger(this.gridPos, this.wayPoints.Peek().gridPos);
                this.gridPos = this.wayPoints.Peek().gridPos;
                this.destination = this.wayPoints.Pop().worldPos;
            }
        }
        if(this.gridPos == LevelGenerator.Instance.DestinationPos)
        {
            StartCoroutine(DestroyObj(this.timeToDestroy));
        }
        return;
    }

    /// <summary>
    /// finds the waypoints to destination
    /// sets the destination to the top node in the stack
    /// </summary>
    /// <param name="wayPoints"></param>
    /// <param name="n"></param>
    private void FindWayPoints(Stack<Node> wayPoints, string n)
    {
        if (n == this.name)
        {
            if (wayPoints != null)
            {
                this.wayPoints = new Stack<Node>();
                this.wayPoints = wayPoints;
                SetAnimTrigger(this.gridPos, this.wayPoints.Peek().gridPos);
                this.gridPos = this.wayPoints.Peek().gridPos;
                this.destination = this.wayPoints.Pop().worldPos;
                return;
            }
        }
    }

    /// <summary>
    /// spawns this object at the starting position
    /// sets the waypoints for this creep 
    /// </summary>
    public void Spawn()
    {
        this.transform.position = LevelGenerator.Instance.creepGate.transform.position;
        FindWayPoints(LevelGenerator.Instance.WayPoints, this.name);
        return;
    }

    /// <summary>
    /// destroy this gameobject
    /// </summary>
    /// <param name="timeToDestroy"></param>
    /// <returns></returns>
    private IEnumerator DestroyObj(float timeToDestroy)
    {
        yield return new WaitForSeconds(timeToDestroy);
        this.isDead = true;
        yield return new WaitForSeconds(0.2f);
        GameHandler.Instance.RemoveFromScene(this.name);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// re-calculate the waypoints
    /// </summary>
    /// <param name="reloadTime"></param>
    private void WaitToCalWayPoints(float reloadTime)
    {
        this.timer += Time.deltaTime;

        if (this.timer > reloadTime)
        {
            this.wayPoints = PathData.CalcPath(this.gridPos, LevelGenerator.Instance.DestinationPos);
            this.timer = 0;
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Projectile")
        {
            TowerProjectile tp = collision.GetComponent<TowerProjectile>();
            this.health.CurrentVal -= tp.Damage;
            return;
        }
    }

    /// <summary>
    /// play's animation using objects direction
    /// </summary>
    /// <param name="currentPos"></param>
    /// <param name="nextPos"></param>
    private void SetAnimTrigger(GridPos currentPos, GridPos nextPos)
    {
        if(currentPos.Y > nextPos.Y)
        {
            //up
            this.animator.SetInteger(vertical, 1);
            this.animator.SetInteger(horizontal, 0);
        }
        else if(currentPos.Y < nextPos.Y)
        {
            //down
            this.animator.SetInteger(vertical, -1);
            this.animator.SetInteger(horizontal, 0);
        }
        if(currentPos.Y == nextPos.Y)
        {
            if(currentPos.X > nextPos.X)
            {
                //left
                this.animator.SetInteger(vertical, 0);
                this.animator.SetInteger(horizontal, -1);
            }
            else if(currentPos.X < nextPos.X)
            {
                //right
                this.animator.SetInteger(vertical, 0);
                this.animator.SetInteger(horizontal, 1);
            }
        }
    }
}
