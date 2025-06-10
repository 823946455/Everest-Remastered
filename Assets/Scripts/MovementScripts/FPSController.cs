using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public enum PlayerMoveStatus { NotMoving, Walking, Crouching}

public class FPSController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private GameObject _windSoundManager;
    [SerializeField] private GameObject _inhaleSound;
    [SerializeField] private GameObject _freezingSound;
    [SerializeField] private GameObject _everestInterface;
    [SerializeField] private Sound _slowWindSound;
    [SerializeField] private Sound _fastWindSound;

    [Header("User Interface")]
    [SerializeField] private Slider _oxygen = null;
    [SerializeField] private Slider _temperature = null;
    [SerializeField] private Text _windSpeed = null;
    [SerializeField] private Text _elevation = null;
    [SerializeField] private Image _screenTransition = null;
    [SerializeField] private float _fadeDuration = 1f;
    [Header("Weather Events")]
    [SerializeField] private ParticleSystem _weather= null;
    [SerializeField] private int[] _windRate = { 10, 20, 30 }; //array of wind speeds
    [SerializeField] private int _windInt=10; //starting wind speed
    [SerializeField] private float _windBlowback = 0.1f;
    [SerializeField] private float _windSpeedRateOfChange = 5f;
    [Header("Elevation Starting Position")]
    [SerializeField] private float _elevationHeight = 0f;
    [Header("Death Variables")]
    [SerializeField] private float sideToSideSpeed = 1f;      // Speed of side-to-side tilting
    [SerializeField] private float sideToSideDuration = 2f;  // Duration of side-to-side movement
    [SerializeField] private float forwardRotationSpeed = 2f; // Speed of falling forward
    [SerializeField] private float tiltAngle = 15f;           // Maximum tilt angle for side-to-side movement
    [SerializeField] private float forwardAngle = 90f;        // Maximum forward fall angle

    //Inspector assigned movement settings
    [Header("Movement")]
    [SerializeField] private float _walkSpeed = 0.5f;
    //use standard assets mouse look class for mouse input -> camera look control
    [Header("Mouse Movement")]
    [SerializeField] private UnityStandardAssets.Characters.FirstPerson.MouseLook _mouseLook;
    
    //private internals
    private Camera _camera=null;
    private float startPosition = 34.67f;
    private float endPosition = 2984.29f;
    private Vector2 _inputVector = Vector2.zero;
    private Vector3 _moveDirection = Vector3.zero;
    private bool _isCrouching = false;
    private PlayerMoveStatus _moveStatus = PlayerMoveStatus.NotMoving;
    private Vector3 _localSpaceCameraPos = Vector3.zero;
    private float sideToSideTimer = 0f;     // Timer for side-to-side animation
    private float forwardRotation = 0f;    // Tracks forward rotation
    private float sideToSideTimeElapsed = 0f; // Tracks how long the side-to-side motion has occurred
    private bool sideToSideComplete = false; // Flag to check if side-to-side motion is done
    private AudioSource _audio;
    protected AudioSource audioSource; //audio source on this gameobject with music clip assigned
    protected float audioVolume; //used to cache intial volume
    private float _elevationInt = 0f;
    //private bool dead = false;
    //getters
    public PlayerMoveStatus moveStatus { get { return _moveStatus; } }
    public float walkSpeed { get { return _walkSpeed; } }
    
    protected void Awake()
    {
        _audio = _windSoundManager.GetComponent<AudioSource>();
        audioSource = _everestInterface.GetComponent<AudioSource>();
        audioVolume = audioSource.volume;
        
    }
    protected void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //get the main camera and cache local position to FPS Controller
        _camera = Camera.main;
        _localSpaceCameraPos = _camera.transform.localPosition;
        //set intial to not moving
        _moveStatus= PlayerMoveStatus.NotMoving;
        //Set up mouse look script
        _mouseLook.Init(transform, _camera.transform);
        _windSpeed.text = _windInt.ToString() +" mph";
       
        _elevation.text = _elevationHeight.ToString("#,0")+" m";
        StartCoroutine(ChangeWinds());
        StartCoroutine(Fade(true, 1));

    }
    protected void Update()
    {

        Vector3 pos = transform.position;
        if (!(transform.position.x >= endPosition))
        { 
            if (!(_oxygen.value == _oxygen.minValue || _temperature.value == _temperature.minValue))
            {
                if (Input.GetKey("w") && _windInt != _windRate[2])
                {
                    _moveStatus = PlayerMoveStatus.Walking;
                    pos.x = pos.x + walkSpeed;
                    transform.position = pos;
                }
                else if (Input.GetKey("w") && _isCrouching && _windInt == _windRate[2])
                {
                    pos.x = pos.x + walkSpeed;
                    transform.position = pos;
                }
                else if (_windInt == _windRate[2] && transform.position.x >= startPosition && !_isCrouching && !Input.GetKey("w"))
                {
                    pos.x = pos.x - _windBlowback;
                    transform.position = pos;
                }
                else
                {
                    _moveStatus = PlayerMoveStatus.NotMoving;
                }
                if (Input.GetKeyDown("c"))
                {
                    if (!_isCrouching)
                    {
                        pos.y = pos.y / 2;
                        _walkSpeed = _walkSpeed / 4;
                    }
                    else
                    {
                        pos.y = pos.y * 2;
                        _walkSpeed = _walkSpeed * 4;
                    }
                    transform.position = pos;
                    _isCrouching = !_isCrouching;
                }
                //allow mouse look a chance to process mouse and rotate camera
                if (Time.timeScale > Mathf.Epsilon)
                    _mouseLook.LookRotation(transform, _camera.transform);
                _elevationInt = _elevationHeight+(transform.position.x - startPosition); //starting position
                _elevation.text = _elevationInt.ToString("#,0") + " m"; //set position to starting text
                if (transform.position.x >= endPosition) //if winning position has been reached
                    StartCoroutine(Fade(false, 1)); //fade to white
            }
            else
            {
                _weather.transform.SetParent(null);
                if (!sideToSideComplete)
                {
                    if (_oxygen.value == _oxygen.minValue)
                    {
                        _inhaleSound.SetActive(true);
                    }
                    else
                    {
                        _freezingSound.SetActive(true);
                    }
                    // Perform side-to-side motion
                    sideToSideTimer += Time.deltaTime * sideToSideSpeed;
                    float tilt = Mathf.Sin(sideToSideTimer) * tiltAngle;
                    transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y, tilt);


                    // Increment the elapsed time for side-to-side motion
                    sideToSideTimeElapsed += Time.deltaTime;
                    if (sideToSideTimeElapsed >= sideToSideDuration)
                    {
                        sideToSideComplete = true; // Move to the forward fall phase
                    }
                }
                else
                {
                    // Perform forward fall
                    if (forwardRotation < forwardAngle)
                    {
                        forwardRotation += Time.deltaTime * forwardRotationSpeed;
                        forwardRotation = Mathf.Min(forwardRotation, forwardAngle);
                        transform.localEulerAngles = new Vector3(forwardRotation, transform.eulerAngles.y, 0);

                    }
                    else
                    {
                        StartCoroutine(Fade(false, 1));
                        Invoke("LoadDeathScreen", _fadeDuration);
                    }
                }
            }
        }
        else
        {
            StartCoroutine(Fade(false, 1)); //fade to white
        }
    }
    private IEnumerator ChangeWinds()
    {
        _audio.clip = _slowWindSound.clip;
        _audio.outputAudioMixerGroup = _slowWindSound.mixer;
        _audio.Play();
        yield return new WaitForSeconds(_windSpeedRateOfChange); //wait until the intital wind speed changes
        while (true)
        {
            _windInt = _windRate[Random.Range(0, _windRate.Length)];
            if (_windInt < _windRate[_windRate.Length - 1])
            {
                _audio.clip = _slowWindSound.clip;
                _audio.outputAudioMixerGroup = _slowWindSound.mixer;
                _audio.Play();
            }
            else
            {
                _audio.clip = _fastWindSound.clip;
                _audio.outputAudioMixerGroup = _fastWindSound.mixer;
                _audio.Play();
            }
            _windSpeed.text = _windInt.ToString() + " mph";
            yield return new WaitForSeconds(_windSpeedRateOfChange);
        }
    }
    
    private IEnumerator Fade(bool fadeIn, float speed)
    {
        Color targetColor = fadeIn ? new Color(0, 0, 0, 0) : new Color(1, 1, 1, 1);
        Color sourceColor = fadeIn ? new Color(1, 1, 1, 1) : new Color(0, 0, 0, 0);

        float timer = 0;
        while (timer <= _fadeDuration)
        {
            _screenTransition.color = Color.Lerp(sourceColor, targetColor, timer / speed);
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
       _screenTransition.color = targetColor;
    }
    private void LoadDeathScreen()
    {
        if (0<=_elevationInt && (_elevationInt<(endPosition-startPosition-1)))
        {
            SceneManager.LoadScene("DeathScreen1");
        }
        else if (((endPosition-startPosition-1)<_elevationInt) && (_elevationInt <=((endPosition-startPosition) * 2)-2))
        {
            SceneManager.LoadScene("DeathScreen2");
        }
        else
        {
            SceneManager.LoadScene("DeathScreen3");
        }
    }
}
