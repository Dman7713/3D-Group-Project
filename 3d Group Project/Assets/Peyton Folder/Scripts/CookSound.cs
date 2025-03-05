using UnityEngine;

public class PlaySoundOnCollision3D : MonoBehaviour
{
    public AudioSource audioSource;

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has the tag "Food" or "Burnt"
        if (collision.gameObject.CompareTag("Food") || collision.gameObject.CompareTag("Burnt") || collision.gameObject.CompareTag("Cuttable"))
        {
            Debug.Log("Collision with: " + collision.gameObject.name); // Debugging
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Stop sound only if exiting a "Food" or "Burnt" object
        if (collision.gameObject.CompareTag("Food") || collision.gameObject.CompareTag("Burnt") || collision.gameObject.CompareTag("Cuttable"))
        {
            audioSource.Stop();
        }
    }
}
