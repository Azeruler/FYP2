using UnityEngine;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }
    public Image interactableImage; // Reference to the UI Image
    public Text itemNameText; // Text to display item name

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowInteractableImage(Vector3 position, string itemName)
    {
        Debug.Log("Showing interactable image for item: " + itemName);
        interactableImage.transform.position = position;
        interactableImage.enabled = true;
        itemNameText.text = itemName;
        itemNameText.enabled = true;
    }

    public void HideInteractableImage()
    {
        Debug.Log("Hiding interactable image");
        interactableImage.enabled = false;
        itemNameText.enabled = false;
    }
}
