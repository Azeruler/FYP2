using UnityEngine;
using UnityEngine.UI;

public class ScrollSelect : MonoBehaviour
{
    public Button[] options; // The UI options
    private int selectedIndex = 0; // The currently selected option
    private int previousIndex = 0; // The previously selected option
    private Color normalColor; // Normal color of the buttons
    public Color highlightColor = Color.gray; // Color of the selected button

    private DoorRaycast doorRaycast;

    void Start()
    {
        // Store the normal color of the buttons
        normalColor = options[0].GetComponent<Image>().color;
        // Highlight the first selected option
        options[selectedIndex].GetComponent<Image>().color = highlightColor;

        doorRaycast = FindObjectOfType<DoorRaycast>();
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            // Reset the color of the previously selected button
            options[previousIndex].GetComponent<Image>().color = normalColor;

            if (scroll > 0) selectedIndex--; // Scroll up
            else selectedIndex++; // Scroll down

            // Wrap around the options
            if (selectedIndex < 0) selectedIndex = options.Length - 1;
            if (selectedIndex >= options.Length) selectedIndex = 0;

            // Highlight the selected option
            options[selectedIndex].GetComponent<Image>().color = highlightColor;
            options[selectedIndex].Select();

            // Update the previously selected index
            previousIndex = selectedIndex;
        }

        // Check for click to confirm selection
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Get the hit information
            RaycastHit hitInfo = doorRaycast.GetHitInfo();
            // Check if the hit point is on the interactive object and the player can interact with it
            if (hitInfo.collider != null && hitInfo.collider.CompareTag("InteractiveObject"))
            {
                Debug.Log("Key pressed. Interactable door hit.");
                options[selectedIndex].onClick.Invoke();
            }
        }
    }
}
