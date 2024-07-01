using UnityEngine;
using UnityEngine.InputSystem;

public class Flashlight : MonoBehaviour
{
    public GameObject flashlight;
    public AudioSource turnOn;
    public AudioSource turnOff;

    private bool isOn = false;

    void Update()
    {
        if (!isOn && Input.GetMouseButtonDown(2))
        {
            flashlight.SetActive(true);
            isOn = true;
            turnOn.Play();
            Debug.Log("Flashlight turned on");
        }
        else if (isOn && Input.GetMouseButtonDown(2))
        {
            flashlight.SetActive(false);
            isOn = false;
            turnOff.Play();
            Debug.Log("Flashlight turned off");
        }
    }
}
