using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SummitLoad : MonoBehaviour
{
    //Serialized fields in inspector
    [SerializeField] private Button _credits = null;
    [SerializeField] private Button _menu = null;
    [SerializeField] private float fadeSpeed = 1.0f;
    [SerializeField] private Text _fadeText;
    //private variables
    private Text _summitText;
    private Text _creditsText;
    private Text _menuText;
    private bool _creditsBool = true;
    private bool _notCreditsorMenu = true;
    void Awake()
    {
        _summitText = _fadeText;
        _creditsText = _credits.GetComponentInChildren<Text>();
        _menuText = _menu.GetComponentInChildren<Text>();

    }
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine(Fade(true, fadeSpeed));
    }

    // Update is called once per frame
    void Update()
    {
        if (_fadeText.color == new Color(0, 0, 0, 1) && _fadeText.text == "Summit Reached" && _notCreditsorMenu)
        {
            _fadeText = _creditsText;
            StartCoroutine(Fade(true, fadeSpeed));
        }
        else if (_fadeText.color == new Color(1, 1, 1, 1) && _fadeText.text == "Credits" && _notCreditsorMenu)
        {
            _fadeText = _menuText;
            StartCoroutine(Fade(true, fadeSpeed));
        }
    }
    IEnumerator Fade(bool fadeIn, float speed)
    {
        Color targetColor;
        Color sourceColor;
        if (_fadeText.text == "Summit Reached")
        {
            targetColor = fadeIn ? new Color(0, 0, 0, 1) : new Color(0, 0, 0, 0);
            sourceColor = fadeIn ? new Color(0, 0, 0, 0) : new Color(0, 0, 0, 1);
        }
        else
        {
            targetColor = fadeIn ? new Color(1, 1, 1, 1) : new Color(0, 0, 0, 0);
            sourceColor = fadeIn ? new Color(0, 0, 0, 0) : new Color(1, 1, 1, 1);
        }

        float timer = 0;
        while (timer <= fadeSpeed)
        {
            _fadeText.color = Color.Lerp(sourceColor, targetColor, timer / speed);
            timer += Time.deltaTime;
            yield return null;
        }
        _fadeText.color = targetColor;
    }
    IEnumerator FadeOut(bool fadeIn, float speed)
    {
        Color targetColor = fadeIn ? new Color(0, 0, 0, 1) : new Color(0, 0, 0, 0);
        Color sourceColor = fadeIn ? new Color(0, 0, 0, 0) : new Color(0, 0, 0, 1);
        float timer = 0;
        while (timer <= fadeSpeed)
        {
            _fadeText.color = Color.Lerp(sourceColor, targetColor, timer / speed);
            if (_creditsBool)
            {
                _creditsText.color = Color.Lerp(sourceColor, targetColor, timer / speed);
            }
            else
            {
                _menuText.color = Color.Lerp(sourceColor, targetColor, timer / speed);
            }
            timer += Time.deltaTime;
            yield return null;
        }
        _fadeText.color = targetColor;
        _creditsText.color = targetColor;
    }
    public void QuitGame()
    {
        _notCreditsorMenu = false;
        _fadeText = _summitText;
        StartCoroutine(FadeOut(false, fadeSpeed));
        Invoke("LoadMenu", fadeSpeed);
    }
    protected void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void ShowCredits()
    {
        _notCreditsorMenu = false;
        _creditsBool = false;
        _fadeText = _summitText;
        StartCoroutine(FadeOut(false, fadeSpeed));
        Invoke("LoadCredits", fadeSpeed);
    }
    protected void LoadCredits()
    {
       SceneManager.LoadScene("Credits");
    }
}
