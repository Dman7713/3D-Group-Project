using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    public Animator doorAnimator; // Animator component of the door
    public string openTrigger = "Open"; // Trigger name for opening the door
    public string closeTrigger = "Close"; // Trigger name for closing the door

    private bool isOpen = false; // Track whether the door is open or closed

    void Update()
    {
        // Detect left mouse button click
        if (Input.GetMouseButtonDown(0))
        {
            ToggleDoor();
        }
    }

    // Toggle the door's state (open or close)
    private void ToggleDoor()
    {
        if (isOpen)
        {
            doorAnimator.SetTrigger(closeTrigger); // Close the door
        }
        else
        {
            doorAnimator.SetTrigger(openTrigger); // Open the door
        }

        isOpen = !isOpen; // Toggle the door's state
    }
}
