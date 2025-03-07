using UnityEngine;

public class KeyTouchDoor : MonoBehaviour
{
    // This method will be called when a trigger collision happens
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object this script is attached to has collided with an object tagged "Door"
        if (other.gameObject.CompareTag("Door"))
        {
            // Destroy the door object
            Destroy(other.gameObject);
        }
    }
}
