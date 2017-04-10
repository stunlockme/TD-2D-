using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerProjectile : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private Creep creepTarget;
    private TowerRange towerRange = null;
    private BarrackUnit barrackUnit = null;
    private const string projectileObjects = "ProjectileObjects";
    private const string medievalLight = "Medieval_Light(Clone)";
    //private const string creep = "Creep";
    private GameObject parent;

    [SerializeField]
    private int damage;
    public int Damage
    {
        get { return damage; }
    }

    [SerializeField]
    private List<GameObject> horizontalProjectileList;

    [SerializeField]
    private List<GameObject> verticalProjectileList;

    [SerializeField]
    private float stunTime;
    public float StunTime
    {
        get
        {
            return stunTime;
        }
    }

    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.parent = GameObject.FindGameObjectWithTag(projectileObjects);
        this.transform.SetParent(this.parent.transform);
    }

    private void Start ()
    {
        this.spriteRenderer.sortingOrder = 4;
	}
	
	private void Update ()
    {
        MoveToCreep();
        Rotate();
    }

    /// <summary>
    /// rotates arrow based on angle between the arrow and creep
    /// rotates rock on the z-axis
    /// </summary>
    private void Rotate()
    {
        if (this.name == medievalLight)
        {
            if (this.creepTarget != null)
            {
                Vector3 vectorToTarget = this.creepTarget.transform.position - transform.position;
                float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
                angle -= 180;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 5.0f);
            }
        }
        else
        {
            transform.Rotate(0, 0, Time.deltaTime * 100);
        }
    }

    /// <summary>
    /// initialize tower range to get the creep to attack
    /// </summary>
    /// <param name="towerRange"></param>
    public void Init(TowerRange towerRange = null, BarrackUnit barrackUnit = null)
    {
        if(towerRange != null)
        {
            this.towerRange = towerRange;
            this.creepTarget = this.towerRange.CreepTarget;
        }
        if(barrackUnit != null)
        {
            this.barrackUnit = barrackUnit;
            this.creepTarget = this.barrackUnit.CreepTarget;
        }
    }

    /// <summary>
    /// move projectile towards the creep
    /// </summary>
    private void MoveToCreep()
    {
        if (this.creepTarget != null && GameHandler.Instance.CreepsInScene.Contains(this.creepTarget.name))
        {
            if(this.towerRange != null)
                this.transform.position = Vector2.MoveTowards(this.transform.position, this.creepTarget.transform.position, this.towerRange.ProjectileSpeed * Time.deltaTime);
            else if (this.barrackUnit != null)
                this.transform.position = Vector2.MoveTowards(this.transform.position, this.creepTarget.transform.position, this.barrackUnit.ProjectileSpeed * Time.deltaTime);
        }
        else if (this.creepTarget.IsDead)
            DestroyObj();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == GameHandler.Instance.Visible)
        {
            if(this.horizontalProjectileList.Count > 0 || this.verticalProjectileList.Count > 0)
            {
                Creep creepHit = collision.GetComponent<Creep>();
                if(creepHit.IsMovingLeft || creepHit.IsMovingRight)
                {
                    Instantiate(this.horizontalProjectileList[0], this.transform.position, Quaternion.identity);
                    Instantiate(this.horizontalProjectileList[1], this.transform.position, Quaternion.identity);
                }
                else if(creepHit.IsMovingUp || creepHit.IsMovingDown)
                {
                    Instantiate(this.verticalProjectileList[0], this.transform.position, Quaternion.identity);
                    Instantiate(this.verticalProjectileList[1], this.transform.position, Quaternion.identity);
                }
                GameObject[] sp = GameObject.FindGameObjectsWithTag("SmallProjectile");
                foreach (GameObject projectile in sp)
                {
                    SmallProjectile spObj = projectile.transform.GetComponent<SmallProjectile>();
                    if(spObj.CreepGridPos.X <=0 && spObj.CreepGridPos.Y <= 0)
                        spObj.CreepGridPos = creepHit.GridPos;
                }
            }
            DestroyObj();
        }
    }

    private void DestroyObj()
    {
        Destroy(this.gameObject);
    }
}
