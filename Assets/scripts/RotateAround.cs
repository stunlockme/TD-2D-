using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private GameObject towerObj;
    private float radius;

    private Vector2 centre;
    private float angle;
    private Vector2 offset;

    private SpriteRenderer spriterRenderer;

    private void Awake()
    {
        this.spriterRenderer = this.transform.GetComponent<SpriteRenderer>();
    }

    void Start ()
    {
        this.radius = 1.5f;
        this.centre = this.towerObj.transform.position + new Vector3(1.5f, -1.4f);
        this.spriterRenderer.sortingOrder = this.transform.parent.transform.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }
	
	
	private void Update ()
    {
        RotateObj();
    }

    private void RotateObj()
    {
        this.angle += this.rotateSpeed * Time.deltaTime;
        this.offset = new Vector2(Mathf.Sin(this.angle), Mathf.Cos(this.angle)) * this.radius;
        this.transform.position = this.centre + this.offset;
    }
}
