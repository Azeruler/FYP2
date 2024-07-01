using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    private bool menuActivated;

    public itemSlot[] itemSlots;
    public ItemSO[] itemSOs;

    private PlayerHealth playerHealth;

    void Start()
    {
        InventoryMenu.SetActive(false); // Ensure the inventory menu is initially hidden
        menuActivated = false;
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            menuActivated = !menuActivated;
            InventoryMenu.SetActive(menuActivated);
        }
    }

    public bool UseItem(string itemName)
    {
        foreach (var itemSO in itemSOs)
        {
            if (itemSO.itemName == itemName)
            {
                bool usable = itemSO.UseItem(playerHealth);
                return usable;
            }
        }
        return false;
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        Debug.Log($"AddItem called with itemName: {itemName}, quantity: {quantity}");

        int remainingQuantity = quantity;
        foreach (var slot in itemSlots)
        {
            if (!slot.isFull && (slot.quantity == 0 || slot.itemName == itemName))
            {
                remainingQuantity = slot.AddItem(itemName, remainingQuantity, itemSprite, itemDescription);
                Debug.Log($"Slot {slot.name} now has {slot.quantity} items of {itemName}");
                if (remainingQuantity <= 0) break;
            }
        }

        Debug.Log($"AddItem completed for itemName: {itemName}, remainingQuantity: {remainingQuantity}");
        return remainingQuantity;
    }

    public void DeselectAllSlots()
    {
        foreach (var slot in itemSlots)
        {
            slot.selectedShader.SetActive(false);
            slot.thisItemSelected = false;
        }

        if (itemSlots.Length > 0)
        {
            itemSlots[0].itemDescriptionNameText.text = "";
            itemSlots[0].itemDescriptionText.text = "";
            itemSlots[0].itemDescriptionImage.sprite = itemSlots[0].emptySprite;
        }
    }

    public bool IsInventoryActive()
    {
        return menuActivated;
    }

    public itemSlot GetItemSlotByName(string itemName)
    {
        foreach (var slot in itemSlots)
        {
            if (slot.itemName == itemName && slot.quantity > 0)
            {
                return slot;
            }
        }
        return null;
    }
}
