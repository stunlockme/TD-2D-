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
	
	void Start ()
    {
        this.radius = 1.0f;
        this.centre = this.towerObj.transform.position + new Vector3(1.5f, -1.4f);
	}
	
	
	void Update ()
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
