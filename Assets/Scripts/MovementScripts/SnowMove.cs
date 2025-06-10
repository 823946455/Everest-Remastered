using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowMove : MonoBehaviour
{
    [SerializeField] private float _walkSpeed = 0.05f;
    public float walkSpeed { get { return _walkSpeed; } }

    //privateInternals
    private bool _isCrouching = false;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        if (Input.GetKey("w"))
        {
            pos.x = pos.x + walkSpeed;
            transform.position = pos;
        }
        if (Input.GetKeyDown("c"))
        {
            if (!_isCrouching)
            {
                _walkSpeed = _walkSpeed / 2;
            }
            else
            {
                _walkSpeed *= 2;
            }
            _isCrouching = !_isCrouching;
        }
    }
}
