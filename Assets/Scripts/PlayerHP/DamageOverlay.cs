using UnityEngine;
using UnityEngine.UI;

public class DamageOverlay : MonoBehaviour
{
    public Image overlayImage;

    public void SetHealthOverlay(float healthPercentage)
    {
        Color color = overlayImage.color;
        color.a = 1 - healthPercentage; // Inverse the health percentage to get the alpha value
        overlayImage.color = color;
    }
}
