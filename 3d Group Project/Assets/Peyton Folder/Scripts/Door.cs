using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    public Animator doorAnimator; // Animator component of the door
    private bool isOpen = false; // Track whether the door is open

    void Start()
    {
        doorAnimator.SetBool("IsOpen", false); // Ensure door starts closed
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click to toggle door
        {
            ToggleDoor();
        }
    }

    private void ToggleDoor()
    {
        isOpen = !isOpen; // Toggle state
        doorAnimator.SetBool("IsOpen", isOpen); // Update Animator parameter
    }
}
