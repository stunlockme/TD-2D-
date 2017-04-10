using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallProjectile : MonoBehaviour
{
    private GridPos creepGridPos;

    public GridPos CreepGridPos
    {
        get
        {
            return creepGridPos;
        }

        set
        {
            creepGridPos = value;
        }
    }

    private Creep creep;
    [SerializeField]
    private Vector2 changeGridPos;

    [SerializeField]
    private int damage;
    public int Damage
    {
        get { return damage; }
    }

    private GameObject parent;
    private const string projectileObjects = "ProjectileObjects";

    [SerializeField]
    private float projectileSpeed;

    private void Awake()
    {
        this.parent = GameObject.FindGameObjectWithTag(projectileObjects);
        this.transform.SetParent(this.parent.transform);
    }

    void Start ()
    {
        Debug.Log(this.creepGridPos);
	}
	
	
	void Update ()
    {
        Move();
        transform.Rotate(0, 0, Time.deltaTime * 200);
        Debug.Log("creepGridPos is : " + creepGridPos.X + " " + creepGridPos.Y);
	}

    public void Move()
    {
        if(this.creepGridPos.X > 0 && this.creepGridPos.Y > 0)
        {
            GridPos tmp = new GridPos(creepGridPos.X - (int)changeGridPos.x, creepGridPos.Y - (int)changeGridPos.y);
            Vector3 destination = LevelGenerator.Instance.tiles[tmp].centreOfTile;
            this.transform.position = Vector2.MoveTowards(this.transform.position, destination, this.projectileSpeed * Time.deltaTime);
            if (this.transform.position == destination)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == GameHandler.Instance.Visible)
            StartCoroutine(DestroyObj());
    }

    private IEnumerator DestroyObj()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        Destroy(this.gameObject);
    }
}
