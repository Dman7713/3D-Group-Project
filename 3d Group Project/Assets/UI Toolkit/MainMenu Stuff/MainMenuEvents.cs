using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Call this method when the Play button is pressed
    public void PlayGame()
    {
        // Change "YourGameScene" to the name of the scene you want to load
        SceneManager.LoadScene("Main Scene");
    }

    // Call this method when the Quit button is pressed
    public void QuitGame()
    {
        Application.Quit();

        // If running in the Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
