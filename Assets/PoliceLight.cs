using UnityEngine;

public class PoliceLight : MonoBehaviour
{
    public Light pointLight; // Assign the Point Light in the Inspector
    public float changeInterval = 0.5f; // Time interval between color changes

    private Color redColor = Color.red;
    private Color blueColor = Color.blue;
    private float timer = 0f;

    void Start()
    {
        if (pointLight == null)
        {
            pointLight = GetComponent<Light>();
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= changeInterval)
        {
            // Toggle between red and blue
            if (pointLight.color == redColor)
            {
                pointLight.color = blueColor;
            }
            else
            {
                pointLight.color = redColor;
            }

            timer = 0f; // Reset the timer
        }
    }
}
