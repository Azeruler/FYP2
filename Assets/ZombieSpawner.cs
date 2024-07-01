using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab; // Assign the zombie prefab in the Inspector
    public Transform[] spawnPoints; // Assign the spawn points in the Inspector
    public AudioSource audioSource; // Assign the AudioSource in the Inspector
    private bool hasTriggered = false; // Flag to check if trigger has already been activated

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player and if the trigger hasn't been activated yet
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true; // Set the flag to true to prevent re-triggering

            // Play the sound
            if (audioSource != null)
            {
                audioSource.Play();
            }

            // Spawn a zombie at each spawn point
            foreach (Transform spawnPoint in spawnPoints)
            {
                Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);
            }
        }
    }
}
