using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerProjectile : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private Creep creepTarget;
    private TowerRange towerRange = null;
    private BarrackUnit barrackUnit = null;
    private const string projectileObjects = "ProjectileObjects";
    //private const string creep = "Creep";
    private GameObject parent;

    [SerializeField]
    private int damage;
    public int Damage
    {
        get { return damage; }
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
	
	void Update ()
    {
        MoveToCreep();

        Debug.Log(this.name);
        if(this.name == "Medieval_Light(Clone)")
        {
            Vector3 vectorToTarget = this.creepTarget.transform.position - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            angle -= 180;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 5.0f);
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
            DestroyObj();
    }

    private void DestroyObj()
    {
        Destroy(this.gameObject);
    }
}
