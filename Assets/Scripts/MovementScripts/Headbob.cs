using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headbob : MonoBehaviour
{
    [Header("Headbob Settings")]
    public float bobFrequency = 5.0f; // How fast the bobbing happens
    public float bobAmplitude = 0.1f; // How high the bobbing goes
    public float transitionSpeed = 5.0f; // Speed of transitioning between movement and idle bobbing
    public Transform cameraTransform; // Assign your camera transform here
    public Transform playerTransform; // Reference to the player's transform

    [Header("Footstep Settings")]
    public AudioSource footstepAudioSource; // AudioSource component for footstep sounds
    public AudioClip[] footstepSounds; // Array of footstep sound clips
    public float stepInterval = 0.5f; // Time between footsteps (adjust based on bobFrequency)

    private Vector3 initialPosition; // Camera's initial local position
    private Vector3 lastPlayerPosition; // Last recorded position of the player
    private float bobbingTime;
    private float playerSpeed;
    private float timeSinceLastStep;

    void Start()
    {
        // Initialize the starting position of the camera
        if (cameraTransform == null)
            cameraTransform = transform;

        if (playerTransform == null)
            playerTransform = transform;

        if (footstepAudioSource == null)
            footstepAudioSource = GetComponent<AudioSource>();

        initialPosition = cameraTransform.localPosition;
        lastPlayerPosition = playerTransform.position;
    }

    void Update()
    {
        // Calculate player speed based on position change
        Vector3 playerMovement = playerTransform.position - lastPlayerPosition;
        playerSpeed = playerMovement.magnitude / Time.deltaTime;
        lastPlayerPosition = playerTransform.position;

        // Check if the player is moving
        if (playerSpeed > 0.1f)
        {
            // Calculate head bobbing
            bobbingTime += Time.deltaTime * bobFrequency;
            float offsetY = Mathf.Sin(bobbingTime) * bobAmplitude;

            // Apply bobbing to the camera
            cameraTransform.localPosition = new Vector3(initialPosition.x, initialPosition.y + offsetY, initialPosition.z);
            // Play footstep sound at the peak of the bob (when bobbingTime reaches half-cycle)
            timeSinceLastStep += Time.deltaTime;
            if (timeSinceLastStep >= stepInterval)
            {
                PlayFootstepSound();
                timeSinceLastStep = 0; // Reset step timer
            }
        }
        else
        {
            // Smoothly reset the camera position to its initial position when idle
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, initialPosition, Time.deltaTime * transitionSpeed);
            bobbingTime = 0; // Reset bobbing time to prevent abrupt starts
        }
        void PlayFootstepSound()
        {
            if (footstepSounds.Length > 0 && footstepAudioSource != null)
            {
                // Randomly select a footstep sound
                AudioClip footstepClip = footstepSounds[Random.Range(0, footstepSounds.Length)];
                footstepAudioSource.PlayOneShot(footstepClip);
            }
        }
    }
}
