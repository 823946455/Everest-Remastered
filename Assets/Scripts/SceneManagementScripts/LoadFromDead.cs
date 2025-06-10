using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadFromDead : MonoBehaviour
{
    [SerializeField] private Button _retry = null;
    [SerializeField] private Button _menu = null;
    [SerializeField] private float fadeSpeed = 1.0f;
    [SerializeField] private Text _fade;
    protected AudioSource audioSource; //audio source on this gameobject with music clip assigned
    protected float audioVolume; //used to cache intial volume
    private Text _deadText;
    private Text _retryText;
    private Text _menuText;
    private bool _notQuitOrRetry = true;
    private bool _retryBool = true;

    void Awake()
    {
        _deadText = _fade;
        _retryText = _retry.GetComponentInChildren<Text>();
        _menuText = _menu.GetComponentInChildren<Text>();
        audioSource = GetComponent<AudioSource>();
        audioVolume = audioSource.volume;
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
        if (_fade.color == new Color(0, 0, 0, 1) && _fade.text == "DEAD" && _notQuitOrRetry)
        {
            _fade = _retryText;
            StartCoroutine(Fade(true, fadeSpeed));
        }
        else if (_fade.color == new Color(1, 1, 1, 1) && _fade.text == "Retry" && _notQuitOrRetry)
        {
            _fade = _menuText;
            StartCoroutine(Fade(true, fadeSpeed));
        }
    }
    IEnumerator Fade(bool fadeIn, float speed)
    {
        Color targetColor;
        Color sourceColor;
        if (_fade.text == "DEAD")
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
            _fade.color = Color.Lerp(sourceColor, targetColor, timer / speed);
            
            if (fadeIn && _fade.text != "Retry" && _fade.text != "Menu")
            {
                audioSource.volume = Mathf.Lerp(0, audioVolume, timer / speed);
            }
            else
            {
                if (_fade.text != "Retry" && _fade.text != "Menu")
                {
                    audioSource.volume = Mathf.Lerp(audioVolume, 0, timer / speed);
                }
            }
            timer += Time.deltaTime;
            yield return null;
        }
        _fade.color = targetColor;
        
    }
    IEnumerator FadeOut(bool fadeIn, float speed)
    {
       Color targetColor = fadeIn ? new Color(0, 0, 0, 1) : new Color(0, 0, 0, 0);
       Color sourceColor = fadeIn ? new Color(0, 0, 0, 0) : new Color(0, 0, 0, 1);
        

      float timer = 0;
      while (timer <= fadeSpeed)
      {
          _fade.color = Color.Lerp(sourceColor, targetColor, timer / speed);
          if (_retryBool)
          {
                _retryText.color = Color.Lerp(sourceColor, targetColor, timer / speed);
          }
          else
          {
                _menuText.color = Color.Lerp(sourceColor, targetColor, timer / speed);
          }
          if (fadeIn)
          {
              audioSource.volume = Mathf.Lerp(0, audioVolume, timer / speed);
          }
          else
          {
             audioSource.volume = Mathf.Lerp(audioVolume, 0, timer / speed);
          }
          timer += Time.deltaTime;
          yield return null;
      }
      _fade.color = targetColor;
      _retryText.color = targetColor;
    }
    public void QuitGame()
    {
        _notQuitOrRetry = false;
        _fade = _deadText;
        StartCoroutine(FadeOut(false, fadeSpeed));
        Invoke("LoadMenu", fadeSpeed);
    }
    protected void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void Retry1()
    {
        _notQuitOrRetry = false;
        _retryBool = false;
        _fade = _deadText;
        StartCoroutine(FadeOut(false, fadeSpeed));
        Invoke("Restart1", fadeSpeed);
    }
    protected void Restart1()
    {
        SceneManager.LoadScene("EverestLevel1");
    }
    public void Retry2()
    {
        _notQuitOrRetry = false;
        _retryBool = false;
        _fade = _deadText;
        StartCoroutine(FadeOut(false, fadeSpeed));
        Invoke("Restart2", fadeSpeed);
    }
    protected void Restart2()
    {
        SceneManager.LoadScene("EverestLevel2");
    }
    public void Retry3()
    {
        _notQuitOrRetry = false;
        _retryBool = false;
        _fade = _deadText;
        StartCoroutine(FadeOut(false, fadeSpeed));
        Invoke("Restart3", fadeSpeed);
    }
    protected void Restart3()
    {
        SceneManager.LoadScene("EverestLevel3");
    }
}
