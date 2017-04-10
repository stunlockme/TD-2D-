using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creep : MonoBehaviour
{
    [SerializeField]
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

    private Animator animator;
    private const string vertical = "Vertical";
    private const string horizontal = "Horizontal";
    private const string projectile = "Projectile";
    private const string smallProjectile = "smallProjectile";
    private const string Unit = "Unit";
    private const string stun = "Stun";

    private BarrackUnit barrackUnitTarget;
    private Queue<BarrackUnit> unitQueue = new Queue<BarrackUnit>();

    private float timerToAttack = 0;
    private bool attackIsActive = false;
    private bool isStunned;
    private float stunTimer;
    private float timeToBeStunned;
    private bool fightingUnit;
    public bool FightingUnit
    {
        get
        {
            return fightingUnit;
        }

        set
        {
            fightingUnit = value;
        }
    }

    private bool isMovingRight;
    public bool IsMovingRight
    {
        get
        {
            return isMovingRight;
        }
    }

    private bool isMovingLeft;
    public bool IsMovingLeft
    {
        get
        {
            return isMovingLeft;
        }
    }

    private bool isMovingUp;
    public bool IsMovingUp
    {
        get
        {
            return isMovingUp;
        }
    }

    private bool isMovingDown;
    public bool IsMovingDown
    {
        get
        {
            return isMovingDown;
        }
    }

    private void Awake()
    {
        this.animator = this.transform.GetComponent<Animator>();
        this.animator.enabled = true;
        health.Init();
        this.gridPos = LevelGenerator.Instance.SpawnPos;
    }

    private void Start()
    {
        //this.speed = 1.0f;
        this.timer = 0;
        this.timeToDestroy = 2.5f;
        this.parent = GameObject.FindGameObjectWithTag(parentName);
        this.transform.SetParent(this.parent.transform);
        this.isDead = false;
    }

    private void Update()
    {
        if (this.wayPoints != null)
        {
            if (!isStunned && !fightingUnit)
                MoveToDestination();
            else
                Attack();
        }

        if(this.isStunned)
        {
            this.stunTimer += Time.deltaTime;
            if(this.stunTimer > this.timeToBeStunned)
            {
                isStunned = false;
                stunTimer = 0;
            }
        }

        if(this.gameObject.tag == "Visible")
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        if (this.health.CurrentVal == 0)
        {
            StartCoroutine(DestroyObj(0));
        }

        WaitToCalWayPoints(2.0f);
    }

    private void Attack()
    {

        if (this.barrackUnitTarget == null && this.unitQueue.Count > 0)
        {
            this.barrackUnitTarget = this.unitQueue.Dequeue();
        }
        if (!attackIsActive)
        {
            timerToAttack += Time.deltaTime;
            if (this.timerToAttack > 1.0f)
            {
                this.attackIsActive = true;
                this.timerToAttack = 0;
            }
        }
        if (this.barrackUnitTarget != null)
        {
            if (attackIsActive)
            {
                this.barrackUnitTarget.Health.CurrentVal -= 1.0f;
                this.attackIsActive = false;
            }
            if (this.gridPos.X >= this.barrackUnitTarget.Gridpos.X + 1)
            {
                this.barrackUnitTarget = null;
            }
        }
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
    public void Spawn(GridPos spawnTile)
    {
        //List<Vector3> spawnPosList = new List<Vector3>();
        //spawnPosList.Add(LevelGenerator.Instance.creepGate.transform.position);
        //spawnPosList.Add(LevelGenerator.Instance.creepGate2.transform.position);
        //int spawnIndex = Random.Range(0, 2);
        this.transform.position = LevelGenerator.Instance.tiles[spawnTile].centreOfTile;
        //this.transform.position = LevelGenerator.Instance.creepGate.transform.position;
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
        this.animator.SetBool("IsDead", true);
        yield return new WaitForSeconds(1.0f);
        if(this.health.CurrentVal > 0)
        {
            GameHandler.Instance.LivesLeft -= 1;
            GameHandler.Instance.LivesLeftText.text = GameHandler.Instance.LivesLeft.ToString();
        }
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
        if(collision.tag == projectile)
        {
            TowerProjectile tp = collision.GetComponent<TowerProjectile>();
            this.health.CurrentVal -= tp.Damage;
            return;
        }
        if(collision.tag == smallProjectile)
        {
            SmallProjectile sp = collision.GetComponent<SmallProjectile>();
            this.health.CurrentVal -= sp.Damage;
            return;
        }
        if (collision.tag == stun)
        {
            TowerProjectile tp = collision.GetComponent<TowerProjectile>();
            this.timeToBeStunned = tp.StunTime;
            isStunned = true;
        }

        if(collision.tag == Unit)
        {
            this.unitQueue.Enqueue(collision.GetComponent<BarrackUnit>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == Unit)
        {
            this.barrackUnitTarget = null;
            //Debug.Log("target removed");
        }
    }

    /// <summary>
    /// play's animation using objects direction
    /// </summary>
    /// <param name="currentPos"></param>
    /// <param name="nextPos"></param>
    private void SetAnimTrigger(GridPos currentPos, GridPos nextPos)
    {
        if (currentPos.Y > nextPos.Y)
        {
            //up
            this.animator.SetInteger(vertical, 1);
            this.animator.SetInteger(horizontal, 0);
            this.isMovingUp = true;
            this.isMovingDown = false;
            this.isMovingLeft = false;
            this.isMovingRight = false;
        }
        else if (currentPos.Y < nextPos.Y)
        {
            //down
            this.animator.SetInteger(vertical, -1);
            this.animator.SetInteger(horizontal, 0);
            this.isMovingUp = false;
            this.isMovingDown = true;
            this.isMovingLeft = false;
            this.isMovingRight = false;
        }
        if (currentPos.Y == nextPos.Y)
        {
            if (currentPos.X > nextPos.X)
            {
                //left
                this.animator.SetInteger(vertical, 0);
                this.animator.SetInteger(horizontal, -1);
                this.isMovingUp = false;
                this.isMovingDown = false;
                this.isMovingLeft = true;
                this.isMovingRight = false;
            }
            else if (currentPos.X < nextPos.X)
            {
                //right
                this.animator.SetInteger(vertical, 0);
                this.animator.SetInteger(horizontal, 1);
                this.isMovingUp = false;
                this.isMovingDown = false;
                this.isMovingLeft = false;
                this.isMovingRight = true;
            }
        }
    }
}
