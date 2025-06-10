using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingLevel : MonoBehaviour
{
    [SerializeField] protected Image fade = null; //black full screen UI image to use for fade in / fade out
    [SerializeField] protected float fadeSpeed = 1f; //speed to fade
    //Internals
    protected AudioSource audioSource; //audio source on this gameobject with music clip assigned
    protected float audioVolume; //used to cache intial volume

    protected void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        fade.color = new Color(0, 0, 0, 1);

        audioSource = GetComponent<AudioSource>();
        audioVolume = audioSource.volume;

        StartCoroutine(Fade(true, fadeSpeed));
    }
    IEnumerator Fade(bool fadeIn, float speed)
    {
        Color targetColor = fadeIn ? new Color(0, 0, 0, 0) : new Color(1, 1, 1, 1);
        Color sourceColor = fadeIn ? new Color(1, 1, 1, 1) : new Color(0, 0, 0, 0);

        float timer = 0;
        while (timer <= fadeSpeed)
        {
            fade.color = Color.Lerp(sourceColor, targetColor, timer / speed);
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
        fade.color = targetColor;
    }
    public void NewGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        StartCoroutine(Fade(false, fadeSpeed));
        Invoke("LoadGameScene", fadeSpeed);
    }
    public void Directions()
    {
        StartCoroutine(Fade(false, fadeSpeed));
        Invoke("LoadDirections", fadeSpeed);
    }
    public void LoadLevelTwo()
    {
        StartCoroutine(Fade(false, fadeSpeed));
        Invoke("LoadSecondLevel", fadeSpeed);
    }
    protected void LoadSecondLevel()
    {
        SceneManager.LoadScene("EverestLevel2");
    }
    public void LoadLevelThree()
    {
        StartCoroutine(Fade(false, fadeSpeed));
        Invoke("LoadThirdLevel", fadeSpeed);
    }
    protected void LoadThirdLevel()
    {
        SceneManager.LoadScene("EverestLevel3");
    }
    protected void LoadDirections()
    {
        SceneManager.LoadScene("Directions");
    }
    protected void LoadGameScene()
    {
        SceneManager.LoadScene("EverestLevel1");
    }
    public void QuitGame()
    {
        StartCoroutine(Fade(false, fadeSpeed));
        Invoke("Exit", fadeSpeed);
    }
    public void MenuReturn()
    {
        StartCoroutine(Fade(false, fadeSpeed));
        Invoke("ReturnToMenu", fadeSpeed);
    }
    protected void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    protected void Exit()
    {
        Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
