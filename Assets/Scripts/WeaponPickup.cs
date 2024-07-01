using UnityEngine;
using Demo.Scripts.Runtime;
using Demo.Scripts.Runtime.Item;
using Demo.Scripts.Runtime.Character;

public class WeaponPickup : MonoBehaviour
{
    public float pickupRange = 5f; // The range within which you can pick up the weapon
    public Transform playerCamera; // The camera attached to the player
    public LayerMask weaponLayer; // The layer that the weapons are on
    public FPSController fpsController; // Reference to the FPSController

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            TryPickupWeapon();
        }

        DebugRaycast();
    }

    private void TryPickupWeapon()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Player camera is not assigned in the WeaponPickup script.");
            return;
        }

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        Debug.Log("Attempting to raycast...");

        if (Physics.Raycast(ray, out hit, pickupRange, weaponLayer))
        {
            Debug.Log("Raycast hit: " + hit.transform.name);

            Weapon weapon = hit.transform.GetComponent<Weapon>();

            if (weapon != null)
            {
                Pickup(weapon);
            }
            else
            {
                Debug.Log("No Weapon component found on the hit object.");
            }
        }
        else
        {
            Debug.Log("Raycast did not hit any weapon.");
        }
    }

    private void Pickup(Weapon weapon)
    {
        Debug.Log("Picked up weapon: " + weapon.weaponName);
        BoxCollider bc = weapon.GetComponent<BoxCollider>();
        if (bc != null)
        {
            Destroy(bc);
        }

        fpsController.AddWeaponToInventory(weapon);
        Destroy(weapon.gameObject);
    }

    private void DebugRaycast()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Player camera is not assigned in the WeaponPickup script.");
            return;
        }

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * pickupRange, Color.red);
    }
}
