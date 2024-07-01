using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
    public AudioSource audioSource; // The audio source to play footstep sounds
    public AudioClip[] footstepClips; // Array of footstep sounds
    public float stepInterval = 0.5f; // Time interval between footsteps
    private float stepTimer = 0f; // Timer to track footstep interval

    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (characterController.isGrounded && characterController.velocity.magnitude > 0.1f)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                PlayFootstepSound();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f; // Reset timer if not moving
        }
    }

    void PlayFootstepSound()
    {
        if (footstepClips.Length > 0)
        {
            int clipIndex = Random.Range(0, footstepClips.Length);
            audioSource.clip = footstepClips[clipIndex];
            audioSource.Play();
        }
    }
}
