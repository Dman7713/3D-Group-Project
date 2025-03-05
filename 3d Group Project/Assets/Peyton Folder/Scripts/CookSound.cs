using UnityEngine;

public class PlaySoundOnCollision3D : MonoBehaviour
{
    public AudioSource audioSource;
    private int collisionCount = 0; // Track number of active collisions

    private void OnCollisionEnter(Collision collision)
    {
        if (IsValidTag(collision.gameObject))
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
        if (IsValidTag(collision.gameObject))
        {
            collisionCount--; // Decrease collision count

            if (collisionCount <= 0)
            {
                StopSound();
            }
        }
    }

    // Call this when the food is replaced to avoid sound issues
    public void ResetCollision()
    {
        Debug.Log("Food replaced, resetting collision count.");
        collisionCount = 0;
        audioSource.Stop();
    }

    private bool IsValidTag(GameObject obj)
    {
        return obj.CompareTag("Food") || obj.CompareTag("Burnt") || obj.CompareTag("Cuttable");
    }

    private void StopSound()
    {
        audioSource.Stop();
        collisionCount = 0; // Ensure count resets
    }
}
