using UnityEngine;

public class PlaySoundOnCollision3D : MonoBehaviour
{
    public AudioSource audioSource;
    private int collisionCount = 0; // Track number of active collisions

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Food") || collision.gameObject.CompareTag("Burnt") || collision.gameObject.CompareTag("Cuttable"))
        {
            Debug.Log("Collision with: " + collision.gameObject.name); // Debugging
            collisionCount++; // Increase collision count

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Food") || collision.gameObject.CompareTag("Burnt") || collision.gameObject.CompareTag("Cuttable"))
        {
            collisionCount--; // Decrease collision count

            // Only stop if no objects remain
            if (collisionCount <= 0)
            {
                audioSource.Stop();
                collisionCount = 0; // Ensure it doesn’t go negative
            }
        }
    }
}
