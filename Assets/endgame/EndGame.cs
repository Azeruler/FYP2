using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndGame : MonoBehaviour
{
    public Image fadeImage; // Assign the black UI Image in the Inspector
    public AudioSource audioSource; // Assign the AudioSource in the Inspector
    public float fadeDuration = 1.0f; // Duration of the fade effect
    public string sceneToLoad; // The name of the scene to load

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(EndGameSequence());
        }
    }

    private IEnumerator EndGameSequence()
    {
        // Start fading to black
        yield return StartCoroutine(FadeToBlack());

        // Play the audio
        audioSource.Play();

        // Wait for the audio to finish
        yield return new WaitForSeconds(audioSource.clip.length);

        // Load the new scene
        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator FadeToBlack()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
    }
}
