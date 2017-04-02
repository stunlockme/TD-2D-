using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    private const string gameScreen = "game_screen";
	
	void Start ()
    {

    }
	
	
	void Update ()
    {
		
	}

    public void LoadGame()
    {
        SceneManager.LoadScene(gameScreen, LoadSceneMode.Single);
    }
}
