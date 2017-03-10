using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BarScript : MonoBehaviour {

    private float fillAmount;

    private Image content;
    private float lerpSpeed;
    [SerializeField]
    private Color fullColor;
    [SerializeField]
    private Color lowColor;

    public float MaxValue { get; set; }
    public float Value
    {
        set
        {
            fillAmount = Map(value, 0, MaxValue, 0, 1);
        }
    }

    private void Awake ()
    {
        this.content = this.transform.GetChild(0).GetChild(0).GetComponent<Image>();
    }

    private void Start()
    {
        this.lerpSpeed = 2.0f;
    }

    void Update ()
    {
        HandleBar();
	}

    private void HandleBar()
    {
        if(fillAmount != content.fillAmount)
        {
            content.fillAmount = Mathf.Lerp(content.fillAmount, fillAmount, lerpSpeed * Time.deltaTime);
        }

        content.color = Color.Lerp(lowColor, fullColor, fillAmount);
    }

    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
}
