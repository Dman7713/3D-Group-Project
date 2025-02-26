using UnityEngine;
using System.Collections;

public class TimedDestruction : MonoBehaviour
{
    public string targetLayer = "TargetLayer"; // Layer the object needs to touch
    public string stovetopLayer = "Stovetop"; // Layer to check for touching the stovetop
    public float timeToDestroy = 15f; // Time before destruction
    public float blackenStartTime = 5f; // Time before the object starts turning dark grey
    public ParticleSystem smokeParticles; // Reference to the smoke particle system
    private float timer;
    private Renderer objectRenderer;
    private bool isTouchingStovetop = false; // Whether the object is touching the stovetop layer

    void Start()
    {
        timer = timeToDestroy;
        objectRenderer = GetComponent<Renderer>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the object collides with the specified target layer
        if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayer))
        {
            StartCoroutine(DestroyAfterTime());
        }

        // Check if the object collides with the stovetop layer
        if (collision.gameObject.layer == LayerMask.NameToLayer(stovetopLayer))
        {
            isTouchingStovetop = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // If the object exits collision with the stovetop layer, stop the timer
        if (collision.gameObject.layer == LayerMask.NameToLayer(stovetopLayer))
        {
            isTouchingStovetop = false;
        }
    }

    IEnumerator DestroyAfterTime()
    {
        while (timer > 0)
        {
            // Pause the timer if not touching the stovetop
            if (!isTouchingStovetop)
            {
                yield return null; // Wait for next frame without decreasing timer
                continue; // Skip the rest of the loop
            }

            // Decrease the timer
            timer -= Time.deltaTime;

            // Check if we should start turning the object dark grey
            if (timer <= blackenStartTime && objectRenderer != null)
            {
                // How much to change color
                float blackenAmount = (timeToDestroy - timer) / blackenStartTime;

                // Loop through all materials and change their color to dark grey
                foreach (Material mat in objectRenderer.materials)
                {
                    mat.color = Color.Lerp(mat.color, new Color(0.2f, 0.2f, 0.2f), blackenAmount); // Dark grey
                }

                // Change the tag to "Burnt" when it starts turning dark grey
                if (timer <= blackenStartTime && tag != "Burnt")
                {
                    tag = "Burnt";

                    // Change smoke particle mesh material color to dark grey
                    if (smokeParticles != null)
                    {
                        var smokeRenderer = smokeParticles.GetComponent<Renderer>(); // Get the particle system's renderer
                        if (smokeRenderer != null)
                        {
                            // Set the color of the mesh material to dark grey
                            smokeRenderer.material.color = new Color(0.2f, 0.2f, 0.2f); // Dark grey
                        }
                    }
                }
            }

            yield return null;
        }

        // Destroy the object once the timer reaches 0
        Destroy(gameObject);
    }
}
