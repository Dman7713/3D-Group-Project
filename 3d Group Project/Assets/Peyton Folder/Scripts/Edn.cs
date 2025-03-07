using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnTouch : MonoBehaviour
{
    // This function is called when another collider enters the trigger area of the object
    private void OnTriggerEnter(Collider other)
    {
        // Check if the other collider is the player
        if (other.CompareTag("Player"))
        {
            // Load the Main Menu scene
            SceneManager.LoadScene("MainMenu");
        }
    }
}
