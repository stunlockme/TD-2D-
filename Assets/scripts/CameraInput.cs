using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInput : MonoBehaviour {

    [SerializeField]
    private float camSpeed;

    private float xMax;
    private float yMin;

	void Start ()
    {
		
	}
	
	private void Update ()
    {
        PlayerInput();

        this.transform.position = new Vector3(Mathf.Clamp(this.transform.position.x, 0, this.xMax), Mathf.Clamp(this.transform.position.y, this.yMin, 0), -10.0f);
	}

    /// <summary>
    /// Moves the camera based on the player keyboard input
    /// </summary>
    private void PlayerInput()
    {
        if (Input.GetKey(KeyCode.W))
            MoveCam(Vector3.up);
        if (Input.GetKey(KeyCode.A))
            MoveCam(Vector3.left);
        if (Input.GetKey(KeyCode.S))
            MoveCam(Vector3.down);
        if (Input.GetKey(KeyCode.D))
            MoveCam(Vector3.right);
    }

    /// <summary>
    /// Moves the camera 
    /// </summary>
    private void MoveCam(Vector3 dir)
    {
        this.transform.Translate(dir * this.camSpeed * Time.deltaTime);
    }

    /// <summary>
    /// restricts camera to the bounds of the game world 
    /// </summary>
    public void RestrictCamera(Vector3 maxTile)
    {
        Vector3 worldPoint = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0.0f));
        this.xMax = maxTile.x - worldPoint.x;
        this.yMin = maxTile.y - worldPoint.y;
    }
}
