using UnityEngine;
using Demo.Scripts.Runtime.Character;
using KINEMATION.FPSAnimationFramework.Runtime.Core;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public DamageOverlay damageOverlay;
    public CameraShake cameraShake;
    public Ragdoll ragdoll;

    public FPSController fpsController;
    public FPSAnimator fpsAnimator;


    void Start()
    {
        Debug.Log("PlayerHealth Start called");

        currentHealth = maxHealth;
        damageOverlay.SetHealthOverlay(1.0f); // Fully transparent at the start
        ragdoll.DeactivateRagdoll();

        // Find FPSController in the scene
        fpsController = FindObjectOfType<FPSController>();
    }

    void Update()
    {
        // //This is just for testing, you can remove this line later
        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    TakeDamage(20);
        //}
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        float healthPercentage = (float)currentHealth / maxHealth;
        damageOverlay.SetHealthOverlay(healthPercentage);

        if (currentHealth <= 0)
        {
            Die();
        }
        cameraShake.Shake(0.1f, 0.2f);
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        float healthPercentage = (float)currentHealth / maxHealth;
        damageOverlay.SetHealthOverlay(healthPercentage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    void Die()
    {
        // Handle the player's death
        Debug.Log("Player Died");

        fpsController.RemoveAllWeapons();

        // Disable FPS framework systems
        fpsController.enabled = false;
        fpsAnimator.enabled = false;

        ragdoll.ActivateRagdoll();

        DisableFPSComponents();

        void DisableFPSComponents()
        {
            // Example: Find and disable all colliders and rigidbodies used by FPS framework systems
            Collider[] colliders = GetComponentsInChildren<Collider>();
            Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }

            foreach (Rigidbody rb in rigidbodies)
            {
                rb.isKinematic = true; // Or rb.useGravity = false; depending on your setup
            }
        }


        // For example, reload the scene
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
