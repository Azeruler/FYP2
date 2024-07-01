using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyDoorController : MonoBehaviour
{
    public Animator doorAnim;
    private Rigidbody doorRb;
    private BoxCollider boxCollider;

    public TMP_Text buttonText;
    public Button OpenCloseButton;

    private bool doorOpen = false;
    private bool isAnimating = false;

    public bool doorAnimplay = true;

    private void Awake()
    {
        doorAnim = GetComponent<Animator>();
        doorRb = gameObject.GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        buttonText = OpenCloseButton.GetComponentInChildren<TMP_Text>();

        DisablePhy();
        OpenCloseButton.onClick.AddListener(StartOpenCloseDoor);
    }

    public void DisablePhy()
    {
        boxCollider.enabled = false;
        doorRb.isKinematic = true;
    }

    public IEnumerator OpenCloseDoor()
    {
        if (isAnimating)
        {
            yield break;
        }

        isAnimating = true;

        if (!doorOpen)
        {
            buttonText.text = "Close";
            doorAnim.Play("dooropen", 0, 0.0f);
            doorOpen = true;
        }
        else
        {
            buttonText.text = "Open";
            doorAnim.Play("doorclose", 0, 0.0f);
            doorOpen = false;
        }

        yield return new WaitForSeconds(doorAnim.GetCurrentAnimatorStateInfo(0).length);
        isAnimating = false;
    }

    public void StartOpenCloseDoor()
    {
        StartCoroutine(OpenCloseDoor());
    }

    public void OpenDoorFully()
    {
        doorRb.isKinematic = false;
        doorRb.AddForce(transform.forward * -500);
    }
}
