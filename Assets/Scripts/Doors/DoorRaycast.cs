using UnityEngine;
using UnityEngine.UI;

public class DoorRaycast : MonoBehaviour
{
    [SerializeField] private int rayLength = 5;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private string excludeLayerName = null;

    private MyDoorController raycastedObj;
    private bool doOnce;
    private bool isInteractableDoorHit;

    private const string interactableTag = "InteractiveObject";

    public GameObject door;
    public Canvas canvas;
    private RaycastHit hitInfo;

    private void Update()
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        int mask = 1 << LayerMask.NameToLayer(excludeLayerName) | layerMaskInteract.value;

        // Draw the debug ray
        Debug.DrawRay(transform.position, fwd * rayLength, Color.red);

        if (Physics.Raycast(transform.position, fwd, out hitInfo, rayLength, mask))
        {
            GameObject hitObject = hitInfo.collider.gameObject;
            Debug.Log("Hit object: " + hitObject.name);

            if (hitObject.CompareTag(interactableTag))
            {
                Debug.Log("Interactable door detected.");
                EnableGUI();
                if (!doOnce)
                {
                    raycastedObj = hitObject.GetComponent<MyDoorController>();
                    if (raycastedObj != null)
                    {
                        Debug.Log("Door controller found.");
                    }
                    else
                    {
                        Debug.LogError("No Door controller found on the hit object: " + hitObject.name);
                        // Debugging: List all components attached to the hit object
                        Component[] components = hitObject.GetComponents<Component>();
                        foreach (var component in components)
                        {
                            Debug.Log("Component: " + component.GetType().Name);
                        }
                    }
                }
                doOnce = true;
            }
        }
        else
        {
            Debug.Log("No interactable door detected.");
            DisableGUI();
            isInteractableDoorHit = false;
            doOnce = false;
        }
    }

    private void EnableGUI()
    {
        Debug.Log("Enabling GUI.");
        if (canvas != null)
        {
            canvas.enabled = true;
        }
        else
        {
            Debug.LogWarning("Canvas is not assigned.");
        }
    }

    private void DisableGUI()
    {
        Debug.Log("Disabling GUI.");
        if (canvas != null)
        {
            canvas.enabled = false;
        }
        else
        {
            Debug.LogWarning("Canvas is not assigned.");
        }
    }

    public RaycastHit GetHitInfo()
    {
        return hitInfo;
    }
}
