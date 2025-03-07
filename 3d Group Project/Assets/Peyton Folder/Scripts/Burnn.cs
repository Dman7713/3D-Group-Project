using UnityEngine;

public class BurntObject : MonoBehaviour
{
    public GameObject objectToSpawn; // Object to spawn after destruction
    public ParticleSystem burntParticles; // Reference to the particle system
    private bool isBurnt = false; // Flag to check if it's already burnt

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Burnt") && !isBurnt)
        {
            // Play particles and set burnt flag
            burntParticles.Play();
            isBurnt = true;

            // Destroy the object after 5 seconds
            Destroy(gameObject, 5f);

            // Call the SpawnObject function directly after the delay
            Invoke("SpawnObject", 5f);
        }
    }

    // Spawn the object after 5 seconds
    void SpawnObject()
    {
        if (objectToSpawn != null)
        {
            // Instantiate the new object at the original object's position
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        }
    }
}
