using UnityEngine;
using TMPro; // Use TMPro for TextMeshPro
using Demo.Scripts.Runtime.Item;

public class AmmoDisplay : MonoBehaviour
{
    public Weapon weapon; // Reference to the weapon script where ammo count is stored
    public TextMeshProUGUI ammoText; // Reference to the UI Text element to display ammo count

    public float fadeDuration = 1.0f; // Duration in seconds for the fade-in effect
    private float currentFadeTime = 0f; // Current time for lerping

    void Start()
    {
        // Find the Weapon script in the scene
        weapon = FindObjectOfType<Weapon>();

        // Get reference to the TextMeshProUGUI component
        ammoText = GetComponent<TextMeshProUGUI>();

        // Ensure ammoText is not null
        if (ammoText == null)
        {
            Debug.LogError("AmmoText UI element not found or not assigned in AmmoDisplay script.");
        }

        // Initially hide the ammo text
        SetAmmoTextAlpha(0f);
    }

    void Update()
    {
        // Example: Display ammo count when 'R' key is pressed for debugging
        if (Input.GetKey(KeyCode.R))
        {
            UpdateAmmoText();
        }
        else
        {
            // Hide the ammo text when 'R' key is released
            SetAmmoTextAlpha(0f);
        }
    }

    void UpdateAmmoText()
    {
        // Update the text to show current ammo count
        if (weapon != null)
        {
            ammoText.text = "Ammo: " + weapon.currentAmmoCount.ToString();

            // Start fading in the ammo text
            FadeInAmmoText();
        }
        else
        {
            Debug.LogWarning("Weapon reference is null. Ammo text update skipped.");
        }
    }

    void FadeInAmmoText()
    {
        // Calculate normalized time for lerping
        currentFadeTime += Time.deltaTime;
        float normalizedTime = currentFadeTime / fadeDuration;
        float alpha = Mathf.Lerp(0f, 1f, normalizedTime);

        // Update the alpha value of ammoText
        SetAmmoTextAlpha(alpha);

        // Reset currentFadeTime if fade is complete
        if (normalizedTime >= 1f)
        {
            currentFadeTime = 0f;
        }
    }

    void SetAmmoTextAlpha(float alpha)
    {
        // Get current color and update alpha
        Color textColor = ammoText.color;
        textColor.a = alpha;
        ammoText.color = textColor;
    }
}
