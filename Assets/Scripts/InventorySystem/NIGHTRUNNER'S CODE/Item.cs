using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemSO itemSO;
    [SerializeField] private int quantity = 1;
    public Camera playerCamera;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private Image interactableImage;

    private InventoryManager inventoryManager;
    private bool isInteracting = false;

    void Start()
    {
        //Check for player camera reference
        if (playerCamera == null)
        {
            Debug.LogError("Player camera reference not set in Item script.");
            return;
        }

        //Check for InventoryManager script on InventoryCanvas
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found on InventoryCanvas.");
        }

        interactableImage.enabled = false;
        //Debug.Log("Item initialized with quantity: " + quantity);
    }

    void Update()
    {
        if (isInteracting) return;

        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.green);

        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            Item hitItem = hit.collider.GetComponent<Item>();
            if (hitItem != null)
            {
                hitItem.ShowInteractableImage();
                if (Input.GetKeyDown(KeyCode.G) && !isInteracting)
                {
                    Debug.Log("Interaction detected with item: " + hitItem.itemSO.itemName);
                    Debug.Log("Quantity to add: " + hitItem.quantity);
                    StartCoroutine(HandleInteraction(hitItem));
                }
            }
        }
        else
        {
            HideAllInteractableImages();
        }
    }

    IEnumerator HandleInteraction(Item hitItem)
    {
        isInteracting = true;
        Debug.Log("Starting interaction with item: " + hitItem.itemSO.itemName);

        int leftOverItems = inventoryManager.AddItem(hitItem.itemSO.itemName, hitItem.quantity, hitItem.itemSO.sprite, hitItem.itemSO.itemDescription);
        Debug.Log($"Leftover items after adding: {leftOverItems}");

        if (leftOverItems <= 0)
        {
            Debug.Log("Destroying gameObject: " + hitItem.gameObject.name);
            Destroy(hitItem.gameObject);
        }
        else
        {
            hitItem.quantity = leftOverItems;
        }

        hitItem.HideInteractableImage();
        isInteracting = false;
        Debug.Log("Ending interaction with item: " + hitItem.itemSO.itemName);
        yield return null;
    }

    void ShowInteractableImage()
    {
        interactableImage.enabled = true;
        if (interactableImage.enabled == true)
        {
            Debug.Log("Press G to pick up item: " + itemSO.itemName);
        }
    }

    void HideInteractableImage()
    {
        interactableImage.enabled = false;
    }

    void HideAllInteractableImages()
    {
        Item[] allItems = FindObjectsOfType<Item>();
        foreach (Item item in allItems)
        {
            item.HideInteractableImage();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
