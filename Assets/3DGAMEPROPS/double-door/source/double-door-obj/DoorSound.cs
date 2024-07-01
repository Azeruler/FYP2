using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DoorSound : MonoBehaviour
{
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the player or any object you want
        if (collision.gameObject.CompareTag("Player"))
        {
            // Play the sound
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }
}
