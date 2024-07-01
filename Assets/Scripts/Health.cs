using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    [HideInInspector]
    public float currentHealth;
    private Animator animator;
    private Ragdoll ragdoll;
    private bool isDead; // New flag to check if the enemy is dead

    [Header("Audio Settings")]
    public AudioSource audioSource; // Single AudioSource to play both hit and death sounds
    public AudioClip[] hitClips; // Array of hit sounds
    public AudioClip deathClip; // Death sound

    void Start()
    {
        ragdoll = GetComponent<Ragdoll>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        isDead = false; // Initialize as not dead

        var rigidBodies = GetComponentsInChildren<Rigidbody>(); // Use GetComponentsInChildren to get an array of Rigidbody components
        foreach (var rigidBody in rigidBodies)
        {
            HitBox hitBox = rigidBody.gameObject.AddComponent<HitBox>(); // Use AddComponent to add the HitBox component
            hitBox.health = this;
        }
    }

    void Update()
    {
        // Debug.Log("Current Health is: " + currentHealth);
    }

    public void TakeDamage(float amount, Vector3 direction)
    {
        if (isDead) return; // Check if the enemy is already dead

        currentHealth -= amount;
        Debug.Log("Current Health is: " + currentHealth);

        // Trigger hit animation
        TriggerHitAnimation();

        // Play hit sound
        PlayHitSound();

        if (currentHealth <= 0.0f)
        {
            PlayDeathSound();
            Die();
        }
    }

    private void TriggerHitAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("isHit");
        }
    }

    private void Die()
    {
        isDead = true; // Set the flag to indicate the enemy is dead
        ragdoll.ActivateRagdoll();
        GetComponent<EnemyAI>()?.SetDead(); // Inform the AI script that the enemy is dead
    }


    private void PlayHitSound()
    {
        if (hitClips.Length > 0)
        {
            int clipIndex = Random.Range(0, hitClips.Length);
            audioSource.PlayOneShot(hitClips[clipIndex]);
        }
    }

    private void PlayDeathSound()
    {
        if (deathClip != null)
        {
            audioSource.PlayOneShot(deathClip);
        }
    }
}
