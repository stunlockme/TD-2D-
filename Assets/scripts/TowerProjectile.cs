using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerProjectile : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private Creep creepTarget;
    private TowerRange towerRange;
    private const string projectileObjects = "ProjectileObjects";
    private const string creep = "Creep";
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
	}

    public void Init(TowerRange towerRange)
    {
        this.towerRange = towerRange;
        this.creepTarget = this.towerRange.CreepTarget;
    }

    private void MoveToCreep()
    {
        if (this.creepTarget != null && GameHandler.Instance.CreepsInScene.Contains(this.creepTarget.name))
            this.transform.position = Vector2.MoveTowards(this.transform.position, this.creepTarget.transform.position, this.towerRange.ProjectileSpeed * Time.deltaTime);
        else if (this.creepTarget.IsDead)
            DestroyObj();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == creep)
            DestroyObj();
    }

    private void DestroyObj()
    {
        Destroy(this.gameObject);
    }
}
