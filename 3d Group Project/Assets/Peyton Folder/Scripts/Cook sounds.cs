using UnityEngine;

public class StovetopSound : MonoBehaviour
{
    public AudioSource audioSource; // Assign this in the Inspector
    private int touchingObjects = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pickup"))
        {
            touchingObjects++;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Pickup"))
        {
            touchingObjects--;
            if (touchingObjects <= 0)
            {
                audioSource.Stop();
                touchingObjects = 0;
            }
        }
    }
}
