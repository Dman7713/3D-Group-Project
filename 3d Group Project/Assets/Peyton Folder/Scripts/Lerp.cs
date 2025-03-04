using UnityEngine;

public class LerpMover : MonoBehaviour
{
    public Vector3 targetPosition; // The target position to move to
    public float lerpSpeed = 1f;   // Speed at which the object moves

    private void Update()
    {
        // Move the object from its current position to the target position over time
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
    }
}
