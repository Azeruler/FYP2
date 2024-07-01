using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public InventoryManager inventoryManager; // Reference to the InventoryManager

    // Start is called before the first frame update
    void Start()
    {
        // Initially hide and lock the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the inventory menu is active
        bool inventoryActive = inventoryManager.IsInventoryActive();

        // Set cursor visibility based on inventory menu state
        Cursor.visible = inventoryActive;
        Cursor.lockState = inventoryActive ? CursorLockMode.None : CursorLockMode.Locked;

        // Debug logs to check cursor state
        Debug.Log("Inventory Active: " + inventoryActive);
        Debug.Log("Cursor Visible: " + Cursor.visible);
        Debug.Log("Cursor Lock State: " + Cursor.lockState);
    }
}
