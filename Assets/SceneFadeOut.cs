using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SceneFadeOut : MonoBehaviour
{
    public Image fadeImage; // Assign the black UI Image in the Inspector
    public TextMeshProUGUI fadeText; // Assign the TextMeshProUGUI in the Inspector
    public float fadeDuration = 2.5f; // Duration of the fade effect
    public float timerDuration = 5.0f; // Duration of the timer before fade out starts

    private void Start()
    {
        // Set the initial alpha of the fade image and text to 0 (fully transparent)
        SetAlpha(fadeImage, 0f);
        SetAlpha(fadeText, 0f);

        // Start the timer coroutine
        StartCoroutine(TimerAndFadeOut());
    }

    private IEnumerator TimerAndFadeOut()
    {
        // Wait for the timer to reach zero
        yield return new WaitForSeconds(timerDuration);

        // Start the fade out effect
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        fadeImage.gameObject.SetActive(true); // Ensure the fade image is active
        fadeText.gameObject.SetActive(true); // Ensure the fade text is active

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            SetAlpha(fadeImage, alpha);
            SetAlpha(fadeText, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetAlpha(fadeImage, 1f);
        SetAlpha(fadeText, 1f);
    }

    private void SetAlpha(Graphic graphic, float alpha)
    {
        if (graphic != null)
        {
            Color color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }
    }

    private void SetAlpha(TextMeshProUGUI text, float alpha)
    {
        if (text != null)
        {
            Color color = text.color;
            color.a = alpha;
            text.color = color;
        }
    }
}
