using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UIScript : MonoBehaviour
{
    //references to objects in the scene
    [Header("Audio")]
    [SerializeField] private GameObject _itemSoundManager;
    [SerializeField] private Sound _drinkSound;
    [SerializeField] private Sound _airSound;
    [Header("User Interface Components")]
    [SerializeField] private Slider _oxygen = null;
    [SerializeField] private Slider _temperature = null;
    //[SerializeField] private Text _elevation = null;
    [SerializeField] private Text _oxygenBottles = null;
    [SerializeField] private Text _hotChocolateBottles = null;
    [SerializeField] private float _oxygenRateOfDecline = 5f;
    [SerializeField] private float _temperatureROD = 2f;
    [SerializeField] private int _oxygenCount = 10;
    [SerializeField] private int _hotChocolate = 10;
    //private internals
    private float _elapsedTimeO = 0f;
    private float _elapsedTimeT = 0f;
    private bool dead = false;
    private AudioSource _audio;
    // Start is called before the first frame update
    void Awake()
    {
        _audio = _itemSoundManager.GetComponent<AudioSource>();
    }
    void Start()
    {
        _oxygenBottles.text = _oxygenCount.ToString();
        _hotChocolateBottles.text = _hotChocolate.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        _elapsedTimeO += Time.deltaTime;
        _elapsedTimeT += Time.deltaTime;
        float t = _elapsedTimeO / _oxygenRateOfDecline;
        float t2 = _elapsedTimeT / _temperatureROD;
        if (!dead)
        {
            _oxygen.value = Mathf.Lerp(_oxygen.maxValue, _oxygen.minValue, t);
            _temperature.value = Mathf.Lerp(_temperature.maxValue, _temperature.minValue, t2);
        }
        if (!(_oxygen.value == _oxygen.minValue || _temperature.value == _temperature.minValue))
        {


            if (Input.GetKeyDown("a"))
            {
                if (_oxygenCount > 0)
                {
                    _audio.clip = _airSound.clip;
                    _audio.outputAudioMixerGroup = _airSound.mixer;
                    _audio.Play();
                    _oxygen.value = _oxygen.maxValue;
                    _elapsedTimeO = 0f;
                    _oxygenCount -= 1;
                    _oxygenBottles.text = _oxygenCount.ToString();
                }

            }
            if (Input.GetKeyDown("d"))
            {
                if (_hotChocolate > 0)
                {
                    _audio.clip = _drinkSound.clip;
                    _audio.outputAudioMixerGroup = _drinkSound.mixer;
                    _audio.Play();
                    _temperature.value = _temperature.maxValue;
                    _elapsedTimeT = 0f;
                    _hotChocolate -= 1;
                    _hotChocolateBottles.text = _hotChocolate.ToString();
                }
            }
        }
        else
        {
            dead = true;
        }
    }
}
