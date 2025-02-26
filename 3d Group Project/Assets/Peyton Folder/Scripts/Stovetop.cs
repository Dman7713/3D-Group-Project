using UnityEngine;
using System.Collections;


public class TimedDestruction : MonoBehaviour
{
    public string targetLayer = "TargetLayer"; // Layer the object needs to touch
    public float timeToDestroy = 5f; // Time before destruction
    public float blackenStartTime = 1f; // Time before the object starts turning black
    private float timer;
    private Renderer objectRenderer;

    void Start()
    {
        timer = timeToDestroy;
        objectRenderer = GetComponent<Renderer>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the object collides with the specified layer
        if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayer))
        {
            StartCoroutine(DestroyAfterTime());
        }
    }

    IEnumerator DestroyAfterTime()
    {
        while (timer > 0)
        {
            // Decrease the timer
            timer -= Time.deltaTime;

            // Check if we should start turning the object black
            if (timer <= blackenStartTime && objectRenderer != null)
            {
                float blackenAmount = (timeToDestroy - timer) / blackenStartTime; // How much to change color
                objectRenderer.material.color = Color.Lerp(objectRenderer.material.color, Color.black, blackenAmount);
            }

            yield return null;
        }

        // Destroy the object once the timer reaches 0
        Destroy(gameObject);
    }
}
