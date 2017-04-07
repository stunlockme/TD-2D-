using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    private const string gameScreen = "game_screen";
    [SerializeField]
    private GameObject loadScreen;

    [SerializeField]
    private Image playImage;

    [SerializeField]
    private Button playBtn;

    [SerializeField]
    private Text playText;

    [SerializeField]
    private GameObject loadingImage;

    private ScreenFade screenFade;

    [SerializeField]
    private Image menuBackGround;

    [SerializeField]
    private GameObject quitBtnObj;

    private void Awake()
    {
 
        menuBackGround.rectTransform.localScale = new Vector2(Screen.width, Screen.height);
    }

    private void Start ()
    {
        this.screenFade = this.loadingImage.transform.GetComponent<ScreenFade>();
        Debug.Log(screenFade);
    }
	
	
	void Update ()
    {
        if (!screenFade.sceneStarting)
            SceneManager.LoadScene(gameScreen, LoadSceneMode.Single);
    }

    public void LoadGame()
    {
        this.playImage.enabled = false;
        this.playBtn.enabled = false;
        this.playText.enabled = false;
        this.quitBtnObj.SetActive(false);
        //this.loadScreen.SetActive(true);
        loadingImage.SetActive(true);
        //StartCoroutine(WaitToLoadMap());
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    private IEnumerator WaitToLoadMap()
    {
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene(gameScreen, LoadSceneMode.Single);
    }
}
