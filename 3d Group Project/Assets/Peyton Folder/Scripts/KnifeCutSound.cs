using UnityEngine;

public class PlaySoundCut : MonoBehaviour
{
    private AudioSource audioSource;

    private void Start()
    {
        // Find the GameObject named "ItemCutSound" and get its AudioSource
        GameObject soundObject = GameObject.Find("ItemCutSound");
        if (soundObject != null)
        {
            audioSource = soundObject.GetComponent<AudioSource>();
        }
        else
        {
            Debug.LogWarning("ItemCutSound object not found in the scene.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has the "Cuttable" tag
        if (collision.gameObject.CompareTag("Cuttable"))
        {
            Debug.Log("Collision with: " + collision.gameObject.name); // Debugging
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Stop sound only if exiting a "Cuttable" object
        if (collision.gameObject.CompareTag("Cuttable") && audioSource != null)
        {
            audioSource.Stop();
        }
    }
}
