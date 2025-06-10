using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class CreditSceneManager : MonoBehaviour
{
    [SerializeField] protected RectTransform _creditsContainer;
    [SerializeField] protected Image _fade;
    [SerializeField] protected float _finalScrollPos;
    [SerializeField] protected float _creditsDuration;
    [SerializeField] protected float _fadeDuration;

    //Internals
    protected AudioSource _audioSource;
    protected float _audioVolume;
    protected Vector3 _intialScrollPos;
    private bool _isFading = false;
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioVolume = _audioSource.volume;
        _audioSource.volume = 0;
        _intialScrollPos = _creditsContainer.localPosition;
        StartCoroutine(Fade(true, _fadeDuration));
        StartCoroutine(CreditRoll());
        StartCoroutine(Music());
    }
    IEnumerator CreditRoll()
    {
        yield return new WaitForSeconds(_fadeDuration);
        float timer = 0;
        while (timer < _creditsDuration)
        {
            Vector3 containerPos = _intialScrollPos;
            containerPos.y = Mathf.Lerp(_intialScrollPos.y, _finalScrollPos, timer / _creditsDuration);
            _creditsContainer.localPosition = containerPos;
            timer += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator Music()
    {
        float timer = 0;
        while (timer < _fadeDuration)
        {
            _audioSource.volume = Mathf.Lerp(0, _audioVolume, timer / _fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(_creditsDuration);
        if (!_isFading)
        {
            timer = 0;
            Color fadeColor = _fade.color;

            while (timer <= _fadeDuration)
            {
                _audioSource.volume = Mathf.Lerp(0, _audioVolume, 1 - (timer / _fadeDuration));
                fadeColor.a = timer / _fadeDuration;
                _fade.color = fadeColor;
                timer += Time.deltaTime;
                yield return null;
            }
            SceneManager.LoadScene("MainMenu");
        }
    }
    IEnumerator Fade(bool fadeIn, float speed)
    {
        Color targetColor = fadeIn ? new Color(1, 1, 1, 0) : new Color(1, 1, 1, 1);
        Color sourceColor = fadeIn ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0);

        float timer = 0;
        while (timer <= _fadeDuration)
        {
            _fade.color = Color.Lerp(sourceColor, targetColor, timer / speed);
            if (fadeIn)
            {
                _audioSource.volume = Mathf.Lerp(0, _audioVolume, timer / speed);
            }
            else
            {
                _audioSource.volume = Mathf.Lerp(_audioVolume, 0, timer / speed);
            }
            timer += Time.deltaTime;
            yield return null;
        }
        _fade.color = targetColor;
    }
    public void MenuReturn()
    {
        _isFading = true;
        StartCoroutine(Fade(false, _fadeDuration));
        Invoke("ReturnToMenu", _fadeDuration);
    }
    protected void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
