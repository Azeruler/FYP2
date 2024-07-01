using UnityEngine;
using UnityEngine.InputSystem;

public class ClickHandler : MonoBehaviour
{
    private InputAction clickAction;

    private void OnEnable()
    {
        // Create a new InputAction reference
        clickAction = new InputAction("Click", InputActionType.Button, "<Mouse>/leftButton");

        // Enable the action
        clickAction.Enable();

        // Bind a callback function to the action
        clickAction.performed += ctx => HandleClick(ctx);
    }

    private void OnDisable()
    {
        // Disable the action
        clickAction.Disable();

        // Unbind the callback function
        clickAction.performed -= ctx => HandleClick(ctx);
    }

    private void HandleClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Object clicked!");
            // Add your logic for what happens when the object is clicked
        }
    }
}
