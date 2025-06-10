using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private float _elevation = 0f;

    private float startPosition = 34.67f;
    private float _endPos = 2984.29f;
    private float _fadeDuration = 1f;

    protected void Update()
    {
        Vector3 pos = transform.position;
        if (pos.x >= _endPos)
        {
            Invoke("LoadNextLevel", _fadeDuration);
        }
    }
    protected void LoadNextLevel()
    {
        if ((_elevation >= (_endPos - startPosition - 1)) && (_elevation < (((_endPos-startPosition)*2)-2)))
        {
            SceneManager.LoadScene("BaseCampA");
        }
        else if ((_elevation >= (((_endPos-startPosition)*2)-2)) && _elevation < (((_endPos-startPosition)*3)-1))
        {
            SceneManager.LoadScene("BaseCampB");
        }
        else
        {
            SceneManager.LoadScene("Summit");
        }
    }
}
