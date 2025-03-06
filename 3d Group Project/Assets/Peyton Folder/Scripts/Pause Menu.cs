using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    public MonoBehaviour playerMovement; // Reference to player's movement script
    public MonoBehaviour cameraMovement; // Reference to camera movement script
    public CinemachineBrain cinemachineBrain; // Reference to CinemachineBrain

    public Volume postProcessingVolume; // Reference to Post-Processing Volume
    private DepthOfField depthOfField; // Depth of Field effect

    void Start()
    {
        if (postProcessingVolume != null)
        {
            postProcessingVolume.profile.TryGet(out depthOfField);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (cameraMovement != null)
            cameraMovement.enabled = true;

        if (cinemachineBrain != null)
            cinemachineBrain.enabled = true; // Re-enable Cinemachine

        if (depthOfField != null)
            depthOfField.active = false; // Disable blur

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (cameraMovement != null)
            cameraMovement.enabled = false;

        if (cinemachineBrain != null)
            cinemachineBrain.enabled = false; // Disable Cinemachine

        if (depthOfField != null)
            depthOfField.active = true; // Enable blur

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
