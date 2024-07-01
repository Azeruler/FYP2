using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    public int damage = 10;

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure your player has a tag named "Player"
        {
            // Assume player has a method to take damage
            //other.GetComponent<PlayerHealth>().TakeDamage(damage);
            Debug.Log("Player hit!");
        }
    }
}
