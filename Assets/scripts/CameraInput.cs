﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInput : MonoBehaviour
{
    private float camSpeed;
    private float xMax;
    private float yMin;
    private float zoomSpeed;
    private float targetOrtho;
    private float smoothSpeed;
    private float minOrtho;
    private float maxOrtho;
    private const string mouseWheel = "Mouse ScrollWheel";
    private AudioSource audioSource;


    private void Awake()
    {
        this.audioSource = GetComponent<AudioSource>();
    }


    private void Start ()
    {
        this.audioSource.Play();
        this.camSpeed = 5.0f;
        this.xMax = 0;
        this.yMin = 0;
        this.targetOrtho = Camera.main.orthographicSize;
        this.zoomSpeed = 5.0f;
        this.smoothSpeed = 10.0f;
        this.minOrtho = 1.0f;
        this.maxOrtho = 20.0f;
	}
	
	private void Update ()
    {
        PlayerInput();
        ResetCameraSize();
        this.transform.position = new Vector3(Mathf.Clamp(this.transform.position.x, 0, this.xMax), Mathf.Clamp(this.transform.position.y, this.yMin, 0), -10.0f);
        Zoom();

        if(Input.GetKey(KeyCode.X))
        {
            audioSource.Stop();
        }
        else if(Input.GetKey(KeyCode.C))
        {
            audioSource.Play();
        }
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
        return;
    }

    /// <summary>
    /// restricts camera to the bounds of the game world 
    /// </summary>
    public void RestrictCamera(Vector3 maxTile)
    {
        Vector3 worldPoint = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0.0f));
        this.xMax = maxTile.x - worldPoint.x;
        this.yMin = maxTile.y - worldPoint.y;
        return;
    }

    private void ResetCameraSize()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            this.targetOrtho = 8.0f;
        }
    }

    private void Zoom()
    {
        float scroll = Input.GetAxis(mouseWheel);
        if(scroll != 0.0f)
        {
            targetOrtho -= scroll * zoomSpeed;
            targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho);
        }
        Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, targetOrtho, smoothSpeed * Time.deltaTime);
    }
}
