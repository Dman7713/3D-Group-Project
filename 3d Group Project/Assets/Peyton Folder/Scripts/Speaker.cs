using UnityEngine;

public class SpeakerSongChanger : MonoBehaviour
{
    public AudioSource audioSource; // Assign your AudioSource
    public AudioClip[] songs; // Drag multiple songs into the Inspector
    private int currentSongIndex = 0;

    void Awake()
    {
        if (songs.Length > 0 && audioSource != null)
        {
            audioSource.clip = songs[0]; // Set the first song
            audioSource.Play(); // Play the first song at start
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Right-click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform) // Check if the speaker was clicked
                {
                    ChangeSong();
                }
            }
        }
    }

    void ChangeSong()
    {
        if (songs.Length == 0) return;

        currentSongIndex = (currentSongIndex + 1) % songs.Length; // Cycle through songs
        audioSource.clip = songs[currentSongIndex];
        audioSource.Play();
    }
}
