using UnityEngine;

public class TagCollisionDestroy : MonoBehaviour
{
    public string requiredTag = "Stovetop"; // The tag the object needs to collide with
    public string targetTag = "Pickup"; // The tag of the object that will be destroyed
    public float collisionTimeLimit = 2f; // Time in seconds before the object is destroyed
    private float collisionTime = 0f;
    private bool isColliding = false;

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Check if the other object has the required tag
        if (collision.gameObject.CompareTag(requiredTag))
        {
            if (!isColliding)
            {
                isColliding = true;
                collisionTime = 0f; // Reset the timer when collision starts
            }

            // If still colliding, count the time
            collisionTime += Time.deltaTime;

            // If the time exceeds the limit, destroy the target object
            if (collisionTime >= collisionTimeLimit && gameObject.CompareTag(targetTag))
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Stop the timer if no longer colliding
        if (collision.gameObject.CompareTag(requiredTag))
        {
            isColliding = false;
            collisionTime = 0f;
        }
    }
}
