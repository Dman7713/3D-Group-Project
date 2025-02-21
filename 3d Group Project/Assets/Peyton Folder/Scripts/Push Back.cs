using UnityEngine;

public class DynamicSpringWithLimit : MonoBehaviour
{
    public Vector3 startPosition;  // The original position of the object
    public Vector3 pullBackPosition;  // The position to pull back to
    public float pullBackSpeed = 5f;  // Speed at which the spring pulls back
    public float returnSpeed = 10f;  // Speed at which the spring returns to its start position
    public float maxPullBackDistance = 5f;  // Maximum distance the object can be pulled back

    private bool isPulledBack = false;  // Whether the spring is pulled back or not
    private float currentPullBackDistance = 0f;  // The current distance the spring has been pulled back

    private void Start()
    {
        startPosition = transform.position;  // Set the starting position
    }

    private void Update()
    {
        // Handle dynamic pulling with input (could be replaced with any dynamic trigger)
        if (Input.GetKey(KeyCode.W))  // Hold W to pull back (or use another input)
        {
            // Calculate the current pull distance
            currentPullBackDistance += pullBackSpeed * Time.deltaTime;

            // Limit the distance to the max pull back distance
            if (currentPullBackDistance > maxPullBackDistance)
            {
                currentPullBackDistance = maxPullBackDistance;
            }

            // Calculate the new pull-back position based on the distance pulled
            pullBackPosition = startPosition + transform.forward * currentPullBackDistance;
        }
        else
        {
            // When the input is not held, return to the original position instantly
            isPulledBack = true;
        }

        // Move the object
        if (isPulledBack)
        {
            // Move instantly back to the start position
            transform.position = Vector3.MoveTowards(transform.position, startPosition, returnSpeed * Time.deltaTime);
        }
        else
        {
            // Gradually pull back to the desired position
            transform.position = Vector3.MoveTowards(transform.position, pullBackPosition, pullBackSpeed * Time.deltaTime);
        }

        // Example trigger to toggle pull-back and return behavior (optional)
        if (Input.GetKeyDown(KeyCode.Space))  // Toggle the spring behavior on spacebar
        {
            isPulledBack = !isPulledBack;
        }
    }
}
